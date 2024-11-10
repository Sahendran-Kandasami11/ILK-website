using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace test_ngo.Models
{
    public class Event
    {
        [Key]
        public string EventID { get; set; }  // Changed to string to match Firestore ID
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public string Description { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public string ImageUrl { get; set; }  // New property to store Firebase Storage image URL
    }
}
