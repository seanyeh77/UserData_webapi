using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    public class UserData
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int grade { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public bool freeze { get; set; } = false;
        [Required]
        public bool state { get; set; } = false;
    }
}