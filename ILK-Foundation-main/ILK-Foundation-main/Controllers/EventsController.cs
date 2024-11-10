using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using test_ngo.Models;

namespace test_ngo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "your-bucket-name"; // Firebase storage bucket name

        public EventsController()
        {
            _firestoreDb = FirestoreDb.Create("your-project-id");
            _storageClient = StorageClient.Create();
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = new List<Event>();
            var snapshot = await _firestoreDb.Collection("Events").GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                var eventObj = doc.ConvertTo<Event>();
                eventObj.EventID = doc.Id; // Use document ID as EventID
                events.Add(eventObj);
            }
            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var doc = await _firestoreDb.Collection("Events").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
                return NotFound();

            var eventObj = doc.ConvertTo<Event>();
            return View(eventObj);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventObj, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload image to Google Cloud Storage
                if (imageFile != null && imageFile.Length > 0)
                {
                    var objectName = $"event-images/{Guid.NewGuid()}";
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        // Upload image to Google Cloud Storage
                        await _storageClient.UploadObjectAsync(_bucketName, objectName, imageFile.ContentType, memoryStream);
                        eventObj.ImageUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
                    }
                }

                // Add event to Firestore
                await _firestoreDb.Collection("Events").AddAsync(eventObj);
                return RedirectToAction(nameof(Index));
            }
            return View(eventObj);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var doc = await _firestoreDb.Collection("Events").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
                return NotFound();

            var eventObj = doc.ConvertTo<Event>();
            return View(eventObj);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Event eventObj, IFormFile imageFile)
        {
            if (string.IsNullOrEmpty(id) || eventObj.EventID != id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update image if new file is uploaded
                if (imageFile != null && imageFile.Length > 0)
                {
                    var objectName = $"event-images/{Guid.NewGuid()}";
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        // Upload new image to Google Cloud Storage
                        await _storageClient.UploadObjectAsync(_bucketName, objectName, imageFile.ContentType, memoryStream);
                        eventObj.ImageUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
                    }
                }

                // Update event in Firestore
                await _firestoreDb.Collection("Events").Document(id).SetAsync(eventObj, SetOptions.Overwrite);
                return RedirectToAction(nameof(Index));
            }
            return View(eventObj);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var doc = await _firestoreDb.Collection("Events").Document(id).GetSnapshotAsync();
            if (!doc.Exists)
                return NotFound();

            var eventObj = doc.ConvertTo<Event>();
            return View(eventObj);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _firestoreDb.Collection("Events").Document(id).DeleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
