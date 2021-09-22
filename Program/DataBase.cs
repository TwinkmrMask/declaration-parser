using System.Text;
using System.Windows;
using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform
    {
        private readonly uint _codeMarker;

        public DataBase()
        { 
            this._codeMarker = GetOrCreateNextMapping(CurrentMappingLinkIndex++);   
        }
        
        public void Delete(uint link) => Links.Delete(link);
        public void CreateTransportCodeLink(string transportDocumentCode)
        {
            var link = ConvertToSequence(transportDocumentCode);
            this.Links.GetOrCreate(_codeMarker, link);
        }
        
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) > 0;

        public bool TransportCodeEach(string transportDocumentCode) =>
            this.Links.SearchOrDefault(_codeMarker, ConvertToSequence(transportDocumentCode)) != 0;
    }
}
