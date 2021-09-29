using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data;
using Platform.Data.Doublets;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Memory.Split.Specific;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Numbers.Raw;
using Platform.Disposables;
using Platform.Memory;

namespace XmlParser
{
    public class Platform : DisposableBase
    {
        private readonly uint _meaningRoot;
        private readonly IConverter<string, uint> _stringToUnicodeSequenceConverter;
        private readonly IConverter<uint, string> _unicodeSequenceToStringConverter;
        private readonly ILinks<uint> _disposableLinks;
        protected readonly ILinks<uint> Links;
        protected static uint _currentMappingLinkIndex = 1;
        
        protected Platform()
        {
            var dataMemory = new FileMappedResizableDirectMemory(IDefaultSettings.DataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(IDefaultSettings.IndexFileName);
            var linksConstants = new LinksConstants<uint>(enableExternalReferencesSupport: true);
            _disposableLinks = new UInt32SplitMemoryLinks(dataMemory, indexMemory, UInt32SplitMemoryLinks.DefaultLinksSizeStep, linksConstants); 
            Links = new UInt32Links(_disposableLinks); 
            _meaningRoot = GetOrCreateMeaningRoot(_currentMappingLinkIndex++);
            var unicodeSymbolMarker = GetOrCreateNextMapping(_currentMappingLinkIndex++);
            var unicodeSequenceMarker = GetOrCreateNextMapping(_currentMappingLinkIndex++);
            var numberToAddressConverter = new RawNumberToAddressConverter<uint>();
            var addressToNumberConverter = new AddressToRawNumberConverter<uint>();
            var balancedVariantConverter = new BalancedVariantConverter<uint>(Links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<uint>(Links, unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<uint>(Links, unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<uint>(Links, addressToNumberConverter, unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<uint>(Links, numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<uint>(Links, new DefaultStack<uint>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, uint>(new StringToUnicodeSequenceConverter<uint>(Links, charToUnicodeSymbolConverter, balancedVariantConverter, unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new CachingConverterDecorator<uint, string>(new UnicodeSequenceToStringConverter<uint>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
        }
        private uint GetOrCreateMeaningRoot(uint meaningRootIndex) => Links.Exists(meaningRootIndex) ? meaningRootIndex : Links.CreatePoint();
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
