namespace UserData_webapi
{
    public interface ILineBotManageRespository
    {
        void Init(string file);
        IEnumerable<LineUser> All { get; }
        List<LineUser> getalluser();
        LineUser getuserid(string ID);
        LineUser getuserrole(string Role);
        string getusername(string ID);
        string changeRole(string ID, string role);
        void adduser(LineUser user,IConfiguration configuration);
        void deluser(string ID);
        string listralluser();
    }
}
