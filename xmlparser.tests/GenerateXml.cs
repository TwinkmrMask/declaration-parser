
using System.IO;
using XmlParser;

namespace xmlparser.tests
{
    internal class GenerateXml
    {
        public void Generate(int num) 
        {
            
            using FileStream fileStream = new(IDefaultSettings.DefaultPath +  $"test {num}.xml", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine("<test/>");
            writer.Close();
        }
    }
}
