using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Platform.Data.Doublets;

#pragma warning disable 649

namespace XmlParser
{
    public class XmlAdapter : Platform, IDefaultSettings
    {
        private static uint _fileNameMarker;

        private Link<uint> Query(uint marker) => new(this.Links.Constants.Any, marker, this.Links.Constants.Any);
        public void CreateLink(in string innerXml, string filename)
        {
            var nameLink = ConvertToSequence(filename);
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
                var item = ConvertToString(link[this.Links.Constants.TargetPart]);
                names.Add(item);
                return this.Links.Constants.Continue;
            }, query);
            return names;
        }
        public void InitialMarker() => _fileNameMarker = GetOrCreateNextMapping(_currentMappingLinkIndex++);
        public string GetContent(string filename) => ConvertToString(Links.GetSource(Links.SearchOrDefault(ConvertToSequence(filename), this.Links.Constants.Any)));
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) > 0;
    }
}
 