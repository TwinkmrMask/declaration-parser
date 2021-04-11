using Platform.IO;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Xml;
using Platform.Data.Doublets;

namespace database
{
    //Adapter pattern
    //Turns xml into links and immediately saves it to the links file
    public class XmlAdapter : DefaultSettings
    {
        private string xmlFileName;
        public XmlAdapter(string xmlFileName)
        {
            this.xmlFileName = xmlFileName;
        }
        public void Run(params string[] args)
        {
            var linksFile = ConsoleHelpers.GetOrReadArgument(0, indexFileName, args);
            var file = ConsoleHelpers.GetOrReadArgument(1, xmlFileName, args);

            using (var cancellation = new ConsoleCancellation())
            using (var memoryAdapter = new UnitedMemoryLinks<uint>(linksFile))
            {
                var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
                var indexer = new XmlIndexer<uint>(links);
                var indexingImporter = new XmlImporter<uint>(indexer);
                indexingImporter.Import(file, cancellation.Token).Wait();
                if (cancellation.NotRequested)
                {
                    var cache = indexer.Cache;
                    var storage = new DefaultXmlStorage<uint>(links, false, cache);
                    var importer = new XmlImporter<uint>(storage);
                    importer.Import(file, cancellation.Token).Wait();
                }
            }
        }
    }
}
