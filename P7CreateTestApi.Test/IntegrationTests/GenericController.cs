using Dot.Net.WebApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace P7CreateTestApi.Test.IntegrationTests
{
    public class GenericController<T> : IClassFixture<CustomWebApplicationFactory<Program>> where T : class
    {
        protected readonly CustomWebApplicationFactory<Program> _factory;
        protected readonly HttpClient _httpClient;

        public GenericController(CustomWebApplicationFactory<Program> factory)
        {
            this._factory = factory;
            this._httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });
        }

        protected async Task ClearDatabase()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                var dbSet = context.Set<T>();
                context.RemoveRange(dbSet.ToArray());
                await context.SaveChangesAsync();
            }
        }
    }
}
