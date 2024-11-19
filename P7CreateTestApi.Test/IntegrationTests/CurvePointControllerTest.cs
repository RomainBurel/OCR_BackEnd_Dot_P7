using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class CurvePointControllerTest : GenericController<CurvePoint>
    {
        public CurvePointControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<CurvePoint> GetCurvePoint()
        {
            return new List<CurvePoint> {
                new CurvePoint() { Id = 1, CurveId = 1, Term = 1.0, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.0 },
                new CurvePoint() { Id = 2, CurveId = 2, Term = 1.2, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.2 },
                new CurvePoint() { Id = 3, CurveId = 3, Term = 1.4, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.4 },
                new CurvePoint() { Id = 4, CurveId = 4, Term = 1.6, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.6 }
            };
        }

        private CurvePointModelAdd NewCurvePoint()
        {
            return new CurvePointModelAdd() { CurveId = 5, Term = 1.8, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.8 };
        }

        private async Task SeedSampleCurvePointsAsync()
        {
            await this.ClearDatabase();
            await this._factory.LoginAsAdmin(this._httpClient);

            var curvePoints = GetCurvePoint();
            foreach (var curvePoint in curvePoints)
            {
                await this._httpClient.PostAsJsonAsync("/Curve/creation", new CurvePointModelAdd
                {
                    CurveId = curvePoint.CurveId,
                    Term = curvePoint.Term,
                    CreationDate = curvePoint.CreationDate,
                    AsOfDate = curvePoint.AsOfDate,
                    CurvePointValue = curvePoint.CurvePointValue
                });
            }

            this._factory.Logout(_httpClient);
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await response.Content.ReadFromJsonAsync<List<CurvePointModel>>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(curvePoints);
            Assert.Equal(this.GetCurvePoint().Count, curvePoints.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Curve/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCurvePointById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var expectedCurvePoint = curvePoints[0];

            // Act
            var response = await this._httpClient.GetAsync($"/Curve/display/{expectedCurvePoint.Id}");
            var curvePoint = await response.Content.ReadFromJsonAsync<CurvePointModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(curvePoint);
            Assert.Equal(expectedCurvePoint.Id, curvePoint.Id);
            Assert.Equal(expectedCurvePoint.CurveId, curvePoint.CurveId);
            Assert.Equal(expectedCurvePoint.Term, curvePoint.Term);
        }

        [Fact]
        public async Task GetCurvePointById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var expectedCurvePoint = curvePoints[0];
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Curve/display/{expectedCurvePoint.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCurvePointById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var nonExistingCurvePointId = curvePoints.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.GetAsync($"/Curve/display/{nonExistingCurvePointId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddCurvePoint_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var nbRecordsInit = this.GetCurvePoint().Count;
            var newCurvePoint = this.NewCurvePoint();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Curve/creation", newCurvePoint);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(curvePoints.Count, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            await this._factory.LoginAsUser(this._httpClient);
            var newCurvePoint = this.NewCurvePoint();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Curve/creation", newCurvePoint);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddCurvePoint_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newCurvePoint = this.NewCurvePoint();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Curve/creation", newCurvePoint);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToUpdate = curvePoints[0];
            var curvePointModelUpdate = new CurvePointModelUpdate()
            {
                CurveId = curvePointToUpdate.CurveId,
                Term = curvePointToUpdate.Term
            };
            curvePointModelUpdate.Term = 5.0;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Curve/update/{curvePointToUpdate.Id}", curvePointModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/Curve/display/{curvePointToUpdate.Id}");
            var curvePointUpdated = await responseUpdated.Content.ReadFromJsonAsync<CurvePointModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(curvePointUpdated.Term, curvePointModelUpdate.Term);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToUpdate = curvePoints[0];
            var curvePointModelUpdate = new CurvePointModelUpdate()
            {
                CurveId = curvePointToUpdate.CurveId,
                Term = curvePointToUpdate.Term
            };
            curvePointModelUpdate.Term = 5.0;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Curve/update/{curvePointToUpdate.Id}", curvePointModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToUpdate = curvePoints[0];
            var curvePointModelUpdate = new CurvePointModelUpdate()
            {
                CurveId = curvePointToUpdate.CurveId,
                Term = curvePointToUpdate.Term
            };
            curvePointModelUpdate.Term = 5.0;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Curve/update/{curvePointToUpdate.Id}", curvePointModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToUpdate = curvePoints[0];
            var curvePointModelUpdate = new CurvePointModelUpdate()
            {
                CurveId = curvePointToUpdate.CurveId,
                Term = curvePointToUpdate.Term
            };
            curvePointModelUpdate.Term = 5.0;
            var nonExistingCurvePointId = curvePoints.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Curve/update/{nonExistingCurvePointId}", curvePointModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToDeleteId = curvePoints[0].Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{curvePointToDeleteId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToDeleteId = curvePoints[0].Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{curvePointToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var curvePointToDeleteId = curvePoints[0].Id;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{curvePointToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await responseAll.Content.ReadFromJsonAsync<List<CurvePointModel>>();
            var nonExistingCurvePointId = curvePoints.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{nonExistingCurvePointId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
