namespace UserData_webapi
{
    public interface IUserCardRepository
    {
        public string GetID(string UID);
        public int GetCount(string ID);
        bool DoesItemExistUID(string UID);
        bool DoesItemExistlockfalse(string UID);
        IEnumerable<UserCard> All { get; }
        UserCard FindUID(string UID);
        UserCard FindUIDlockfalse(string UID);
        void Insert(UserCard item);
        void Update(UserCard item);
        void DeleteID(string ID);
        void DeletelockUID(string UID);
    }
}
