using System.Collections.Generic;
using System.Windows.Documents;
using Platform.Data.Doublets;
using TLinkAddress = System.UInt32;

namespace database
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : Platform
    {
        private uint _xmlMarker;
        
        public TLinkAddress CreateLink(string xmlFileName, string innerXml)
        {
            var nameLink = ConvertToSequence(xmlFileName);
            var documentLink = ConvertToSequence(innerXml);
            return links.GetOrCreate( _xmlMarker, links.GetOrCreate(nameLink, documentLink));
        }

        public List<string> GetAllFileNames()
        {
            string name;
            List<string> names = new List<string>();
            var query = new Link<TLinkAddress>(this.links.Constants.Any, _xmlMarker, this.links.Constants.Any);
            
            if (!isLinks(query)) return default;
            
            this.links.Each((link) =>
            {
                var doublet = link[this.links.Constants.TargetPart];
                name = ConvertToString(this.links.GetSource(doublet));
                    names.Add(name);
                return this.links.Constants.Continue;
            }, query);
            return names;           
        }
        
        private bool isLinks(Link<TLinkAddress> query) => this.links.Count(query) != 0;

        public (string, string) GetFile(string filename) { return default; }
        
        public XmlAdapter(string indexFileName, string dataFileName, string path) : base(indexFileName, dataFileName, path)
        {
            _xmlMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
        }
    }
}