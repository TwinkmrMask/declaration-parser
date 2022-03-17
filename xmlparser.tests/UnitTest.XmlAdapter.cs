using Platform.Data.Doublets;
using System.Collections.Generic;
using XmlParser;
using System.Linq;
using Xunit;

#pragma warning disable IDE0051

namespace xmlparser.tests
{
    public class UnitTestXmlAdapter : XmlAdapter
    {
        Dictionary<string, string> testData = new()
        {
            ["filename1"] = "content1",
            ["filename3"] = "content2",
            ["filename5"] = "content3"
        };

        public UnitTestXmlAdapter() : base(IDefaultSettings.DataFileName, IDefaultSettings.IndexFileName) { }

        [Fact]
        void CreateLinkTest()
        {
            foreach (var item in testData)
            {
                CreateLink(item.Key, item.Value);
                Assert.True(Links.SearchOrDefault(ConvertToSequence(item.Key), ConvertToSequence(item.Value)) > 0);
            }

        }

        [Fact]
        void UnitTestContentGetAllFileNames()
        {
            Assert.NotNull(GetAllFileNames());

            var filenames = GetAllFileNames();
            foreach (var filename in filenames)
                Assert.True(testData.ContainsKey(filename));
            Assert.Equal(testData.Count, filenames.Count);
        }

        [Fact]
        void UnitTestIsLinks()
        {
            foreach (var item in testData)
            {
                Assert.True(IsLinks(item.Key));
                Assert.False(IsLinks(item.Value));
            }

        }
    }
}