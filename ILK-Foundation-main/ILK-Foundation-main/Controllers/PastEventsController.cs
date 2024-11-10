using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using test_ngo.Models;
using Newtonsoft.Json;
using test_ngo.Data.Migrations;

namespace test_ngo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PastEventsController : Controller
    {
        private readonly FirestoreDb _firestore;
        private readonly HttpClient _httpClient;
        private readonly string _firebaseStorageUrl = "https://firebasestorage.googleapis.com/v0/b/ilkfoundation.appspot.com/o";

        public PastEventsController(FirestoreDb firestore)
        {
            _firestore = firestore;
            _httpClient = new HttpClient();
        }

        // GET: PastEvents
        public async Task<IActionResult> Index()
        {
            var eventsSnapshot = await _firestore.Collection("PastEvents").GetSnapshotAsync();
            var events = eventsSnapshot.Documents.Select(doc => doc.ConvertTo<PastEvent>()).ToList();
            return View(events);
        }

        // GET: PastEvents/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("PastEvents").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var pastEvent = doc.ConvertTo<PastEvent>();
            return View(pastEvent);
        }

        // GET: PastEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PastEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PastEvent pastEvent, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Upload image to Firebase Storage and get the URL
                    string imageUrl = await UploadImageToFirebase(imageFile);
                    pastEvent.ImageUrl = imageUrl;
                }

                DocumentReference newDocRef = await _firestore.Collection("PastEvents").AddAsync(pastEvent);
                pastEvent.EventId = newDocRef.Id;  // Using the correct property name

                return RedirectToAction(nameof(Index));
            }

            return View(pastEvent);
        }

        // GET: PastEvents/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("PastEvents").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var pastEvent = doc.ConvertTo<PastEvent>();
            return View(pastEvent);
        }

        // POST: PastEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PastEvent pastEvent, IFormFile imageFile)
        {
            if (id != pastEvent.EventId)  // Using the correct property name
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string imageUrl = await UploadImageToFirebase(imageFile);
                    pastEvent.ImageUrl = imageUrl;
                }

                await _firestore.Collection("PastEvents").Document(id).SetAsync(pastEvent);
                return RedirectToAction(nameof(Index));
            }

            return View(pastEvent);
        }

        // GET: PastEvents/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doc = await _firestore.Collection("PastEvents").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
            {
                return NotFound();
            }

            var pastEvent = doc.ConvertTo<PastEvent>();
            return View(pastEvent);
        }

        // POST: PastEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestore.Collection("PastEvents").Document(id).DeleteAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to upload image to Firebase Storage and return its URL
        private async Task<string> UploadImageToFirebase(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new Exception("No image file provided");
            }

            string fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";

            using (var stream = imageFile.OpenReadStream())
            {
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(stream), "file", fileName }
                };

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                var uploadUrl = $"{_firebaseStorageUrl}/{fileName}?uploadType=media";
                var response = await _httpClient.PostAsync(uploadUrl, content);

                if (response.IsSuccessStatusCode)
                {
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
