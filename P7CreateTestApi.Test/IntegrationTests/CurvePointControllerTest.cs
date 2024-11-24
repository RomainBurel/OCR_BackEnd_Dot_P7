using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class CurvePointControllerTest : GenericControllerTest<CurvePoint>
    {
        public CurvePointControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<CurvePoint> GetCurvePoints()
        {
            return new List<CurvePoint> {
                new CurvePoint() { Id = 1, CurveId = 1, Term = 1.0, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.0 },
                new CurvePoint() { Id = 2, CurveId = 2, Term = 1.2, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.2 },
                new CurvePoint() { Id = 3, CurveId = 3, Term = 1.4, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.4 },
                new CurvePoint() { Id = 4, CurveId = 4, Term = 1.6, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.6 }
            };
        }

        private CurvePointModelAdd GetNewCurvePointModelAdd()
        {
            return new CurvePointModelAdd() { CurveId = 5, Term = 1.8, CreationDate = DateTime.Now, AsOfDate = DateTime.Now, CurvePointValue = 3.8 };
        }

        private CurvePointModelUpdate GetCurvePointModelToUpdate(CurvePoint curvePoint)
        {
            return new CurvePointModelUpdate() { CurveId = curvePoint.CurveId, Term = curvePoint.Term };
        }

        private async Task SeedSampleCurvePointsAsync()
        {
            await this.ClearTable();
            await this.FillTable(this.GetCurvePoints());
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            var nbRecords = this.NbRecordsInTable();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Curve/list");
            var curvePoints = await response.Content.ReadFromJsonAsync<List<CurvePointModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(curvePoints);
            Assert.Equal(nbRecords, curvePoints.Count);
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
            var expectedCurvePoint = this.GetFirstRecordInTable();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Curve/display/{expectedCurvePoint.Id}");
            var curvePoint = await response.Content.ReadFromJsonAsync<CurvePointModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
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
            var expectedCurvePoint = this.GetFirstRecordInTable();
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
            var nonExistingCurvePointId = -1;
            await this._factory.LoginAsUser(this._httpClient);

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
            var nbRecordsInit = this.NbRecordsInTable();
            var newCurvePoint = this.GetNewCurvePointModelAdd();
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Curve/creation", newCurvePoint);
            var nbRecordsAfterAdd = this.NbRecordsInTable();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(nbRecordsAfterAdd, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            var newCurvePoint = this.GetNewCurvePointModelAdd();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Curve/creation", newCurvePoint);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddCurvePoint_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            var newCurvePoint = this.GetNewCurvePointModelAdd();
            this._factory.Logout(this._httpClient);

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
            var curvePointToUpdate = this.GetFirstRecordInTable();
            var curvePointModelUpdate = this.GetCurvePointModelToUpdate(curvePointToUpdate);
            curvePointModelUpdate.Term = 5.0;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Curve/update/{curvePointToUpdate.Id}", curvePointModelUpdate);
            var curvePointUpdated = this.GetRecordById(curvePointToUpdate.Id);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(curvePointUpdated.Term, curvePointModelUpdate.Term);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            var curvePointToUpdate = this.GetFirstRecordInTable();
            var curvePointModelUpdate = this.GetCurvePointModelToUpdate(curvePointToUpdate);
            curvePointModelUpdate.Term = 5.0;
            await this._factory.LoginAsUser(this._httpClient);

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
            var curvePointToUpdate = this.GetFirstRecordInTable();
            var curvePointModelUpdate = this.GetCurvePointModelToUpdate(curvePointToUpdate);
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
            var responseAll = await this._httpClient.GetAsync("/Curve/list");
            var curvePointToUpdate = this.GetFirstRecordInTable();
            var curvePointModelUpdate = this.GetCurvePointModelToUpdate(curvePointToUpdate);
            curvePointModelUpdate.Term = 5.0;
            var nonExistingCurvePointId = -1;
            await this._factory.LoginAsAdmin(this._httpClient);

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
            var curvePointToDeleteId = this.GetFirstRecordInTable().Id;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{curvePointToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleCurvePointsAsync();
            var curvePointToDeleteId = this.GetFirstRecordInTable().Id;
            await this._factory.LoginAsUser(this._httpClient);

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
            var curvePointToDeleteId = this.GetFirstRecordInTable().Id;
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
            var nonExistingCurvePointId = -1;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Curve/deletion/{nonExistingCurvePointId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
