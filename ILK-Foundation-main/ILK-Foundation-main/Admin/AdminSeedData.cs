using Microsoft.AspNetCore.Identity;

namespace test_ngo.Models
{
    public class AdminSeedData
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Define an array of roles
            string[] roleNames = { "Admin", "User" };

            // Instantiate identity result
            IdentityResult roleResult;

            // Check if roles exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // If role doesn't exist, add it
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create a default Admin user
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };
            // Store the default password
            string adminPassword = "Admin@123";
            // Check if admin exists
            var adminExists = await userManager.FindByEmailAsync("admin@example.com");

            if (adminExists == null)
            {
                // If admin does not exist, add to the database
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
