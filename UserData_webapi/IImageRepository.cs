using HtmlAgilityPack;

namespace UserData_webapi
{
    public interface IImageRepository
    {
        public Task<List<string>> crawler();
        public Task<string> geticonurl();
        public Task<List<string>> getteamimg();
        public List<string> frcview(string language);
        public Task<string> teamview(string language);

    }
}
