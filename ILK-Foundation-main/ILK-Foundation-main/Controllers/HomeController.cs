using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using test_ngo.Models;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.Extensions.Logging;

namespace test_ngo.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // Load credentials from the JSON file
            var credential = GoogleCredential.FromFile("C:\\Users\\User\\OneDrive\\Desktop\\ILK-Foundation-main\\ilkfoundation-firebase-adminsdk-675ei-d599e5fb95.json");

            // Convert to ChannelCredentials
            var channelCredentials = credential.ToChannelCredentials();

            // Create a FirestoreClient with the channel credentials
            var firestoreClient = new FirestoreClientBuilder
            {
                ChannelCredentials = channelCredentials
            }.Build();

            // Initialize FirestoreDb with the FirestoreClient
            _firestoreDb = FirestoreDb.Create("ilkfoundation", firestoreClient);
        }

        public IActionResult Index()
        {
            return View();
        }


        // Method for saving the Testimonial with Image URL to Firestore
        public async Task<IActionResult> AddTestimonial(Testimonial model)
        {
            if (model.ImageFile != null)
            {
                var imageUrl = await UploadImageToFirebaseStorage(model.ImageFile);
                model.ImageUrl = imageUrl;
            }

            var testimonialRef = _firestoreDb.Collection("Testimonials").Document();
            await testimonialRef.SetAsync(model);

            return RedirectToAction("Index");
        }

        // Method for saving the Event with Image URL to Firestore
        public async Task<IActionResult> AddEvent(Event model)
        {
            if (model.ImageFile != null)
            {
                var imageUrl = await UploadImageToFirebaseStorage(model.ImageFile);
                model.ImageUrl = imageUrl;
            }

            var eventRef = _firestoreDb.Collection("Events").Document();
            await eventRef.SetAsync(model);

            return RedirectToAction("Index");
        }

        // Method to upload image to Firebase Storage and get URL
        private async Task<string> UploadImageToFirebaseStorage(IFormFile imageFile)
        {
            var storageClient = StorageClient.Create();
            var bucket = storageClient.GetBucket("ilkfoundation.appspot.com"); // Replace with your bucket name
            var fileName = Path.GetFileName(imageFile.FileName);

            using (var stream = imageFile.OpenReadStream())
            {
                var uploadObjectOptions = new UploadObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead
                };

                var uploadedObject = await storageClient.UploadObjectAsync(
                    bucket.Name, fileName, imageFile.ContentType, stream, options: uploadObjectOptions);
                return uploadedObject.MediaLink; // The URL of the uploaded image
            }
        }
    }
}
