using System.Collections.Generic;
using System.Linq;
using Platform.Data.Doublets;

namespace XmlParser
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : Platform
    {
        private readonly uint _xmlMarker;
        private readonly string _fileName; 
        
        public void CreateLink(in string innerXml)
        {
            var nameLink = ConvertToSequence(this._fileName);
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
        
        private (string, List<string>, int) GetContentIndexes()
        {
            var foundLinks = Links.All(Links.GetSource(ConvertToSequence(this._fileName)), Links.Constants.Any);

            var results = 
                (from foundLink in foundLinks let linkIndex = 
                    Links.SearchOrDefault(_xmlMarker, Links.GetIndex(foundLink)) where linkIndex != 
                    default select ConvertToString(Links.GetTarget(foundLink))).ToList();
            
            return (this._fileName, results, results.Count);
        }

        public (string, string) GetContent()
        {
            var (item1, item2, item3) = GetContentIndexes();
            return item3 == 1 ? (item1, item2[0]) : default;
        }
        
        public (string, List<string>) GetContents()
        {
            var (item1, item2, item3) = GetContentIndexes();
            return item3 == 1 ? (item1, item2) : default;
        }
        
        public XmlAdapter(string fileName)
        {
            this._fileName = fileName;
            _xmlMarker = GetOrCreateNextMapping(CurrentMappingLinkIndex++);
        }
                
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) != 0;
    }
}
