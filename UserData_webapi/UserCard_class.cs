using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    public class UserCard
    {
        [Required]
        public string ID { get; set; }

        [Required]
        public string UID { get; set; }
        [Required]
        public bool freeze { get; set; } = false;
    }
}
