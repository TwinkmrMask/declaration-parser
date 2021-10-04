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
        private readonly IConverter<string, uint> _stringToUnicodeSequenceConverter;
        private readonly IConverter<uint, string> _unicodeSequenceToStringConverter;
        private readonly ILinks<uint> _disposableLinks;
        protected readonly ILinks<uint> Links;
        private const uint meaningRootIndex = 1;
        AddressToRawNumberConverter<uint> _addressToNumberConverter;

        protected Platform()
        {
            var dataMemory = new FileMappedResizableDirectMemory(IDefaultSettings.DataFileName);
            var indexMemory = new FileMappedResizableDirectMemory(IDefaultSettings.IndexFileName);
            var linksConstants = new LinksConstants<uint>(enableExternalReferencesSupport: true);

            _disposableLinks = new UInt32SplitMemoryLinks(dataMemory, indexMemory, UInt32SplitMemoryLinks.DefaultLinksSizeStep, linksConstants);

            Links = new UInt32Links(_disposableLinks);

            _addressToNumberConverter = new AddressToRawNumberConverter<uint>();

            const uint unicodeSymbolMarker = 2;
            const uint unicodeSequenceMarker = 3;

            var unicodeSymbolCriterionMatcher = new TargetMatcher<uint>(Links , unicodeSymbolMarker);

            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, uint>
                (new StringToUnicodeSequenceConverter<uint>(Links, 
                new CharToUnicodeSymbolConverter<uint>(Links, _addressToNumberConverter, unicodeSymbolMarker),
                new BalancedVariantConverter<uint>(Links), unicodeSequenceMarker));

            _unicodeSequenceToStringConverter = new CachingConverterDecorator<uint, string>(new UnicodeSequenceToStringConverter<uint>
                (Links, new TargetMatcher<uint>(Links, unicodeSequenceMarker), new RightSequenceWalker<uint>(Links, new DefaultStack<uint>(), 
                unicodeSymbolCriterionMatcher.IsMatched),
                new UnicodeSymbolToCharConverter<uint>(Links, new RawNumberToAddressConverter<uint>(), unicodeSymbolCriterionMatcher)));
        }
        protected uint GetOrCreateMarker(uint currentMappingIndex) => Links.GetOrCreate(meaningRootIndex, _addressToNumberConverter.Convert(currentMappingIndex));
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
