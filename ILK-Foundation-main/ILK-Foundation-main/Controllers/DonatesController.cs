using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test_ngo.Models;
using Newtonsoft.Json;

namespace test_ngo.Controllers
{
    public class DonatesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseBaseUrl = "https://ilkfoundation.firebaseio.com/";

        public DonatesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Donates
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_firebaseBaseUrl}donates.json");
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var donations = JsonConvert.DeserializeObject<Dictionary<string, Donate>>(json);
                return View(donations?.Values);
            }
            return View(new List<Donate>());
        }

        // GET: Donates/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"{_firebaseBaseUrl}donates/{id}.json");
            var json = await response.Content.ReadAsStringAsync();
            var donate = JsonConvert.DeserializeObject<Donate>(json);

            if (donate == null)
            {
                return NotFound();
            }

            return View(donate);
        }

        // GET: Donates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Donates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Name,Surname,Email,Date,Amount")] Donate donate)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{_firebaseBaseUrl}donates.json", donate);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(donate);
        }

        // GET: Donates/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"{_firebaseBaseUrl}donates/{id}.json");
            var json = await response.Content.ReadAsStringAsync();
            var donate = JsonConvert.DeserializeObject<Donate>(json);

            if (donate == null)
            {
                return NotFound();
            }

            return View(donate);
        }

        // POST: Donates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DonateID,UserID,Name,Surname,Email,Date,Amount")] Donate donate)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"{_firebaseBaseUrl}donates/{id}.json", donate);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(donate);
        }

        // GET: Donates/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"{_firebaseBaseUrl}donates/{id}.json");
            var json = await response.Content.ReadAsStringAsync();
            var donate = JsonConvert.DeserializeObject<Donate>(json);

            if (donate == null)
            {
                return NotFound();
            }

            return View(donate);
        }

        // POST: Donates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_firebaseBaseUrl}donates/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
