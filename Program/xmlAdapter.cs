using System.Collections.Generic;
using Platform.IO;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Xml;
using Platform.Data.Doublets;
using Platform.Disposables;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Memory;
using Platform.Data;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Memory.Split.Specific;
using TLinkAddress = System.UInt32;

namespace database
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : DataBase
    {
        public TLinkAddress CreateLink(string xmlFileName, string innerXml)
        {
            var nameLink = ConvertToSequence(xmlFileName);
            var documentLink = ConvertToSequence(innerXml);
            return links.GetOrCreate(nameLink, documentLink);
        }

        public List<string> GetAllFileNames() { return default; }

        public (string, string) GetFile(string filename) { return default; }
        
        public XmlAdapter(string indexFileName, string dataFileName, string path) : base(indexFileName, dataFileName, path) { }
    }
}
