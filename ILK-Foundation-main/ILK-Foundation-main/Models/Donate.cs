using System.ComponentModel.DataAnnotations;

namespace test_ngo.Models
{
    public class Donate
    {
        [Key]
        public int DonateID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateOnly Date { get; set; }
        public double Amount { get; set; }
    }
}
