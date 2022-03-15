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
        private readonly ulong _meaningRoot;
        private readonly ulong _unicodeSymbolMarker;
        private readonly ulong _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<ulong> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<ulong> _addressToNumberConverter;
        private readonly CachingConverterDecorator<string, ulong> _stringToUnicodeSequenceConverter;
        private readonly CachingConverterDecorator<ulong, string> _unicodeSequenceToStringConverter;
        private readonly UInt64SplitMemoryLinks _disposableLinks;
        protected readonly UInt64Links Links;
        public Platform()
        {

            var dataMemory = new FileMappedResizableDirectMemory(IDefaultSettings.DataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(IDefaultSettings.IndexFileName);

            var linksConstants = new LinksConstants<ulong>(enableExternalReferencesSupport: true);

            // Init the links storage
            _disposableLinks = new(dataMemory, indexMemory, UInt64SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); // Low-level logic
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
            BalancedVariantConverter<ulong> balancedVariantConverter = new(Links);
            TargetMatcher<ulong> unicodeSymbolCriterionMatcher = new(Links, _unicodeSymbolMarker);
            TargetMatcher<ulong> unicodeSequenceCriterionMatcher = new(Links, _unicodeSequenceMarker);
            CharToUnicodeSymbolConverter<ulong> charToUnicodeSymbolConverter = new(Links, _addressToNumberConverter, _unicodeSymbolMarker);
            UnicodeSymbolToCharConverter<ulong> unicodeSymbolToCharConverter = new(Links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            RightSequenceWalker<ulong> sequenceWalker = new(Links, new DefaultStack<ulong>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new(new StringToUnicodeSequenceConverter<ulong>(Links, charToUnicodeSymbolConverter, balancedVariantConverter, _unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new(new UnicodeSequenceToStringConverter<ulong>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private ulong GetOrCreateMeaningRoot(ulong meaningRootIndex) => Links.Exists(meaningRootIndex) ? meaningRootIndex : Links.CreatePoint();
        private ulong GetOrCreateNextMapping(ulong currentMappingIndex) => Links.Exists(currentMappingIndex) ? currentMappingIndex : Links.CreateAndUpdate(_meaningRoot, Links.Constants.Itself);
        public string ConvertToString(ulong sequence) => _unicodeSequenceToStringConverter.Convert(sequence);
        public ulong ConvertToSequence(string @string) => _stringToUnicodeSequenceConverter.Convert(@string);
        public void Delete(ulong link) => Links.Delete(link);
        protected override void Dispose(bool manual, bool wasDisposed)
        {
            if (!wasDisposed) _disposableLinks.DisposeIfPossible();
        }

    }
}