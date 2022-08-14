using isRock.LineBot;

namespace UserData_webapi
{
    public class linkline
    {
        string ChannelAccessToken = "";
        string linenotifyManagerToken = "";
        string linenotifyToken = "";
        private readonly IConfiguration _configuration;
        public linkline(IConfiguration configuration)
        {
            _configuration = configuration;
            ChannelAccessToken = _configuration.GetValue<string>("line:ChannelAccessToken");
            linenotifyManagerToken = _configuration.GetValue<string>("line:linenotifyManagerToken"); ;
            linenotifyToken = _configuration.GetValue<string>("line:linenotifyToken");
        }
        public string getusername(string ID)
        {
            string Name = null;
            try
            {
                Name = isRock.LineBot.Utility.GetUserInfo(ID, ChannelAccessToken).displayName;
            }
            catch
            {
                return null;
            }
            return Name;
        }
        public void sendlinenotify(string Message,string  level)
        {

            switch (level)
            {
                case "level1":
                    isRock.LineNotify.Utility.SendNotify(linenotifyManagerToken, Message);
                    break;
                case "level2":
                    isRock.LineNotify.Utility.SendNotify(linenotifyManagerToken, Message);
                    isRock.LineNotify.Utility.SendNotify(linenotifyToken, Message);
                    break;
            }

        }
        public void sendlinebot(string Token,string Message)
        {
            Utility.ReplyMessage(Token, Message, ChannelAccessToken);
        }
        public byte[] getaudiobytes(string ID)
        {
            return Utility.GetUserUploadedContent(ID, ChannelAccessToken);
        }
    }
}