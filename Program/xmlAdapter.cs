using System.Collections.Generic;
using System.Linq;
using Platform.Data.Doublets;

namespace database
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : database.Platform
    {
        private readonly uint _xmlMarker;
        
        public void CreateLink(string xmlFileName, string innerXml)
        {
            var nameLink = ConvertToSequence(xmlFileName);
            var documentLink = ConvertToSequence(innerXml);
            Links.GetOrCreate( _xmlMarker, Links.GetOrCreate(nameLink, documentLink));
        }

        public List<string> GetAllFileNames()
        {
            string name;
            var names = new List<string>();
            var query = new Link<uint>(this.Links.Constants.Any, _xmlMarker, this.Links.Constants.Any);
            
            if (!IsLinks(query)) return default;
            
            this.Links.Each((link) =>
            {
                var doublet = link[this.Links.Constants.TargetPart];
                name = ConvertToString(this.Links.GetSource(doublet));
                    names.Add(name);
                return this.Links.Constants.Continue;
            }, query);
            return names;           
        }
        
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) != 0;

        public (string, string) GetFile(string filename)
        {
            var foundLinks = Links.All(Links.GetSource(ConvertToSequence(filename)), Links.Constants.Any);

            var results = (from foundLink in foundLinks let linkIndex = 
                Links.SearchOrDefault(_xmlMarker, Links.GetIndex(foundLink)) 
                where linkIndex != default select Links.GetTarget(foundLink)).ToList();
            return (filename, ConvertToString(results[0]));
        }
        
        public (string, List<string>) GetFile(string filename, bool flag)
        {
            if (flag)
            {
                var foundLinks = Links.All(Links.GetSource(ConvertToSequence(filename)), Links.Constants.Any);

                var results = (from foundLink in foundLinks let linkIndex = 
                    Links.SearchOrDefault(_xmlMarker, 
                        Links.GetIndex(foundLink)) where linkIndex != 
                                                         default select ConvertToString(Links.GetTarget(foundLink))).ToList();

                return (filename, results);
            }
            else return (GetFile(filename).Item1, new List<string>(){ GetFile(filename).Item2 });
            
        }
        
        public XmlAdapter()
        {
            _xmlMarker = GetOrCreateNextMapping(CurrentMappingLinkIndex++);
        }
    }
}