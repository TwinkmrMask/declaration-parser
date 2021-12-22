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
namespace XmlParser
{
    public class Platform : DisposableBase
    {
        protected static string indexFileName;
        protected static string dataFileName;
        private readonly uint _meaningRoot;
        private readonly uint _unicodeSymbolMarker;
        private readonly uint _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<uint> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<uint> _addressToNumberConverter;
        private readonly CachingConverterDecorator<string, uint> _stringToUnicodeSequenceConverter;
        private readonly CachingConverterDecorator<uint, string> _unicodeSequenceToStringConverter;
        private readonly UInt32SplitMemoryLinks _disposableLinks;
        protected readonly UInt32Links Links;
        public Platform()
        {

            var dataMemory = new FileMappedResizableDirectMemory(IDefaultSettings.DataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(IDefaultSettings.IndexFileName);

            var linksConstants = new LinksConstants<uint>(enableExternalReferencesSupport: true);

            // Init the links storage
            _disposableLinks = new(dataMemory, indexMemory, UInt32SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); // Low-level logic
            Links = new(_disposableLinks); // Main logic in the combined decorator

            // Set up constant links (markers, aka mapped links)
            uint currentMappingLinkIndex = 1;
            _meaningRoot = GetOrCreateMeaningRoot(currentMappingLinkIndex++);
            _unicodeSymbolMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
            _unicodeSequenceMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
            // Create converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            _numberToAddressConverter = new();
            _addressToNumberConverter = new();

            // Create converters that are able to convert string to unicode sequence stored as link and back
            BalancedVariantConverter<uint> balancedVariantConverter = new(Links);
            TargetMatcher<uint> unicodeSymbolCriterionMatcher = new(Links, _unicodeSymbolMarker);
            TargetMatcher<uint> unicodeSequenceCriterionMatcher = new(Links, _unicodeSequenceMarker);
            CharToUnicodeSymbolConverter<uint> charToUnicodeSymbolConverter = new(Links, _addressToNumberConverter, _unicodeSymbolMarker);
            UnicodeSymbolToCharConverter<uint> unicodeSymbolToCharConverter = new(Links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            RightSequenceWalker<uint> sequenceWalker = new(Links, new DefaultStack<uint>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new(new StringToUnicodeSequenceConverter<uint>(Links, charToUnicodeSymbolConverter, balancedVariantConverter, _unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new(new UnicodeSequenceToStringConverter<uint>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private uint GetOrCreateMeaningRoot(uint meaningRootIndex) => Links.Exists(meaningRootIndex) ? meaningRootIndex : Links.CreatePoint();
        private uint GetOrCreateNextMapping(uint currentMappingIndex) => Links.Exists(currentMappingIndex) ? currentMappingIndex : Links.CreateAndUpdate(_meaningRoot, Links.Constants.Itself);
        public string ConvertToString(uint sequence) => _unicodeSequenceToStringConverter.Convert(sequence);
        public uint ConvertToSequence(string @string) => _stringToUnicodeSequenceConverter.Convert(@string);
        public void Delete(uint link) => Links.Delete(link);
        protected override void Dispose(bool manual, bool wasDisposed)
        {
            if (!wasDisposed) _disposableLinks.DisposeIfPossible();
        }
        
    }
}