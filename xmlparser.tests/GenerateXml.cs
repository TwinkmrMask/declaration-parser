
using System.IO;

namespace xmlparser.tests
{
    internal class GenerateXml
    {
        public void Generate(int num) 
        {
            IDefaultSettingsTest.DefaultXmlFileName = num.ToString();
            using FileStream fileStream = new(IDefaultSettingsTest.DefaultPath + IDefaultSettingsTest.DefaultXmlFileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine("<test/>");
        }
    }
}
