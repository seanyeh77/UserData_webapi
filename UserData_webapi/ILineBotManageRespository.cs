namespace UserData_webapi
{
    public interface ILineBotManageRespository
    {
        void Init(string file);
        IEnumerable<LineUser> All { get; }
        List<LineUser> getalluser();
        LineUser getuserid(string ID);
        LineUser getuserrole(string Role);
        string changeRole(string ID, string role);
        void adduser(LineUser user);
        void deluser(string ID);
        string listralluser();
    }
}
