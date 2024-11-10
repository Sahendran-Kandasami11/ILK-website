using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using test_ngo.Models;
using Newtonsoft.Json;

namespace test_ngo.Controllers
{
    [Authorize]
    public class VolunteersController : Controller
    {
        private readonly FirestoreDb _firestore;
        private readonly HttpClient _httpClient;
        private readonly string _firebaseStorageUrl = "https://firebasestorage.googleapis.com/v0/b/ilkfoundation.appspot.com/o";

        public VolunteersController(FirestoreDb firestore)
        {
            _firestore = firestore;
            _httpClient = new HttpClient();
        }

        // GET: Volunteers
        public async Task<IActionResult> Index()
        {
            var volunteersSnapshot = await _firestore.Collection("Volunteers").GetSnapshotAsync();
            var volunteers = volunteersSnapshot.Documents.Select(doc => doc.ConvertTo<Volunteer>()).ToList();
            return View(volunteers);
        }

        public IActionResult Volunteer()
        {
            return View();
        }


        // GET: Volunteers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("Volunteers").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var volunteer = doc.ConvertTo<Volunteer>();
            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Volunteers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Volunteer volunteer, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Upload image to Firebase Storage and get the URL
                    string imageUrl = await UploadImageToFirebase(imageFile);
                    volunteer.ImageUrl = imageUrl;  // Set the URL in the model
                }

                // Save volunteer to Firestore
                DocumentReference newDocRef = await _firestore.Collection("Volunteers").AddAsync(volunteer);
                volunteer.VolunteerID = newDocRef.Id;  // Store Firestore ID for reference

                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("Volunteers").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var volunteer = doc.ConvertTo<Volunteer>();
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Volunteer volunteer, IFormFile imageFile)
        {
            if (id != volunteer.VolunteerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Upload image to Firebase Storage and get the URL
                    string imageUrl = await UploadImageToFirebase(imageFile);
                    volunteer.ImageUrl = imageUrl;  // Set the URL in the model
                }

                await _firestore.Collection("Volunteers").Document(id).SetAsync(volunteer);
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("Volunteers").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var volunteer = doc.ConvertTo<Volunteer>();
            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestore.Collection("Volunteers").Document(id).DeleteAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to upload image to Firebase Storage and return its URL using HttpClient
        private async Task<string> UploadImageToFirebase(IFormFile imageFile)
        {
            // Ensure the image file is not null and has content
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new Exception("No image file provided");
            }

            // Generate a unique filename (e.g., using GUID) to avoid filename collisions
            string fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";

            // Create a stream for the file to upload
            using (var stream = imageFile.OpenReadStream())
            {
                // Prepare the content for the HTTP request
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(stream), "file", fileName }
                };

                // Set the request headers
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                // Perform the upload request to Firebase Storage via HTTP
                var uploadUrl = $"{_firebaseStorageUrl}/{fileName}?uploadType=media";
                var response = await _httpClient.PostAsync(uploadUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response to get the file URL
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string imageUrl = result?.mediaLink;
                    return imageUrl;
                }
                else
                {
                    throw new Exception("Failed to upload image to Firebase Storage");
                }
            }
        }
    }
}
