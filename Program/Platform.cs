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
        private readonly ulong _meaningRoot;
        private readonly CachingConverterDecorator<string, ulong> _stringToUnicodeSequenceConverter;
        private readonly CachingConverterDecorator<ulong, string> _unicodeSequenceToStringConverter;
        private readonly UInt64SplitMemoryLinks _disposableLinks;
        protected readonly UInt64Links Links;

        protected Platform(string dataFileName, string indexFileName)
        {
            var dataMemory = new FileMappedResizableDirectMemory(dataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(indexFileName);

            var linksConstants = new LinksConstants<ulong>(enableExternalReferencesSupport: true);

            // Init the links storage
            _disposableLinks = new(dataMemory, indexMemory, UInt64SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); // Low-level logic
            Links = new(_disposableLinks); // Main logic in the combined decorator

            // Set up constant links (markers, aka mapped links)
            uint currentMappingLinkIndex = 1;
            _meaningRoot = GetOrCreateMeaningRoot(currentMappingLinkIndex++);
            var unicodeSymbolMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
            var unicodeSequenceMarker = GetOrCreateNextMapping(currentMappingLinkIndex);
            // Create converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            RawNumberToAddressConverter<ulong> numberToAddressConverter = new();
            AddressToRawNumberConverter<ulong> addressToNumberConverter = new();

            // Create converters that are able to convert string to unicode sequence stored as link and back
            BalancedVariantConverter<ulong> balancedVariantConverter = new(Links);
            TargetMatcher<ulong> unicodeSymbolCriterionMatcher = new(Links, unicodeSymbolMarker);
            TargetMatcher<ulong> unicodeSequenceCriterionMatcher = new(Links, unicodeSequenceMarker);
            CharToUnicodeSymbolConverter<ulong> charToUnicodeSymbolConverter = new(Links, addressToNumberConverter, unicodeSymbolMarker);
            UnicodeSymbolToCharConverter<ulong> unicodeSymbolToCharConverter = new(Links, numberToAddressConverter, unicodeSymbolCriterionMatcher);
            RightSequenceWalker<ulong> sequenceWalker = new(Links, new DefaultStack<ulong>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new(new StringToUnicodeSequenceConverter<ulong>(Links, charToUnicodeSymbolConverter, balancedVariantConverter, unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new(new UnicodeSequenceToStringConverter<ulong>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private ulong GetOrCreateMeaningRoot(ulong meaningRootIndex) => Links.Exists(meaningRootIndex) ? meaningRootIndex : Links.CreatePoint();
        private ulong GetOrCreateNextMapping(ulong currentMappingIndex) => Links.Exists(currentMappingIndex) ? currentMappingIndex : Links.CreateAndUpdate(_meaningRoot, Links.Constants.Itself);
        protected string ConvertToString(ulong sequence) => _unicodeSequenceToStringConverter.Convert(sequence);
        protected ulong ConvertToSequence(string @string) => _stringToUnicodeSequenceConverter.Convert(@string);
        public void Delete(ulong link) => Links.Delete(link);
        protected override void Dispose(bool manual, bool wasDisposed) { if (!wasDisposed) _disposableLinks.DisposeIfPossible(); }

    }
}