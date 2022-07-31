namespace UserData_webapi
{
    public interface IUserLogRepository
    {
        IEnumerable<UserPoint> usertable(IUserCardRepository _userCardRepistory, IUserDataRepository _userDataRepository);
        bool DoesItemExist(string id);
        //public IEnumerable<UserPoint> usertable();
        IEnumerable<UserLog> All { get; }
        UserLog FindUID(string id);
        void Insert(UserLog item);
        void DeleteUIDAll(string id);
    }
}
