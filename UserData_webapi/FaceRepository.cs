using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace UserData_webapi
{
    public class FaceRepository : IFaceRepository
    {
        private readonly IConfiguration _configuration;
        HttpClient _httpClient = new HttpClient();
        public FaceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }
        private static Byte[] ToByteArray(Stream stream)
        {
            Int32 length = stream.Length > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(stream.Length);
            Byte[] buffer = new Byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }
        public async Task<List<string>> DetictFace(IFormFile formFile)
        {
            _httpClient= new HttpClient() { BaseAddress = new Uri(_configuration.GetSection("face++:url").Value + _configuration.GetSection("face++:Detect").Value) };
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            string apiKey = _configuration.GetSection("face++:API Key").Value;
            string apiSecret = _configuration.GetSection("face++:API Secret").Value;
            Stream sf = formFile.OpenReadStream();
            dictionary.Add("api_key", apiKey);
            dictionary.Add("api_secret", apiSecret);            
            dictionary.Add("image_base64", Convert.ToBase64String(ToByteArray(sf)));
            var content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage response = await _httpClient.PostAsync("", content);
            if (!response.IsSuccessStatusCode)
            {
                string str = await response.Content.ReadAsStringAsync();
                return null;
            }
            var json  = await response.Content.ReadFromJsonAsync<Detectjson>();
            List<string> faces_token_list = new List<string>();
            foreach(var item in json.faces)
            {
                faces_token_list.Add(item.face_token);
            }
            return faces_token_list;
        }
        public async Task<List<SearchUser>> SearchUser(List<string> face_tokes)
        {
            try
            {
                List<SearchUser> searchUsers = new List<SearchUser>();
                if (!face_tokes.Any())
                {
                    return null;
                }
                _httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection("face++:url").Value + _configuration.GetSection("face++:Search").Value) };
                foreach (var face in face_tokes)
                {
                    Dictionary<String, String> dictionary = new Dictionary<string, string>();
                    string apiKey = _configuration.GetSection("face++:API Key").Value;
                    string apiSecret = _configuration.GetSection("face++:API Secret").Value;
                    dictionary.Add("api_key", apiKey);
                    dictionary.Add("api_secret", apiSecret);
                    dictionary.Add("faceset_token", _configuration.GetSection("face++:faceset_token").Value);
                    dictionary.Add("face_token", face);
                    var content = new FormUrlEncodedContent(dictionary);
                    //string end_point = _configuration.GetSection("face++:Detect").Value;
                    HttpResponseMessage response = await _httpClient.PostAsync("", content);
                    if (response.IsSuccessStatusCode)
                    {
                        searchUsers.Add(await response.Content.ReadFromJsonAsync<SearchUser>());
                    }
                }
                return searchUsers;
            }
            catch
            {
                return null;
            }
        }
        //public Task CreateFaceSet()
        //{

        //}
        public async Task<int> AddFaceAsync(List<string> face_tokens)
        {
            List<string> face_tokes_temp = new List<string>();//暫存face_toke
            int sucess_count = 0;//紀錄成功數量
            for (int index = 0; index < face_tokens.Count; index++)
            {
                face_tokes_temp.Add(face_tokens[index]);
                if (index + 1 % 5 == 0)//限制每五個發送一次
                {
                    sucess_count += await sendface_tokens(face_tokes_temp);
                    face_tokes_temp.Clear();
                }
            }
            sucess_count += await sendface_tokens(face_tokes_temp);
            return sucess_count;
        }
        /// <summary>
        /// 發送新增face_token需求
        /// </summary>
        /// <param name="face_tokes_temp"></param>
        /// <returns></returns>
        public async Task<int> sendface_tokens(List<string> face_tokes_temp)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection("face++:url").Value + _configuration.GetSection("face++:FaceSet:AddFaceAsync").Value) };
            string face_temp = string.Join(",", face_tokes_temp.Select(s => s));
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            string apiKey = _configuration.GetSection("face++:API Key").Value;
            string apiSecret = _configuration.GetSection("face++:API Secret").Value;
            string faceset_token = _configuration.GetSection("face++:faceset_token").Value;
            dictionary.Add("api_key", apiKey);
            dictionary.Add("api_secret", apiSecret);
            dictionary.Add("faceset_token", faceset_token);
            dictionary.Add("face_tokens", face_temp);
            var content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage response = await _httpClient.PostAsync("", content);
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }
            return face_tokes_temp.Count();
        }
        public async Task<int> RemoveFaceAsync(List<string> face_tokens)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection("face++:url").Value + _configuration.GetSection("face++:FaceSet:RemoveFaceAsync").Value) };
            string face_temp = string.Join(",", face_tokens.Select(s => s));
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            string apiKey = _configuration.GetSection("face++:API Key").Value;
            string apiSecret = _configuration.GetSection("face++:API Secret").Value;
            string faceset_token = _configuration.GetSection("face++:faceset_token").Value;
            dictionary.Add("api_key", apiKey);
            dictionary.Add("api_secret", apiSecret);
            dictionary.Add("faceset_token", faceset_token);
            dictionary.Add("face_tokens", face_temp);
            var content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage response = await _httpClient.PostAsync("", content);
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }
            return face_temp.Count();
        }
    }
}
