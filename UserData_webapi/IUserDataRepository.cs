namespace UserData_webapi
{
    public interface IUserDataRepository
    {
        public string getname(int id);
        bool DoesItemExistID(int ID); 
        bool DoesItemExistfreeze(int ID);
        bool DoesItemExistfreezefalse(int ID);
        public bool getIDstate(int ID);
        IEnumerable<UserData> All { get; }
        UserData FindID(int ID); 
        UserData Find(int ID);
        public void reset();
        void Insert(UserData item);
        void Update(UserData item);
        void Delete(int ID);
        void DeletefreezeID(int ID);
        void DeletedisfreezeID(int ID);
        void changestate(int ID);
    }
}
