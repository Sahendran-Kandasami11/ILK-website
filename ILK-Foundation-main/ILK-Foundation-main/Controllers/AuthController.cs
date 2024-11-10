using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using test_ngo.Models;

namespace test_ngo.Controllers
{
    public class AuthController : Controller
    {
        private readonly string firebaseApiKey = "AIzaSyCHeg6_BrgQjNuEjyygjV5SveV9wJkprDo"; 
        private readonly HttpClient httpClient;

        public AuthController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Ensure you are passing a valid RegisterModel to the view
            return View(new RegisterModel());
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if passwords match
                    if (register.Password != register.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                        return View();
                    }

                    // Firebase Authentication API URL for creating users
                    var firebaseUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={firebaseApiKey}";

                    // Create user data
                    var userData = new
                    {
                        email = register.Email,
                        password = register.Password,
                        returnSecureToken = true
                    };

                    var jsonData = JsonConvert.SerializeObject(userData);
                    Console.WriteLine($"Request Data: {jsonData}"); // Log request data for debugging

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Make the HTTP POST request to Firebase Authentication API
                    var response = await httpClient.PostAsync(firebaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // On successful registration, get the response
                        var responseString = await response.Content.ReadAsStringAsync();
                        var firebaseResponse = JsonConvert.DeserializeObject<dynamic>(responseString);

                        string currentUserId = firebaseResponse.localId;

                        if (!string.IsNullOrEmpty(currentUserId))
                        {
                            // Store the UID in session to track logged-in user
                            HttpContext.Session.SetString("currentUser", currentUserId);

                            // Redirect to the desired action after registration
                            return RedirectToAction("Index", "Home"); // Update this with your target action and controller
                        }
                    }
                    else
                    {
                        // Log the error response for debugging
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");

                        // Handle error response
                        ModelState.AddModelError("", $"Registration failed: {errorResponse}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle the exception (e.g., network issues, Firebase API issues)
                ModelState.AddModelError("", "Registration failed. Please try again.");
                Console.WriteLine($"Request failed: {ex.Message}"); // Log the exception message
            }

            return View();
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpGet]
        public IActionResult Login()
        {
            // Make sure you are passing an instance of LoginModel to the view
            return View(new LoginModel());
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Firebase Authentication API URL for logging in
                    var firebaseUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}";

                    // Prepare user data for the login request
                    var userData = new
                    {
                        email = login.Email,
                        password = login.Password,
                        returnSecureToken = true
                    };

                    var jsonData = JsonConvert.SerializeObject(userData);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Send the login request to Firebase
                    var response = await httpClient.PostAsync(firebaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successful login
                        var responseString = await response.Content.ReadAsStringAsync();
                        var firebaseResponse = JsonConvert.DeserializeObject<dynamic>(responseString);

                        string currentUserId = firebaseResponse.localId;

                        if (!string.IsNullOrEmpty(currentUserId))
                        {
                            // Store the UID or email in session to track logged-in user
                            HttpContext.Session.SetString("currentUser", login.Email); // or use currentUserId if needed

                            // Redirect to another page after login (e.g., a dashboard or home page)
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        // Handle failed login (e.g., wrong email or password)
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(errorResponse);
                        ModelState.AddModelError("", $"Login failed: {firebaseEx.error.message}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Login failed. Please try again.");
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ToString());
                ModelState.AddModelError("", $"Firebase error: {firebaseEx.error.message}");
            }

            return View(login);
        }



        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpPost]
        public IActionResult LogOut()
        {
            // Remove the current user session
            HttpContext.Session.Remove("currentUser");
            return RedirectToAction("Login", "Auth");
        }

    }
}
    