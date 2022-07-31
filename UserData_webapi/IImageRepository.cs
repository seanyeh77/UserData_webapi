using HtmlAgilityPack;

namespace UserData_webapi
{
    public interface IImageRepository
    {
        public HtmlNodeCollection crawler();
    }
}
