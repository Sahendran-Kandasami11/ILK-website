using System.ComponentModel.DataAnnotations;

namespace test_ngo.Models
{
    public class References
    {
        [Key]
        public int ReferenceID { get; set; }
        public int UserID { get; set; }
        public int ApplicationID { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Details { get; set; }
    }
}
