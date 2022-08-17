using System.ComponentModel.DataAnnotations;

namespace UserData_webapi
{
    public class UserPoint
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int ID { get; set; }
        [Required]
        public int? grade { get; set; }
        [Required]
        public float monaverage { get; set; }
        [Required]
        public double dayaverage { get; set; }
        public double totaleverydayminute { get; set; }
        public int notcheckday { get; set; }
        public bool state { get; set; }
    }
}
