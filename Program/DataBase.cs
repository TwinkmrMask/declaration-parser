using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform, IDefaultSettings
    {
        private const uint CodeMarker = 21;
        public void CreateTransportCodeLink(string transportDocumentCode) => Links.GetOrCreate(CodeMarker, ConvertToSequence(transportDocumentCode));
        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(CodeMarker, ConvertToSequence(transportDocumentCode)) != 0;

        public DataBase() => GetOrCreateMarker(CodeMarker);
    }
}
