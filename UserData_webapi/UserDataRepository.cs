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

        public UserDataRepository(IHostEnvironment environment)
        {
            _environment = environment;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string file = Path.Combine(_environment.ContentRootPath, "json", "User.json");
            Init(file);
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

        public string getname(int id)
        {
            return _todoList.FirstOrDefault(item => item.ID == id).Name;
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
        public void Insert(UserData item)
        {
            _todoList.Add(item);
            SaveToFile();
        }
        public void reset()
        {
            foreach(UserData item in _todoList)
            {
                item.state = false;
            }
            SaveToFile();
        }
        public void Update(UserData item)
        {
            var Name = this.FindID(item.ID);
            var index = _todoList.IndexOf(Name);
            if(item.Name == "null")
            {
                item.Name = Name.Name;
            }
            if (item.grade == 500)
            {
                item.grade = Name.grade;
            }
            _todoList.RemoveAt(index);
            _todoList.Insert(index, item);
            SaveToFile();
        }

        public void Delete(int ID)
        {
            _todoList.Remove(FindID(ID));
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
    }
}
