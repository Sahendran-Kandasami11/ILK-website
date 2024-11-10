using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RegisterModel : PageModel
{
    private readonly ILogger<RegisterModel> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public RegisterModel(
        ILogger<RegisterModel> logger,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public List<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    // This method will be invoked on GET requests
    public async Task OnGetAsync(string returnUrl = null)
    {
        // Set the return URL
        ReturnUrl = returnUrl ?? Url.Content("~/");

        // Get the external authentication providers (like Google, Facebook, etc.)
        ExternalLogins = (await _authenticationSchemeProvider.GetAllSchemesAsync())
            .Where(scheme => scheme.DisplayName != null)
            .ToList();
    }

    // This method will be invoked on POST requests when the user submits the registration form
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        // Check if the model state is valid
        if (ModelState.IsValid)
        {
            // Create a new user using the UserManager
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // If registration succeeded, log the user in
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to the return URL or default page
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed; redisplay the form
        return Page();
    }
}
