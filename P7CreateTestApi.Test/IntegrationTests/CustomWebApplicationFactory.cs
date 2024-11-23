using Dot.Net.WebApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P7CreateRestApi.Models;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LocalDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                if (dbConnectionDescriptor != null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                services.AddDbContext<LocalDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDB");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    SeedData.SeedAdminUserAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                }
            });

            builder.UseEnvironment("Development");
        }

        public async Task LoginAsAdmin(HttpClient httpClient)
        {
            await AuthenticateUserAsync(httpClient, SeedData.ADMIN_MAIL, SeedData.ADMIN_PWD);
        }

        public async Task LoginAsUser(HttpClient httpClient)
        {
            await AuthenticateUserAsync(httpClient, SeedData.USER_MAIL, SeedData.USER_PWD);
        }

        public void Logout(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = null; 
        }

        private async Task AuthenticateUserAsync(HttpClient httpClient, string email, string pwd)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync(httpClient, email, pwd));
        }

        private async Task<string> GetJwtAsync(HttpClient httpClient, string email, string pwd)
        {
            var loginModel = new LoginModel
            {
                Email = email,
                Password = pwd
            };

            var response = await httpClient.PostAsJsonAsync("/Login/login", loginModel);
            response.EnsureSuccessStatusCode();

            var registrationResponse = await response.Content.ReadAsStringAsync();
            using (var document = JsonDocument.Parse(registrationResponse))
            {
                var json = document.RootElement.GetProperty("token").GetString();
                return json;
            }
        }
    }
}
