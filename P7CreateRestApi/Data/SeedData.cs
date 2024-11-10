using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;

namespace Dot.Net.WebApi.Data
{
    public static class SeedData
    {
        const string ADMIN_ROLE_NAME = "Admin";
        const string USER_ROLE_NAME = "User";

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedData.AddRole(roleManager, ADMIN_ROLE_NAME);
            await SeedData.AddRole(roleManager, USER_ROLE_NAME);

            await SeedData.AddUser(userManager, "AdminUser", "admin@findexium.com", "Admin123@", ADMIN_ROLE_NAME);
            await SeedData.AddUser(userManager, "SimpleUser", "user@findexium.com", "User1234@", USER_ROLE_NAME);
        }

        private static async Task AddRole(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var identityRole = new IdentityRole(role);
                var roleResult = await roleManager.CreateAsync(identityRole);

                if (!roleResult.Succeeded)
                {
                    throw new Exception("Failed to create '" + role + "' role");
                }
            }
        }

        private static async Task AddUser(UserManager<User> userManager, string username, string email, string pwd, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    Role = role
                };

                var userResult = await userManager.CreateAsync(user, pwd);

                if (!userResult.Succeeded)
                {
                    throw new Exception("Failed to create '" + username + "' user");
                }

                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}