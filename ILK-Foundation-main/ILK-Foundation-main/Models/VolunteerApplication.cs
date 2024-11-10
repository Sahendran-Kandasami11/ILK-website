using System.ComponentModel.DataAnnotations;

namespace test_ngo.Models
{
    public class VolunteerApplication
    {
        [Key]
        public int ApplicationID { get; set; }
        public int UserID { get; set; }
        public DateOnly Date { get; set; }
        public string UserAvailability { get; set; }
        public string UserLicense { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
