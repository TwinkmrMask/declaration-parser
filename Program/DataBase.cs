using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform, IDefaultSettings
    {
        private const uint _codeMarker = 21;
        public void CreateTransportCodeLink(string transportDocumentCode) => Links.GetOrCreate(_codeMarker, ConvertToSequence(transportDocumentCode));
        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(_codeMarker, ConvertToSequence(transportDocumentCode)) != 0;

        public DataBase() => GetOrCreateMarker(_codeMarker);
    }
}
