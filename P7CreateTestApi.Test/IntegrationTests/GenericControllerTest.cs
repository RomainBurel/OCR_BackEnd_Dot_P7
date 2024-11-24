using Dot.Net.WebApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace P7CreateTestApi.Test.IntegrationTests
{
    public class GenericControllerTest<T> : IClassFixture<CustomWebApplicationFactory<Program>> where T : class
    {
        private static bool TestInProgress = false;

        protected readonly CustomWebApplicationFactory<Program> _factory;
        protected readonly HttpClient _httpClient;

        public GenericControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            this._factory = factory;
            this._httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });
        }

        protected async Task FillTable(List<T> records)
        {
            using (var scope = this._factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                var dbSet = context.Set<T>();
                dbSet.AddRange(records);
                await context.SaveChangesAsync();
            }
        }

        protected int NbRecordsInTable()
        {
            using (var scope = this._factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                var dbSet = context.Set<T>();
                return dbSet.Count();
            }
        }

        protected virtual async Task ClearTable()
        {
            using (var scope = this._factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                var dbSet = context.Set<T>();
                context.RemoveRange(dbSet.ToArray());
                await context.SaveChangesAsync();
            }
        }
    }
}
