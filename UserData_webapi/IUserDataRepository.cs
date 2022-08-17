namespace UserData_webapi
{
    public interface IUserDataRepository
    {
        public string getchinesename(int id);
        bool DoesItemExistID(int ID); 
        bool DoesItemExistfreeze(int ID);
        bool DoesItemExistfreezefalse(int ID);
        public bool getIDstate(int ID);
        IEnumerable<UserData> All { get; }
        UserData FindID(int ID); 
        UserData Find(int ID);
        public void reset();
        Task Insert(UserData item);
        void Update(UserData item);
        Task Delete(int ID);
        void DeletefreezeID(int ID);
        void DeletedisfreezeID(int ID);
        void changestate(int ID);
        bool getstate(int ID);
    }
}
