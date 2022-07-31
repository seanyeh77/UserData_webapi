using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    public class UserLog
    {
        [Required]
        public string UID { get; set; }

        [Required]
        public DateTime time { get; set; }
    }
}
