using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;

namespace test_ngo.Models
{
    [FirestoreData]  // This attribute marks the class for Firestore mapping
    public class PastEvent
    {
        [FirestoreProperty]  // Maps this property to Firestore
        public string EventId { get; set; }  // Fixed the property name to match the controller

        [FirestoreProperty]  // Maps this property to Firestore
        public string Title { get; set; }

        [FirestoreProperty]  // Maps this property to Firestore
        public string Description { get; set; }

        [FirestoreProperty]  // Maps this property to Firestore
        public DateTime EventDate { get; set; }

        [FirestoreProperty]  // Maps this property to Firestore
        public string ImageUrl { get; set; }  // Store the image URL instead of raw data

        // This property will not be stored in Firestore, as we don't need to map it
        public IFormFile ImageFile { get; set; }  // This is not mapped to Firestore
    }
}
