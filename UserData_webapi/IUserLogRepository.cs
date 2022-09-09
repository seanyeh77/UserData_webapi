namespace UserData_webapi
{
    public interface IUserLogRepository
    {
        IEnumerable<UserPoint> usertable(IUserCardRepository _userCardRepistory, IUserDataRepository _userDataRepository);
        bool DoesItemExist(string id);
        //public IEnumerable<UserPoint> usertable();
        IEnumerable<UserLog> UserLogs_All { get; }
        UserLog FindUID(string id);
        void UserLog_Insert(UserLog item);
        void UserLogByFace_Insert(UserLogByFace item);
        void DeleteUIDAll(string id);
        Task<(List<UserData>, int)> detect_face(IFormFile formFile);
    }
}
