using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserData_webapi
{
    public class UserDataRepository : IUserDataRepository
    {
        private List<UserData> _todoList;
        private string _file;
        private readonly IHostEnvironment _environment;
        private readonly IFaceRepository _faceRepository;
        string imgfile = string.Empty;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserDataRepository(IHostEnvironment environment, IFaceRepository faceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _faceRepository = faceRepository;
            _contextAccessor = httpContextAccessor;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (!Directory.Exists(json_dir))
                Directory.CreateDirectory(json_dir);
            string jsonfile = Path.Combine(_environment.ContentRootPath, "json", "User.json");
            Init(jsonfile);
            imgfile = Path.Combine(_environment.ContentRootPath, "img");
            if (!Directory.Exists(imgfile))
                Directory.CreateDirectory(imgfile);

        }
        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize<IList<UserData>>(_todoList);
            File.WriteAllText(_file, json, Encoding.UTF8);
        }
        public void Init(string file)
        {
            _file = file;
            string json = "";
            if (File.Exists(file))
            {
                json = File.ReadAllText(file, Encoding.UTF8);
                _todoList = JsonSerializer.Deserialize<List<UserData>>(json);
            }
            else
            {
                _todoList = new List<UserData>();
            }
        }
        public IEnumerable<UserData> All
        {
            get { return _todoList; }
        }
        public async Task<List<UserData>> GetUserData_poistion(string position)
        {
            List<UserData> list = new List<UserData>();
            foreach (UserData item in _todoList.Where(x => x.position == position))
            {
                UserData userData = item.CopyTo();
                var filePath = Path.Combine(imgfile, item.ID);
                DirectoryInfo di = new DirectoryInfo(filePath);
                filePath = $"{GetCompleteUrl()}/img/{item.ID}/{di.GetFiles().First().Name}";
                userData.url = filePath;
                list.Add(userData);
            }
            return list;
        }
        public string getchinesename(string id)
        {
            UserData userdata = _todoList.FirstOrDefault(item => item.ID == id);
            if (userdata == null)
            {
                return null;
            }
            return _todoList.FirstOrDefault(item => item.ID == id).ChineseName;
        }
        public bool DoesItemExistID(string id)
        {
            return _todoList.Any(item => item.ID == id);
        }
        public bool DoesItemExistlock(string id)
        {
            return _todoList.Any(item => item.ID == id && item.Lock);
        }
        public bool DoesItemExistlockfalse(string ID)
        {
            return _todoList.Any(item => item.ID == ID && !item.Lock);
        }
        public UserData FindID(string ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID);
        }
        public UserData Find(string ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID && item.Lock == false);
        }
        public async Task<int> Insert(UserData item)
        {
            List<string> face_tokens = new List<string>();
            foreach (IFormFile formFile in item.Image)
            {
                List<string> faces_token = await _faceRepository.DetictFace(formFile);
                if (faces_token.Count() == 1)
                {
                    face_tokens.Add(faces_token.First());
                }
                else
                {
                    return 0;
                }
            }
            int sucess_count = await _faceRepository.AddFaceAsync(face_tokens);
            IFormFile file = item.Image.First();
            var filePath = Path.Combine(imgfile, item.ID);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = Path.Combine(filePath, file.FileName);
            if (file.Length > 0)
            {

                using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            UserData userData = new UserData()
            {
                ID = item.ID,
                ChineseName = item.ChineseName,
                EnglishName = item.EnglishName,
                grade = item.grade,
                email = item.email,
                gender = item.gender,
                position = item.position,
                view = item.view,
                state = false,
                Lock = false,
                face_tokens = face_tokens,
            };
            _todoList.Add(userData);
            SaveToFile();
            return sucess_count;
        }
        public void reset()
        {
            foreach (UserData item in _todoList)
            {
                item.state = false;
            }
            SaveToFile();
        }
        public void Update(UserData item)
        {
            UserData ID = this.FindID(item.ID);
            UserData userData = new UserData()
            {
                ID = item.ID,
                ChineseName = item.ChineseName,
                EnglishName = item.EnglishName,
                grade = item.grade,
                email = item.email,
                gender = item.gender,
                position = item.position,
                view = item.view,
                state = item.state,
                Lock = item.Lock,
                face_tokens = ID.face_tokens,
            };
            var index = _todoList.IndexOf(ID);
            _todoList.RemoveAt(index);
            _todoList.Insert(index, userData);
            SaveToFile();
        }
        public async Task<int> Delete(string ID)
        {
            UserData userData = Find(ID);
            int sucess = await _faceRepository.RemoveFaceAsync(userData.face_tokens);
            DeleteDirectory(Path.Combine(imgfile, ID));
            _todoList.Remove(userData);
            SaveToFile();
            return sucess;
        }
        public void DeletelockID(string ID)
        {
            _todoList.FirstOrDefault(x => x.ID == ID).Lock = true;
            _todoList.FirstOrDefault(x => x.ID == ID).state = false;
            SaveToFile();
        }
        public void DeleteunLockID(string ID)
        {
            _todoList.FirstOrDefault(x => x.ID == ID).Lock = false;
            _todoList.FirstOrDefault(x => x.ID == ID).state = false;
            SaveToFile();
        }
        public void changestate(string ID)
        {
            _todoList.FirstOrDefault(x => x.ID == ID).state = !_todoList.FirstOrDefault(x => x.ID == ID).state;
            SaveToFile();
        }
        public bool getstate(string ID)
        {
            return _todoList.FirstOrDefault(x => x.ID == ID).state;

        }
        public async Task<(UserData, int)> SearchUser(IFormFile formFile)
        {
            List<string> face_tokes = await _faceRepository.DetictFace(formFile);
            if (!face_tokes.Any())
            {
                return (null, 0);//找不到人臉
            }
            List<SearchUser> user = await _faceRepository.SearchUser(face_tokes);
            if (user != null)
            {
                UserData selectuser = _todoList.FirstOrDefault(item => item.face_tokens.Any(x => x == ((user.First().results.First().confidence >= 75) ? user.First().results.First().face_token : "")));
                if (selectuser != null)
                {
                    return (selectuser, 2);//找到人臉也找到人
                }
                else
                {
                    return (null, 1);//找到人臉但找不到人
                }
            }
            else
            {
                return (null, 3);//與Face++連線時出現問題
            }
        }
        public string getemail(string ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID).email;
        }
        /// <summary>
        /// 刪除資料夾
        /// </summary>
        /// <param name="path"></param>
        void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }
        /// <summary>
        /// 獲取當前請求完整的Url地址
        /// </summary>
        /// <returns></returns>
        private string GetCompleteUrl()
        {
            return new StringBuilder()
                 .Append(_contextAccessor.HttpContext.Request.Scheme)
                 .Append("://")
                 .Append(_contextAccessor.HttpContext.Request.Host)
                 .Append(_contextAccessor.HttpContext.Request.PathBase)
                 .Append(_contextAccessor.HttpContext.Request.QueryString)
                 .ToString();
        }
    }
}