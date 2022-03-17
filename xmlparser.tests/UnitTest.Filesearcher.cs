using Xunit;
using XmlParser;
using System.Collections.Generic;

namespace xmlparser.tests
{
    public class UnitTestFileSearcher : FileSearcher
    {
        [Fact]
        public void GetFilesTest() 
        {
            List<string> testFiles = new();

            GenerateXml generateXml = new();
            generateXml.Generate(1);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 1.xml");
            generateXml.Generate(2);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 2.xml");
            generateXml.Generate(3);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 3.xml");

            var files = GetFiles(IDefaultSettings.DefaultPath, "*.xml");

            Assert.Equal(files, testFiles);
        }
    }
}
