namespace UserData_webapi
{
    public interface IUserCardRepository
    {
        public int GetID(string UID);
        public int GetCount(int ID);
        bool DoesItemExistUID(string UID);
        bool DoesItemExistfreezefalse(string UID);
        IEnumerable<UserCard> All { get; }
        UserCard FindUID(string UID);
        UserCard FindUIDfreezefalse(string UID);
        void Insert(UserCard item);
        void Update(UserCard item);
        void DeleteID(int ID);
        void DeletefreezeUID(string UID);
    }
}
