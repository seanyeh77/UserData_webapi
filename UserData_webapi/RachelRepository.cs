using System.Text;
using System.Text.Json;

namespace UserData_webapi
{
    public class RachelRepository : IRechalRepository
    {
        private List<Rachelstate> _todoList;
        private string _file;
        private readonly IHostEnvironment _environment;
        private readonly IUserLogRepository _userLogRepistory;
        private readonly IUserCardRepository _userCardRepository;

        public RachelRepository(IHostEnvironment environment, IUserLogRepository userLogRepistory, IUserCardRepository userCardRepository)
        {
            _environment = environment;
            _userLogRepistory = userLogRepistory;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string file = Path.Combine(_environment.ContentRootPath, "json", "Rachelstate.json");
            Init(file);
            _userCardRepository = userCardRepository;
        }
        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize<IList<Rachelstate>>(_todoList);
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
                _todoList = JsonSerializer.Deserialize<List<Rachelstate>>(json);
            }
            else
            {
                _todoList = new List<Rachelstate>();
                _todoList.Add(new Rachelstate() { ID = null,state = false});
            }
        }
        public IEnumerable<Rachelstate> All
        {
            get { return _todoList; }
        }
        public void Update(Rachelstate rachelstate)
        {
            _todoList.RemoveAt(0);
            _todoList.Insert(0, rachelstate);
            SaveToFile();
        }
        public bool getstate()
        {
            return _todoList.First().state;
        }
    }
}
