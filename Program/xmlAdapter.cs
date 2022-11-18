using System.Collections.Generic;
using Platform.Data.Doublets;

#pragma warning disable 649

namespace XmlParser
{
    public class XmlAdapter : Platform
    {
        private static ulong _fileNameMarker;
        
        public XmlAdapter(string dataFileName, string indexFileName) : base(dataFileName, indexFileName) =>
            _fileNameMarker = Links.GetOrCreate(ConvertToSequence(nameof(_fileNameMarker)), ConvertToSequence(nameof(_fileNameMarker)));
        private Link<ulong> Query(ulong marker) => new(this.Links.Constants.Any, marker, this.Links.Constants.Any);
        private bool IsLinks(Link<ulong> query) => this.Links.Count(query) > 0;
        public void CreateLink(string filename, in string innerXml)
        {
            Links.GetOrCreate(_fileNameMarker, ConvertToSequence(filename));
            Links.GetOrCreate(ConvertToSequence(filename), ConvertToSequence(innerXml));
        }
        public List<string> GetAllFileNames()
        {
            var names = new List<string>();
            var query = Query(_fileNameMarker);
            if (!IsLinks(query)) { return default; }
            Links.Each((link) =>
            {
                var item = ConvertToString(Links.GetTarget(link));
                names.Add(item);
                return this.Links.Constants.Continue;
                 
            }, query);
            return names;
        }
        public string GetContent(string filename) => ConvertToString(Links.GetTarget(Links.SearchOrDefault(ConvertToSequence(filename), Links.Constants.Any)));
        public bool IsLinks(string element) => this.Links.Count(Query(ConvertToSequence(element))) > 0;
        
    }
}