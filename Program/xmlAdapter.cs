﻿using System.Collections.Generic;
using Platform.Data.Doublets;

#pragma warning disable 649

namespace XmlParser
{
    public class XmlAdapter : Platform
    {
        private readonly uint FileNameMarker;

        private Link<uint> Query(uint marker) => new(this.Links.Constants.Any, marker, this.Links.Constants.Any);
        public void CreateLink(in string innerXml, string filename)
        {
            var nameLink = ConvertToSequence(filename);
            var documentLink = ConvertToSequence(innerXml);
            Links.GetOrCreate(FileNameMarker, nameLink);
            Links.GetOrCreate(nameLink, documentLink);
        }

        public List<string> GetAllFileNames()
        {
            var names = new List<string>();
            var query = Query(FileNameMarker);
            if (!IsLinks(query)) { return default; }

            Links.Each((link) =>
            {
                var item = ConvertToString(Links.GetTarget(link));
                names.Add(item);
                return this.Links.Constants.Continue;
            }, query);
            return names;
        }
        public string GetContent(string filename)
        {
            var query = Query(ConvertToSequence(filename));
            return ConvertToString(Links.GetTarget(query));
        }
        private bool IsLinks(Link<uint> query) => this.Links.Count(query) > 0;

        public XmlAdapter() => FileNameMarker = Links.GetOrCreate(ConvertToSequence("FileNameMarker"), ConvertToSequence("FileNameMarker"));
    }
}
 