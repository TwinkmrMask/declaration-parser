
using System.IO;
using XmlParser;

namespace XmlParser.Tests
{
    internal class GenerateXml
    {
        public static void Generate(int num) 
        {
            using FileStream fileStream = new(IDefaultSettings.DefaultPath +  $"test {num}.xml", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new (fileStream);
            writer.WriteLine("<test/>");
            writer.Close();
        }
    }
}
