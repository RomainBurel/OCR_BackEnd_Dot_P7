using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;

namespace Dot.Net.WebApi.Data
{
    public static class SeedData
    {
        public const string ADMIN_MAIL = "admin@findexium.com";
        public const string USER_MAIL = "user@findexium.com";
        public const string ADMIN_PWD = "Admin123@";
        public const string USER_PWD = "User1234@";
        const string ADMIN_ROLE_NAME = "Admin";
        const string USER_ROLE_NAME = "User";

        public static async Task SeedAdminAndSimpleUsersAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<LocalDbContext>();
            db.Database.EnsureCreated();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedData.AddRole(roleManager, ADMIN_ROLE_NAME);
            await SeedData.AddRole(roleManager, USER_ROLE_NAME);

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            await SeedData.AddUser(userManager, "AdminUser", ADMIN_MAIL, ADMIN_PWD, ADMIN_ROLE_NAME);
            await SeedData.AddUser(userManager, "SimpleUser", USER_MAIL, USER_PWD, USER_ROLE_NAME);
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

        public static async Task AddUser(UserManager<User> userManager, string userName, string email, string pwd, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = email,
                    FullName = userName,
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(user, pwd);

                if (!userResult.Succeeded)
                {
                    throw new Exception("Failed to create '" + userName + "' user");
                }

                await userManager.AddToRoleAsync(user, role);
            }
        }

        public static async Task RemoveUser(UserManager<User> userManager, string email, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await userManager.RemoveFromRoleAsync(user, role);
                await userManager.DeleteAsync(user);
            }
        }
    }
}