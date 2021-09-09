using Platform.Data.Doublets;
using TLinkAddress = System.UInt32;
namespace DataBase
{
    public class DataBase : Platform
    {
        public void Delete(uint link) => links.Delete(link);
        public void CreateTransportCodeLink(string transportDocumentCode)
        {
            var link = ConvertToSequence(transportDocumentCode);
            this.links.GetOrCreate(this.links.Constants.Itself, link);
        }
        public bool TransportCodeEach(string transportDocumentCode) => 
            this.links.SearchOrDefault(this.links.Constants.Itself, ConvertToSequence(transportDocumentCode)) != 0;
    }
}