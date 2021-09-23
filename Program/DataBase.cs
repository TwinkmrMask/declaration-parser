using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform
    {
        private readonly uint _codeMarker;

        public DataBase() => _codeMarker= NewMarker();

        //public void Delete(uint link) => Links.Delete(link);
        public void CreateTransportCodeLink(string transportDocumentCode)
        {
            var link = ConvertToSequence(transportDocumentCode);
            Links.GetOrCreate(_codeMarker, link);
        }
        
        //private bool IsLinks(Link<uint> query) => this.Links.Count(query) > 0;

        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(_codeMarker, ConvertToSequence(transportDocumentCode)) != 0;
    }
}
