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
        public int ID { get; set; }
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
        [Required]
        public List<IFormFile> Image { get; set; }
        /// <summary>
        /// ���A
        /// </summary>
        public bool state { get; set; } =false;
        /// <summary>
        /// �ᵲ���A
        /// </summary>
        public bool freeze { get; set; } =false;
        /// <summary>
        /// �s�դH�yID
        /// </summary>
        public Guid personID { get; set; }
    }
}