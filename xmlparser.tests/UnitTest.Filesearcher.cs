using Xunit;
using XmlParser;
using System.Collections.Generic;

namespace xmlparser.tests
{
    public class UnitTestFilesearcher : FileSearcher
    {
        [Fact]
        public void Test() 
        {
            List<string> testFiles = new();

            GenerateXml generateXml = new();
            generateXml.Generate(1);
            testFiles.Add(IDefaultSettingsTest.DefaultXmlFileName);
            generateXml.Generate(2);
            testFiles.Add(IDefaultSettingsTest.DefaultXmlFileName);
            generateXml.Generate(3);
            testFiles.Add(IDefaultSettingsTest.DefaultXmlFileName);

            var files = GetFiles(IDefaultSettingsTest.DefaultPath, "*.xml");

            Assert.Equal(files, testFiles);
        }
    }
}
