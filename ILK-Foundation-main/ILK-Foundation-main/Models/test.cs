using System.ComponentModel.DataAnnotations;

namespace test_ngo.Models
{
    public class test
    {
        [Key]
        public int testId { get; set; }
        public string testName { get; set; }    
    }
}
