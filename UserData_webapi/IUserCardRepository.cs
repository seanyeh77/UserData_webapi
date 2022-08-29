namespace UserData_webapi
{
    public interface IUserCardRepository
    {
        public string GetID(string UID);
        public int GetCount(string ID);
        bool DoesItemExistUID(string UID);
        bool DoesItemExistfreezefalse(string UID);
        IEnumerable<UserCard> All { get; }
        UserCard FindUID(string UID);
        UserCard FindUIDfreezefalse(string UID);
        void Insert(UserCard item);
        void Update(UserCard item);
        void DeleteID(string ID);
        void DeletefreezeUID(string UID);
    }
}
