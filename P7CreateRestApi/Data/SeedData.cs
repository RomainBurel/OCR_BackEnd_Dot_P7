using Microsoft.AspNetCore.Identity;
using Dot.Net.WebApi.Domain;

namespace Dot.Net.WebApi.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                var roleResult = await roleManager.CreateAsync(adminRole);

                if (!roleResult.Succeeded)
                {
                    throw new Exception("Failed to create 'Admin' role");
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@findexium.com");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@findexium.com",
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(adminUser, "Admin123@");

                if (!userResult.Succeeded)
                {
                    throw new Exception("Failed to create 'Admin' user");
                }

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}