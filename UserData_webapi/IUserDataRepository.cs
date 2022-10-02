namespace UserData_webapi
{
    public interface IUserDataRepository
    {
        public Task<List<UserData>> GetUserData_poistion(string position);
        public string getchinesename(string id);
        bool DoesItemExistID(string ID);
        bool DoesItemExistlock(string ID);
        bool DoesItemExistlockfalse(string ID);
        IEnumerable<UserData> All { get; }
        UserData FindID(string ID);
        UserData Find(string ID);
        public void reset();
        public Task<int> Insert(UserData item);
        void Update(UserData item);
        Task<int> Delete(string ID);
        void DeletelockID(string ID);
        void DeleteunLockID(string ID);
        void changestate(string ID);
        bool getstate(string ID);
        public Task<(UserData, int)> SearchUser(IFormFile formFile);
        public string getemail(string ID);
    }
}