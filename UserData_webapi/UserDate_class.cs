using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    /// <summary>
    /// 使用者資料
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// UserID
        /// </summary>
        [Required]
        public int ID { get; set; }
        /// <summary>
        /// 中文姓名
        /// </summary>
        [Required]
        public string ChineseName { get; set; }
        /// <summary>
        /// 英文姓名
        /// </summary>
        [Required]
        public string EnglishName { get; set; }
        /// <summary>
        /// 年級
        /// </summary>
        public int? grade { get; set; }
        /// <summary>
        /// 性別
        /// </summary>
        [Required]
        public string gender { get; set; }
        /// <summary>
        /// 職位
        /// </summary>
        [Required]
        public string position { get; set; }
        /// <summary>
        /// email
        /// </summary>
        [Required]
        public string email { get; set; }
        /// <summary>
        /// 個人簡介
        /// </summary>
        [Required]
        public string view { get; set; }
        /// <summary>
        /// 臉部照片檔案
        /// </summary>
        [Required]
        public List<IFormFile> Image { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public bool state { get; set; } =false;
        /// <summary>
        /// 凍結狀態
        /// </summary>
        public bool freeze { get; set; } =false;
        /// <summary>
        /// 群組人臉ID
        /// </summary>
        public Guid personID { get; set; }
    }
}