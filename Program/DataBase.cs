using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform
    {
        private uint CodeMarker;
        public void CreateTransportCodeLink(string transportDocumentCode) => Links.GetOrCreate(CodeMarker, ConvertToSequence(transportDocumentCode));
        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(CodeMarker, ConvertToSequence(transportDocumentCode)) != 0;
        public DataBase() => CodeMarker = Links.GetOrCreate(ConvertToSequence("CodeMarker"), ConvertToSequence("CodeMarker"));
    }
}
