
using HtmlAgilityPack;
using System.Net;

namespace UserData_webapi
{
    public class ImageRepository : IImageRepository
    {
        public HtmlNodeCollection crawler()
        {
            HtmlWeb webClient = new HtmlWeb(); //建立htmlweb
            HtmlDocument doc = webClient.Load("https://www.facebook.com/Frc_8723-110764354730115/photos/"); //載入網址資料
            HtmlNodeCollection list = doc.DocumentNode.SelectNodes("//h2[.//a[contains(@href,\"/photos\")]]//following::div//img"); //抓取Xpath資料
            return list;
        }
    }
}