using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform
    {
        private readonly uint _codeMarker;
        public void CreateTransportCodeLink(string transportDocumentCode) => Links.GetOrCreate(_codeMarker, ConvertToSequence(transportDocumentCode));
        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(_codeMarker, ConvertToSequence(transportDocumentCode)) != 0;
        public DataBase() => _codeMarker = Links.GetOrCreate(ConvertToSequence("CodeMarker"), ConvertToSequence("CodeMarker"));
    }
}
