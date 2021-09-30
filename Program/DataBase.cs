using Platform.Data.Doublets;

namespace XmlParser
{
    public class DataBase : Platform, IDefaultSettings
    {
        private static uint _codeMarker;
        public void CreateTransportCodeLink(string transportDocumentCode) => Links.GetOrCreate(_codeMarker, ConvertToSequence(transportDocumentCode));
        public bool TransportCodeEach(string transportDocumentCode) => this.Links.SearchOrDefault(_codeMarker, ConvertToSequence(transportDocumentCode)) != 0;
        public void InitialMarker() => _codeMarker = GetOrCreateMarker(IDefaultSettings._currentMappingLinkIndex++);
    }
}
