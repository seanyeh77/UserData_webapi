namespace UserData_webapi
{
    public interface IUserDataRepository
    {
        public Task<List<UserData>> GetUserData_poistion(string position);
        public string getchinesename(string id);
        bool DoesItemExistID(string ID);
        bool DoesItemExistfreeze(string ID);
        bool DoesItemExistfreezefalse(string ID);
        IEnumerable<UserData> All { get; }
        UserData FindID(string ID);
        UserData Find(string ID);
        public void reset();
        public Task<int> Insert(UserData item);
        void Update(UserData item);
        Task<int> Delete(string ID);
        void DeletefreezeID(string ID);
        void DeletedisfreezeID(string ID);
        void changestate(string ID);
        bool getstate(string ID);
        public Task<(UserData, int)> SearchUser(IFormFile formFile);
        public string getemail(string ID);
    }
}