using System.Collections.Generic;
using Platform.Data.Doublets;

#pragma warning disable 649

namespace XmlParser
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : Platform
    {
        private readonly string _fileName;

        private static uint _fileNameMarker;

        private Link<uint> Query(uint marker) => new(this.Links.Constants.Any, marker, this.Links.Constants.Any);
        //private Link<uint> Query(uint marker, string data) => new(this.Links.Constants.Any, marker, ConvertToSequence(data));
            
        public void CreateLink(in string innerXml)
        {
            var nameLink = ConvertToSequence(this._fileName);
            var documentLink = ConvertToSequence(innerXml);
            Links.GetOrCreate(_fileNameMarker, nameLink);
            Links.GetOrCreate(nameLink, documentLink);
        }
        
        public List<string> GetAllFileNames()
        {
            var names = new List<string>();
            var query = Query(_fileNameMarker);
            if (!IsLinks(query)) return default;
            
            Links.Each((link) =>
            {
                var item = ConvertToString( Links.GetTarget(link));
                names.Add(item);
                return this.Links.Constants.Continue;
            }, query);
            return names;           
        }
        
        /*
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
*/
        public XmlAdapter(string fileName) : base(out _fileNameMarker) => this._fileName = fileName;
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) > 0;
    }
}
