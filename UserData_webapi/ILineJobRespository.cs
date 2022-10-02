﻿namespace UserData_webapi
{
    public interface ILineJobRespository
    {
        public string function();
        public string listdata(string ID);
        public string Lock(string ID);
        public string unLock(string ID);
        public Task<string[]> audiototext(string ID);
        public string state();
        public void sendmessage(string Token, string notify, string bot, string level);
    }
}
