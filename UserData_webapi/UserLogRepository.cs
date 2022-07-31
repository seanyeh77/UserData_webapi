using System.Linq;
using System.Text;
using System.Text.Json;

namespace UserData_webapi
{
    public class UserLogRepository : IUserLogRepository
    {
        private List<UserLog> _todoList;
        private string _file;
        private readonly IHostEnvironment _environment;
        private readonly IUserDataRepository _userDataRepository;

        public UserLogRepository(IHostEnvironment environment, IUserDataRepository userDataRepistory)
        {
            _environment = environment;
            string json_dir = Path.Combine(_environment.ContentRootPath, "json");
            if (Directory.Exists(json_dir) == false)
                Directory.CreateDirectory(json_dir);
            string file = Path.Combine(_environment.ContentRootPath, "json", "UserLog.json");
            Init(file);
            _userDataRepository = userDataRepistory;
        }
        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize<IList<UserLog>>(_todoList);
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
                _todoList = JsonSerializer.Deserialize<List<UserLog>>(json);
            }
            else
            {
                _todoList = new List<UserLog>();
            }
        }
        public IEnumerable<UserLog> All
        {
            get { return _todoList; }
        }
        public IEnumerable<UserPoint> usertable(IUserCardRepository _userCardRepistory, IUserDataRepository _userDataRepository)
        {
            int beforenoon = 12;
            int afternoon = 13;
            int beforeafternoon = 16;
            int afterafternoon = 21;
            var quary = from x in _userDataRepository.All
                        join y in _userCardRepistory.All on x.ID equals y.ID into xy
                        from y in xy.DefaultIfEmpty()
                        join z in _todoList on y == null ? "" : y.UID equals z.UID into yz
                        from z in yz.DefaultIfEmpty()
                        select new alluserdata { ID = x.ID, Name = x.Name, grade = x.grade, UID = y == null ? "" : y.UID, Time = z == null ? new DateTime() : z.time, Freeze = x.freeze };
            quary = from x in quary
                        // 暑假時段(只到21點)
                    where (((int)x.Time.Month ==7 || (int)x.Time.Month == 8) && x.Time.Hour <= afterafternoon)
                        // 非暑假且為假日時段(只到21點)
                        || (((int)x.Time.Month != 7 || (int)x.Time.Month != 8)&&((int)x.Time.DayOfWeek == 0 || (int)x.Time.DayOfWeek == 6) && x.Time.Hour <= afterafternoon)
                        // 非暑假且非假日時段(中午12點到13點、16點到21點)
                        || (((int)x.Time.Month != 7 || (int)x.Time.Month != 8) && ((int)x.Time.DayOfWeek != 0 && (int)x.Time.DayOfWeek != 6)&&((x.Time.Hour >= beforenoon && x.Time.Hour <= afternoon) || (x.Time.Hour >= beforeafternoon && x.Time.Hour <= afterafternoon)))
                    select x;

            //算出全部總時間
            var userPoint1 = from x in quary
                             group x by new { x.ID, x.Time.Date ,time_type= ((int)x.Time.Month == 7 || (int)x.Time.Month == 8?0:((int)x.Time.DayOfWeek == 0 || (int)x.Time.DayOfWeek == 6 ?1: (x.Time.Hour >= beforenoon && x.Time.Hour <= afternoon?2:3))) } into g
                             select new
                             {
                                 ID = g.Key.ID,
                                 totaleverydayminute = (g.Max(a => a.Time) - g.Min(a => a.Time)).TotalMinutes,
                                 time_type = g.Key.time_type
                             };
            var userPoint6 = from x in userPoint1
                             group x by x.ID into g
                             select new UserPoint
                             {
                                 ID = g.Key,
                                 totaleverydayminute = g.Sum(a => a.totaleverydayminute)
                             };
            //算出上個月總天數
            var userPoint2 = from x in quary
                             where x.Time.ToString("yyyyMM") == DateTime.Today.AddMonths(-1).ToString("yyyyMM")
                             group x by x.ID into g
                             select new
                             {
                                 ID = g.Key,
                                 monaverage = (float)g.Select(y => y.Time.Day).Distinct().Count()
                             };
            //算出上個月每一天的時間
            var userPoint3 = from x in quary
                             where x.Time.ToString("yyyyMM") == DateTime.Today.AddMonths(-1).ToString("yyyyMM")
                             group x by new { x.ID, x.Time.Date, time_type = ((int)x.Time.DayOfWeek == 0 || (int)x.Time.DayOfWeek == 6 ? 0 : (x.Time.Hour >= beforenoon && x.Time.Hour <= afternoon ? 1 : 2)) } into g
                             select new
                             {
                                 ID = g.Key.ID,
                                 Dates = g.Key.Date,
                                 totalminutes = (g.Max(a => a.Time) - g.Min(a => a.Time)).TotalMinutes,
                                 time_type = g.Key.time_type
                             };
            //算出上個月總時間
            var userPoint4 = from x in userPoint3
                             group x by x.ID into g
                             select new
                             {
                                 ID = g.Key,
                                 dayaverage = (float)g.Sum(p => p.totalminutes)
                             };
            var userPoint7 = from x in quary
                             group x by x.ID into g
                             select new
                             {
                                 ID = g.Key,
                                 notcheckday = (DateTime.Now - g.Max(y => y.Time)).Days,
                             };


            var userPoint5 = from a in _userDataRepository.All
                             join d in userPoint6 on a.ID equals d.ID into e
                             from d in e.DefaultIfEmpty()
                             join b in userPoint2 on a.ID equals b.ID into f
                             from b in f.DefaultIfEmpty()
                             join c in userPoint4 on a.ID equals c.ID into g
                             from c in g.DefaultIfEmpty()
                             join i in userPoint7 on a.ID equals i.ID into h
                             from i in h.DefaultIfEmpty()
                             select new UserPoint
                             {
                                 ID = a.ID,
                                 Name = a.Name,
                                 grade = a.grade,
                                 state = a.state,
                                 monaverage = b == null ? new float() : b.monaverage,
                                 dayaverage = c == null ? new float() : c.dayaverage,
                                 totaleverydayminute = d==null?new int():d.totaleverydayminute,
                                 notcheckday = i==null ?  new int():i.notcheckday,
                             };
            return userPoint5;
        }
        public bool DoesItemExist(string UID)
        {
            return _todoList.Any(item => item.UID == UID);
        }

        public UserLog FindUID(string UID)
        {
            return _todoList.FirstOrDefault(item => item.UID == UID);
        }
        public void Insert(UserLog item)
        {
            _todoList.Add(item);
            SaveToFile();
        }

        public void Update(UserLog item)
        {
            var Name = this.FindUID(item.UID);
            var index = _todoList.IndexOf(Name);
            _todoList.RemoveAt(index);
            _todoList.Insert(index, item);
            SaveToFile();
        }

        public void DeleteUIDAll(string UID)
        {
            _todoList.RemoveAll(x => x.UID == UID);
            SaveToFile();
        }
    }
}
