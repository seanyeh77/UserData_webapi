using System.Net.Mail;
using System.Net;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using Amazon;
using Microsoft.Extensions.Hosting;

namespace UserData_webapi
{
    public class SendEmail
    {
        private readonly IConfiguration _configuration;
        private readonly IUserDataRepository _userDataRepository;
        public SendEmail(IConfiguration configuration, IUserDataRepository userDataRepository)
        {
            _configuration = configuration;
            _userDataRepository = userDataRepository;
        }
        /// <summary>
        /// 發送電子郵件
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task sendemail_id(string ID,string subject,string message)
        {
            linkline linkline = new linkline(_configuration);
            string toemail = _userDataRepository.getemail(ID);
            string username = _configuration.GetSection("email:account").Value;
            string password = _configuration.GetSection("email:passward").Value;
            ICredentialsByHost credentials = new NetworkCredential(username, password);
            SmtpClient smtpClient = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = credentials
            };

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(username);
            mail.To.Add(toemail);
            mail.Subject = subject;
            mail.Body = message;

            await smtpClient.SendMailAsync(mail);
        }
    }
}
