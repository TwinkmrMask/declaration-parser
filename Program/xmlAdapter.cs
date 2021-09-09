using System.Collections.Generic;
using System.Linq;
using Platform.Data.Doublets;

namespace DataBase
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : Platform
    {
        private readonly uint _xmlMarker;
        
        public void CreateLink(string xmlFileName, string innerXml)
        {
            var nameLink = ConvertToSequence(xmlFileName);
            var documentLink = ConvertToSequence(innerXml);
            links.GetOrCreate( _xmlMarker, links.GetOrCreate(nameLink, documentLink));
        }

        public List<string> GetAllFileNames()
        {
            string name;
            var names = new List<string>();
            var query = new Link<uint>(this.links.Constants.Any, _xmlMarker, this.links.Constants.Any);
            
            if (!IsLinks(query)) return default;
            
            this.links.Each((link) =>
            {
                var doublet = link[this.links.Constants.TargetPart];
                name = ConvertToString(this.links.GetSource(doublet));
                    names.Add(name);
                return this.links.Constants.Continue;
            }, query);
            return names;           
        }

        private bool IsLinks(Link<uint> query) => this.links.Count(query) != 0;

        public (string, string) GetFile(string filename)
        {
            var foundLinks = links.All(links.GetSource(ConvertToSequence(filename)), links.Constants.Any);

            var results = (from foundLink in foundLinks let linkIndex = 
                links.SearchOrDefault(_xmlMarker, links.GetIndex(foundLink)) 
                where linkIndex != default select links.GetTarget(foundLink)).ToList();
            return (filename, ConvertToString(results[0]));
        }
        
        public (string, List<string>) GetFile(string filename, bool flag)
        {
            if (flag)
            {
                var foundLinks = links.All(links.GetSource(ConvertToSequence(filename)), links.Constants.Any);

                var results = (from foundLink in foundLinks let linkIndex = 
                    links.SearchOrDefault(_xmlMarker, 
                        links.GetIndex(foundLink)) where linkIndex != 
                                                         default select ConvertToString(links.GetTarget(foundLink))).ToList();

                return (filename, results);
            }
            else return (GetFile(filename).Item1, new List<string>(){ GetFile(filename).Item2 });
            
        }
        
        public XmlAdapter()
        {
            _xmlMarker = GetOrCreateNextMapping(currentMappingLinkIndex++);
        }
    }
}