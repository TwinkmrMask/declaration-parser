using Xunit;
using XmlParser;
using System.Collections.Generic;

namespace XmlParser.Tests
{
    public class UnitTestFileSearcher : TryMethods.TryMethods
    {
        [Fact]
        public void GetFilesTest() 
        {
            List<string> testFiles = new();

            GenerateXml generateXml = new();
            GenerateXml.Generate(1);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 1.xml");
            GenerateXml.Generate(2);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 2.xml");
            GenerateXml.Generate(3);
            testFiles.Add(IDefaultSettings.DefaultPath + $"test 3.xml");

            var files = GetFiles(IDefaultSettings.DefaultPath, "*.xml");

            Assert.Equal(files, testFiles);
        }
    }
}
