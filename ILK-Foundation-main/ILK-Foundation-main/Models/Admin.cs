using System.ComponentModel.DataAnnotations;

namespace test_ngo.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
