using NAudio.CoreAudioApi;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserData_webapi
{
    public class LineManageRepository : ILineBotManageRespository
    {
        private List<LineUser> _todoList;
        private string _file;
        private readonly IHostEnvironment _environment;
        public LineManageRepository(IHostEnvironment environment)
        {
            _environment = environment;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string file = Path.Combine(_environment.ContentRootPath, "json", "LineUser.json");
            Init(file);
        }
        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize<IList<LineUser>>(_todoList);
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
                _todoList = JsonSerializer.Deserialize<List<LineUser>>(json);
            }
            else
            {
                _todoList = new List<LineUser>();
            }
        }
        public IEnumerable<LineUser> All
        {
            get { return _todoList; }
        }
        public void adduser(LineUser lineUser)
        {
            _todoList.Add(lineUser);
        }
        public void deluser(string ID)
        {
            _todoList.RemoveAll(item => item.ID == ID);
        }
        public List<LineUser> getalluser()
        {
            return _todoList;
        }
        public LineUser getuserid(string ID)
        {
            return _todoList.FirstOrDefault(item => item.ID == ID);
        }
        public string listralluser()
        {
            if (_todoList.Count == 0)
            {
                return "沒有找到任何使用者的資料";
            }
            string Message = "用戶ID         角色\n";
            foreach (LineUser lineUser in getalluser())
            {
                Message += $"{lineUser.ID}  {lineUser.Role}\n";
            }
            return Message;
        }
        public LineUser getuserrole(string Role)
        {
            return _todoList.FirstOrDefault(item => item.Role == Role);
        }
        public string changeRole(string ID, string Role)
        {
            var user = _todoList.FirstOrDefault(item => item.ID.ToLower() == ID);
            if (user != null)
            {
                if (Role == "admin") //當角色為admin時
                {
                    user.Role = Role;
                }
                else if (Role == "manager") //當角色為manager時
                {
                    user.Role = Role;
                }
                else if (Role == "texter") //當角色為texter時
                {
                    user.Role = Role;
                }
            }
            else
            {
                return $"未找到ID:{ID}";
            }
            SaveToFile();
            return $"已將{user.ID}的角色更改為{user.Role}";
        }
    }
}
