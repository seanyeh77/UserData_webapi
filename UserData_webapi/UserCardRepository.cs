using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserData_webapi
{
    public class UserCardRepository : IUserCardRepository
    {
        private List<UserCard> _todoList;
        private string _file;
        private readonly IHostEnvironment _environment;
        private readonly IUserLogRepository _userLogRepistory;

        public UserCardRepository(IHostEnvironment environment,IUserLogRepository userLogRepistory)
        {
            _environment = environment;
            _userLogRepistory = userLogRepistory;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string file = Path.Combine(_environment.ContentRootPath, "json", "UserCard.json");
            Init(file);
        }
        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize<IList<UserCard>>(_todoList);
            File.WriteAllText(_file, json, Encoding.UTF8);
        }
        public void Init(string file)
        {
            //file=Path.
            _file = file;
            string json = "";
            if (File.Exists(file))
            {
                json = File.ReadAllText(file, Encoding.UTF8);
                _todoList = JsonSerializer.Deserialize<List<UserCard>>(json);
            }
            else
            {
                _todoList = new List<UserCard>();
            }
        }
        public IEnumerable<UserCard> All
        {
            get { return _todoList; }
        }
        public string GetID(string UID)
        {
            var user =  _todoList.Where(x => x.UID == UID).ToList();
            if (!user.Any())
            {
                return "";
            }
            return user[0].ID;
        }
        public int GetCount(string ID)
        {
            return _todoList.Count(x =>x.ID==ID&&!x.freeze);
        }
        public bool DoesItemExistUID(string UID)
        {
            return _todoList.Any(item => item.UID == UID);
        }
        public bool DoesItemExistfreezefalse(string UID)
        {
            return _todoList.Any(item => item.UID == UID && item.freeze == false);
        }
        public UserCard FindUID(string UID)
        {
            return _todoList.FirstOrDefault(item => item.UID == UID);
        }
        public UserCard FindUIDfreezefalse(string UID)
        {
            return _todoList.FirstOrDefault(item => item.UID == UID && item.freeze==false);
        }
        public void Insert(UserCard item)
        {
            _todoList.Add(item);
            SaveToFile();
        }
        public void Update(UserCard item)
        {
            var Name = this.FindUID(item.UID);
            var index = _todoList.IndexOf(Name);
            _todoList.RemoveAt(index);
            _todoList.Insert(index, item);
            SaveToFile();
        }
        public void DeleteID(string ID)
        {
            foreach (var item in _todoList.Where(x => x.ID == ID))
            {
                _userLogRepistory.DeleteUIDAll(item.UID);
            }
            _todoList.RemoveAll(x => x.ID == ID);
            SaveToFile();
        }
        public void DeletefreezeUID(string UID)
        {
            foreach (UserCard item in _todoList.Where(x => x.UID == UID))
            {
                item.freeze = true;
            }
            SaveToFile();
        }
    }
}
