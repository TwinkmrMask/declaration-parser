using Platform.Data.Doublets;
using System.Collections.Generic;
using Xunit;

#pragma warning disable IDE0051

namespace XmlParser.Tests
{
    public class UnitTestXmlAdapter : XmlAdapter
    {
        private readonly Dictionary<string, string> _testData = new()
        {
            ["filename1"] = "content1",
            ["filename3"] = "content2",
            ["filename5"] = "content3"
        };

        public UnitTestXmlAdapter() : base(IDefaultSettings.DataFileName, IDefaultSettings.IndexFileName) { }

        [Fact]
        private void CreateLinkTest()
        {
            foreach (var item in _testData)
            {
                CreateLink(item.Key, item.Value);
                Assert.True(Links.SearchOrDefault(ConvertToSequence(item.Key), ConvertToSequence(item.Value)) > 0);
            }
        }

        [Fact]
        private void UnitTestContentGetAllFileNames()
        {
            Assert.NotNull(GetAllFileNames());

            var filenames = GetAllFileNames();
            foreach (var filename in filenames)
                Assert.True(_testData.ContainsKey(filename));
            Assert.Equal(_testData.Count, filenames.Count);
        }

        [Fact]
        private void UnitTestIsLinks()
        {
            foreach (var item in _testData)
            {
                Assert.True(IsLinks(item.Key));
                Assert.False(IsLinks(item.Value));
            }
        }
    }
}