using Platform.Data.Doublets;
using TLinkAddress = System.UInt32;
namespace database
{
    public class DataBase : Platform
    {
        public DataBase(string indexFileName, string dataFileName, string path) : base(indexFileName, dataFileName, path)
        {
            
        }
        
        public void Delete(TLinkAddress link) => links.Delete(link);
        public void CreateTransportCodeLink(string transportDocumentCode)
        {
            var Link = ConvertToSequence(transportDocumentCode);
            this.links.GetOrCreate(this.links.Constants.Itself, Link);
        }
        public bool TransportCodeEach(string transportDocumentCode) => 
            this.links.SearchOrDefault(this.links.Constants.Itself, ConvertToSequence(transportDocumentCode)) != 0;
    }
}