using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : XmlParser.Platform
    {
        public void Delete(uint link) => Links.Delete(link);
        public void CreateTransportCodeLink(string transportDocumentCode)
        {
            var link = ConvertToSequence(transportDocumentCode);
            this.Links.GetOrCreate(this.Links.Constants.Itself, link);
        }
        public bool TransportCodeEach(string transportDocumentCode) => 
            this.Links.SearchOrDefault(this.Links.Constants.Itself, ConvertToSequence(transportDocumentCode)) != 0;
    }
}
