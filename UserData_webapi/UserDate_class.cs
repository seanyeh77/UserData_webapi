using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    /// <summary>
    /// �ϥΪ̸��
    /// </summary>
    public class UserData        
    {
        

        /// <summary>
        /// UserID
        /// </summary>
        [Required]
        public string ID { get; set; }
        /// <summary>
        /// ����m�W
        /// </summary>
        [Required]
        public string ChineseName { get; set; }
        /// <summary>
        /// �^��m�W
        /// </summary>
        [Required]
        public string EnglishName { get; set; }
        /// <summary>
        /// �~��
        /// </summary>
        public int? grade { get; set; }
        /// <summary>
        /// �ʧO
        /// </summary>
        [Required]
        public string gender { get; set; }
        /// <summary>
        /// ¾��
        /// </summary>
        [Required]
        public string position { get; set; }
        /// <summary>
        /// email
        /// </summary>
        [Required]
        public string email { get; set; }
        /// <summary>
        /// �ӤH²��
        /// </summary>
        [Required]
        public string view { get; set; }
        /// <summary>
        /// �y���Ӥ��ɮ�
        /// </summary>
        public List<IFormFile>? Image { get; set; }
        /// <summary>
        /// ���A
        /// </summary>
        public bool state { get; set; } = false;
        /// <summary>
        /// ��w���A
        /// </summary>
        public bool Lock { get; set; } = false;
        /// <summary>
        /// �s�դH�yID
        /// </summary>
        public List<string> face_tokens { get; set; } = new List<string>();
        public string url { get; set; } = "";

        public UserData CopyTo()
        {
            UserData userData1 = new UserData();
            userData1.ID = this.ID;
            userData1.ChineseName = this.ChineseName;
            userData1.EnglishName = this.EnglishName;
            userData1.position = this.position;
            userData1.state = this.state;
            userData1.email = this.email;
            userData1.view = this.view;
            userData1.gender = this.gender;
            userData1.grade = this.grade;
            return userData1;
        }
    }
}