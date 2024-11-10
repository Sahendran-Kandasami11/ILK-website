using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace test_ngo.Models
{
    public class Testimonial
    {
        [Key]
        public string TestimonialID { get; set; }  // Use string for Firestore ID compatibility

        public string Author { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }  // Use DateTime for Firestore compatibility

        public string Position { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }  // For image upload form handling

        public string ImageUrl { get; set; }  // URL for Firebase Storage image
    }
}
