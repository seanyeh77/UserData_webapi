using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Net.Http.Headers;
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
        public UserDataRepository(IHostEnvironment environment, IFaceRepository faceRepository)
        {
            _environment = environment;
            _faceRepository = faceRepository;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string jsonfile = Path.Combine(_environment.ContentRootPath, "json", "User.json");
            imgfile = Path.Combine(_environment.ContentRootPath, "img");
            Init(jsonfile);
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

        public string getchinesename(int id)
        {
            return _todoList.FirstOrDefault(item => item.ID == id).ChineseName;
        }
        public bool DoesItemExistID(int id)
        {
            return _todoList.Any(item => item.ID == id);
        }
        public bool DoesItemExistfreeze(int id)
        {
            return _todoList.Any(item => item.ID == id && item.freeze);
        }
        public bool DoesItemExistfreezefalse(int ID)
        {
            return _todoList.Any(item => item.ID == ID && !item.freeze);
        }
        public bool getIDstate(int ID)
        {
            return _todoList.FirstOrDefault(x => x.ID == ID).state;
        }
        public UserData FindID(int ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID);
        }
        public UserData Find(int ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID && item.freeze == false);
        }
        public async Task Insert(UserData item)
        {
            Person person = await _faceRepository.CreatePersonGroupPersonAsync(item.ID);
            IFormFile file = item.Image.First();
            var filePath = Path.Combine(imgfile, item.ID.ToString(), file.FileName);
            if (!File.Exists(filePath))
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            foreach (var formFile in item.Image)
            {
                PersistedFace face = await _faceRepository.AddPersonGroupPersonFaceAsync(person.PersonId, item.ChineseName, formFile.OpenReadStream());
            }
            await _faceRepository.TrainingPersonGroupAsync();
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
                freeze = item.freeze,
                personID = person.PersonId,
            };
            _todoList.Add(userData);
            SaveToFile();
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
            var ID = this.FindID(item.ID);
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
                freeze = item.freeze,
                personID = ID.personID,
            };
            var index = _todoList.IndexOf(ID);
            _todoList.RemoveAt(index);
            _todoList.Insert(index, item);
            SaveToFile();
        }

        public async Task Delete(int ID)
        {
            UserData userData = Find(ID);
            await _faceRepository.DeletePersonGroupPersonAsync(userData.personID);
            DeleteDirectory(Path.Combine(imgfile, ID.ToString()));
            _todoList.Remove(userData);
            SaveToFile();
        }
        public void DeletefreezeID(int ID)
        {
            foreach (UserData item in _todoList.Where(x => x.ID == ID))
            {
                item.freeze = true;
            }
            SaveToFile();
        }
        public void DeletedisfreezeID(int ID)
        {
            foreach (UserData item in _todoList.Where(x => x.ID == ID))
            {
                item.freeze = false;
            }
            SaveToFile();
        }
        public void changestate(int ID)
        {
            _todoList.FirstOrDefault(x => x.ID == ID).state = !_todoList.FirstOrDefault(x => x.ID == ID).state;
            SaveToFile();
        }
        public bool getstate(int ID)
        {
            return _todoList.FirstOrDefault(x => x.ID == ID).state;
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
    }
}
