using HtmlAgilityPack;
using System.Net;
using System.Security.Policy;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;

namespace UserData_webapi
{
    public class ImageRepository : IImageRepository
    {
        private readonly IConfiguration _configurgation;
        public ImageRepository(IConfiguration configurgation)
        {
            _configurgation = configurgation;
        }

        public async Task<List<string>> crawler()
        {
            List<string> imagesurls = new List<string>();
            string str1 = "";
            facebookimageclass? result = new facebookimageclass();
            Photos result1 = new Photos();
            string access_token = _configurgation.GetSection("facebook:access_token").Value;
            string url = $"{_configurgation.GetSection("facebook:urlphotoimage").Value}{access_token}";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            try
            {
                result = await client.GetFromJsonAsync<facebookimageclass>("");
            }
            catch
            {
                result = new facebookimageclass();
            }
            foreach (var imagesurl in result.photos.data)
            {
                imagesurls.Add(imagesurl.images[0].source);
            }
            url = result.photos.paging.next;
            while (!string.IsNullOrEmpty(url))
            {

                client = new HttpClient() { BaseAddress = new Uri(url) };
                try
                {
                    result1 = await client.GetFromJsonAsync<Photos>("");
                }
                catch
                {
                    result1 = new Photos();
                }
                foreach (var imagesurl in result1.data)
                {
                    imagesurls.Add(imagesurl.images[0].source);
                }
                url = result1.paging.next;
            }
            return imagesurls.Distinct().ToList<string>();
        }
        public async Task<string> geticonurl()
        {
            iconurl? result = new iconurl();
            string access_token = _configurgation.GetSection("facebook:access_token").Value;
            string url = $"{_configurgation.GetSection("facebook:urlimageBurl").Value}{access_token}";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            try
            {
                result = await client.GetFromJsonAsync<iconurl>("");
            }
            catch
            {
                result = new iconurl();
            }
            return result.picture.data.url;
        }

        public async Task<List<string>> getteamimg()
        {
            List<string> imagesurls = new List<string>();
            string str1 = "";
            facebookimageclass? result = new facebookimageclass();
            Photos result1 = new Photos();
            string access_token = _configurgation.GetSection("facebook:access_token").Value;
            string url = $"{_configurgation.GetSection("facebook:urlphotosBimages").Value}{access_token}";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            try
            {
                result = await client.GetFromJsonAsync<facebookimageclass>("");
            }
            catch
            {
                result = new facebookimageclass();
            }
            foreach (var imagesurl in result.photos.data)
            {
                imagesurls.Add(imagesurl.images[0].source);
            }
            url = result.photos.paging.next;
            while (!string.IsNullOrEmpty(url))
            {

                client = new HttpClient() { BaseAddress = new Uri(url) };
                try
                {
                    result1 = await client.GetFromJsonAsync<Photos>("");
                }
                catch
                {
                    result1 = new Photos();
                }
                foreach (var imagesurl in result1.data)
                {
                    imagesurls.Add(imagesurl.images[0].source);
                }
                url = result1.paging.next;
            }
            return imagesurls.Distinct().ToList<string>();
        }
        public List<string> frcview(string language)
        {
            List<string> strings = new List<string>();
            HtmlWeb webClient = new HtmlWeb(); //建立htmlweb
            HtmlDocument doc = new HtmlDocument();
            if (language == "chinese")
            {
                doc = webClient.Load(_configurgation.GetSection("wikipedia:chinesse").Value); //載入網址資料
            }
            else if (language == "english")
            {
                doc = webClient.Load(_configurgation.GetSection("wikipedia:english").Value); //載入網址資料
            }
            HtmlNodeCollection data = doc.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div[1]/p"); //抓取Xpath資料
            foreach (HtmlNode node in data)
            {
                StringWriter myWriter = new StringWriter();
                string text = node.InnerText;
                HttpUtility.HtmlDecode(text, myWriter);
                text = myWriter.ToString();
                text = Regex.Replace(text, "\n", string.Empty);
                text = Regex.Replace(text, "\\[\\d\\]", string.Empty);
                text = Regex.Replace(text, "\\[\\d\\d\\]", string.Empty);
                text = Regex.Replace(text, "\\[\\d\\d\\d\\]", string.Empty);
                strings.Add("　　"+text);
            }
            strings.RemoveAll(s => string.IsNullOrEmpty(s));
            return strings;
        }
        public async Task<string> teamview(string language)
        {
            List<string> imagesurls = new List<string>();
            teamabout? result = new teamabout();
            string access_token = _configurgation.GetSection("facebook:access_token").Value;
            string url = $"{_configurgation.GetSection("facebook:urlaboutdescription").Value}{access_token}";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            try
            {
                result = await client.GetFromJsonAsync<teamabout>("");
            }
            catch
            {
                result = new teamabout();
            }
            if (language == "english")
            {
                return result.description;
            }
            else if (language == "chinese")
            {
                return result.about;
            }
            else
            {
                return null;
            }
        }
    }
}