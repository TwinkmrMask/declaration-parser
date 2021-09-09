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
    public class Platform : DisposableBase, IDefaultSettings
    {
        protected uint CurrentMappingLinkIndex = 1;
        private readonly uint _meaningRoot;
        private readonly IConverter<string, uint> _stringToUnicodeSequenceConverter;
        private readonly IConverter<uint, string> _unicodeSequenceToStringConverter;
        private readonly ILinks<uint> _disposableLinks;
        protected readonly ILinks<uint> Links;
        protected Platform()
        {
            var unicodeSymbolMarker = GetOrCreateNextMapping(CurrentMappingLinkIndex++);
            var unicodeSequenceMarker = GetOrCreateNextMapping(CurrentMappingLinkIndex++);
            
            var dataMemory = new FileMappedResizableDirectMemory(IDefaultSettings.IndexFileName);
            var indexMemory = new FileMappedResizableDirectMemory(IDefaultSettings.DataFileName);
            
            var linksConstants = new LinksConstants<uint>(enableExternalReferencesSupport: true);

            // Init the links storage
            this._disposableLinks = new UInt32SplitMemoryLinks(dataMemory, indexMemory, UInt32SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); // Low-level logic
            this.Links = new UInt32Links(_disposableLinks); // Main logic in the combined decorator

            // Set up constant links (markers, aka mapped links)
            this._meaningRoot = GetOrCreateMeaningRoot(CurrentMappingLinkIndex++);
            // Create converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            var numberToAddressConverter = new RawNumberToAddressConverter<uint>();
            var addressToNumberConverter = new AddressToRawNumberConverter<uint>();

            //Create converters that are able to convert string to unicode sequence stored as link and back
            var balancedVariantConverter = new BalancedVariantConverter<uint>(Links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<uint>(Links, unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<uint>(Links, unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<uint>(Links, addressToNumberConverter, unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<uint>(Links, numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<uint>(Links, new DefaultStack<uint>(), unicodeSymbolCriterionMatcher.IsMatched);
            this._stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, uint>(new StringToUnicodeSequenceConverter<uint>(Links, charToUnicodeSymbolConverter, balancedVariantConverter, unicodeSequenceMarker));
            this._unicodeSequenceToStringConverter = new CachingConverterDecorator<uint, string>(new UnicodeSequenceToStringConverter<uint>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private uint GetOrCreateMeaningRoot(uint meaningRootIndex) => Links.Exists(meaningRootIndex) ? meaningRootIndex : Links.CreatePoint();
        
        //TODO: Exception: "Object reference not set to an instance of an object."
        protected uint GetOrCreateNextMapping(uint currentMappingIndex) => Links.Exists(currentMappingIndex) ? currentMappingIndex : Links.CreateAndUpdate(_meaningRoot, Links.Constants.Itself);
        protected string ConvertToString(uint sequence) => _unicodeSequenceToStringConverter.Convert(sequence);
        protected uint ConvertToSequence(string @string) => _stringToUnicodeSequenceConverter.Convert(@string);
        protected override void Dispose(bool manual, bool wasDisposed)
        {
            if (!wasDisposed)
            {
                _disposableLinks.DisposeIfPossible();
            }
        }
    }
}
