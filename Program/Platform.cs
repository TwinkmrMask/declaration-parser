using Platform.Disposables;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Memory;
using Platform.Data;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Memory.Split.Specific;
using TLinkAddress = System.UInt32;
namespace database
{
    //Part of the code, along with comments, is taken from https://github.com/linksplatform/Comparisons.SQLiteVSDoublets/commit/289cf361c82ab605b9ba0d1621496b3401e432f7
    public class Platform : DisposableBase, IDefaultSettings
    {
        protected TLinkAddress currentMappingLinkIndex = 1;
        
        string indexFileName;
        string dataFileName;
        private readonly TLinkAddress _meaningRoot;
        private readonly TLinkAddress _unicodeSymbolMarker;
        private readonly TLinkAddress _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<TLinkAddress> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<TLinkAddress> _addressToNumberConverter;
        private readonly IConverter<string, TLinkAddress> _stringToUnicodeSequenceConverter;
        private readonly IConverter<TLinkAddress, string> _unicodeSequenceToStringConverter;
        private readonly ILinks<TLinkAddress> _disposableLinks;
        protected readonly ILinks<TLinkAddress> links;
    
        public Platform()
        {
            this.indexFileName = IDefaultSettings.indexFileName;
            this.dataFileName = IDefaultSettings.dataFileName;

            var dataMemory = new FileMappedResizableDirectMemory(this.dataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(this.indexFileName);

            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);

            // Init the links storage
            this._disposableLinks = new UInt32SplitMemoryLinks(dataMemory, indexMemory, UInt32SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); // Low-level logic
            this.links = new UInt32Links(_disposableLinks); // Main logic in the combined decorator

            // Set up constant links (markers, aka mapped links)
            
            this._meaningRoot = GetOrCreateMeaningRoot(currentMappingLinkIndex++);
            this._unicodeSymbolMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
            this._unicodeSequenceMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
            // Create converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            this._numberToAddressConverter = new RawNumberToAddressConverter<TLinkAddress>();
            this._addressToNumberConverter = new AddressToRawNumberConverter<TLinkAddress>();

            // Create converters that are able to convert string to unicode sequence stored as link and back
            var balancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<TLinkAddress>(links, _unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<TLinkAddress>(links, _unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLinkAddress>(links, _addressToNumberConverter, _unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<TLinkAddress>(links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<TLinkAddress>(links, new DefaultStack<TLinkAddress>(), unicodeSymbolCriterionMatcher.IsMatched);
            this._stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLinkAddress>(new StringToUnicodeSequenceConverter<TLinkAddress>(links, charToUnicodeSymbolConverter, balancedVariantConverter, _unicodeSequenceMarker));
            this._unicodeSequenceToStringConverter = new CachingConverterDecorator<TLinkAddress, string>(new UnicodeSequenceToStringConverter<TLinkAddress>(links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private TLinkAddress GetOrCreateMeaningRoot(TLinkAddress meaningRootIndex) => links.Exists(meaningRootIndex) ? meaningRootIndex : links.CreatePoint();
        protected TLinkAddress GetOrCreateNextMapping(TLinkAddress currentMappingIndex) => links.Exists(currentMappingIndex) ? currentMappingIndex : links.CreateAndUpdate(_meaningRoot, links.Constants.Itself);
        public string ConvertToString(TLinkAddress sequence) => _unicodeSequenceToStringConverter.Convert(sequence);
        public TLinkAddress ConvertToSequence(string @string) => _stringToUnicodeSequenceConverter.Convert(@string);
        
        protected override void Dispose(bool manual, bool wasDisposed)
        {
            if (!wasDisposed)
            {
                _disposableLinks.DisposeIfPossible();
            }
        }
    }
}
