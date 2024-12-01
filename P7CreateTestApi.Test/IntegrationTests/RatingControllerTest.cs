using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class RatingControllerTest: GenericControllerTest<Rating>
    {
        public RatingControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<Rating> GetRatings()
        {
            return new List<Rating> {
                new Rating() { Id = 1, MoodysRating = "Moodys1", SandPRating = "Sand1", FitchRating = "Fitch1" },
                new Rating() { Id = 2, MoodysRating = "Moodys2", SandPRating = "Sand4", FitchRating = "Fitch2" },
                new Rating() { Id = 3, MoodysRating = "Moodys3", SandPRating = "Sand3", FitchRating = "Fitch3" },
                new Rating() { Id = 4, MoodysRating = "Moodys4", SandPRating = "Sand4", FitchRating = "Fitch4" }
            };
        }

        private RatingModelAdd GetNewRatingModelAdd()
        {
            return new RatingModelAdd() { FitchRating = "Ficth", MoodysRating = "Mood", SandPRating = "SandP", OrderNumber = 12 };
        }

        private RatingModelUpdate GetRatingModelToUpdate(Rating rating)
        {
            return new RatingModelUpdate()
            {
                FitchRating = rating.FitchRating,
                MoodysRating = rating.MoodysRating,
                SandPRating = rating.SandPRating,
                OrderNumber = rating.OrderNumber
            };
        }

        private async Task SeedSampleRatingsAsync()
        {
            await this.ClearTable();
            await this.FillTable(this.GetRatings());
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var nbRecords = this.NbRecordsInTable();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await response.Content.ReadFromJsonAsync<List<RatingModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(ratings);
            Assert.Equal(nbRecords, ratings.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Rating/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRatingById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var expectedRating = this.GetFirstRecordInTable();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Rating/display/{expectedRating.Id}");
            var rating = await response.Content.ReadFromJsonAsync<RatingModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(rating);
            Assert.Equal(expectedRating.Id, rating.Id);
            Assert.Equal(expectedRating.MoodysRating, rating.MoodysRating);
            Assert.Equal(expectedRating.SandPRating, rating.SandPRating);
            Assert.Equal(expectedRating.FitchRating, rating.FitchRating);
            Assert.Equal(expectedRating.OrderNumber, rating.OrderNumber);
        }

        [Fact]
        public async Task GetRatingById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var expectedRating = this.GetFirstRecordInTable();
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Rating/display/{expectedRating.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRatingById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var nonExistingRatingId = -1;
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Rating/display/{nonExistingRatingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddRating_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var nbRecordsInit = this.NbRecordsInTable();
            var newRating = this.GetNewRatingModelAdd();
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Rating/creation", newRating);
            var nbRecordsAfterAdd = this.NbRecordsInTable();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(nbRecordsAfterAdd, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddRating_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            var newRating = this.GetNewRatingModelAdd();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Rating/creation", newRating);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRating_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            var newRating = this.GetNewRatingModelAdd();
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Rating/creation", newRating);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToUpdate = this.GetFirstRecordInTable();
            var ratingModelUpdate = this.GetRatingModelToUpdate(ratingToUpdate);
            ratingModelUpdate.FitchRating = "UpdatedFitch";
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Rating/update/{ratingToUpdate.Id}", ratingModelUpdate);
            var ratingUpdated = this.GetRecordById(ratingToUpdate.Id);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(ratingUpdated.FitchRating, ratingModelUpdate.FitchRating);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratingToUpdate = this.GetFirstRecordInTable();
            var ratingModelUpdate = this.GetRatingModelToUpdate(ratingToUpdate);
            ratingModelUpdate.FitchRating = "Updated Fitch";
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Rating/update/{ratingToUpdate.Id}", ratingModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToUpdate = this.GetFirstRecordInTable();
            var ratingModelUpdate = this.GetRatingModelToUpdate(ratingToUpdate);
            ratingModelUpdate.FitchRating = "Updated Fitch";
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Rating/update/{ratingToUpdate.Id}", ratingModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToUpdate = this.GetFirstRecordInTable();
            var ratingModelUpdate = this.GetRatingModelToUpdate(ratingToUpdate);
            ratingModelUpdate.FitchRating = "UpdatedFitch";
            var nonExistingRatingId = -1;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Rating/update/{nonExistingRatingId}", ratingModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToDeleteId = this.GetFirstRecordInTable().Id;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Rating/deletion/{ratingToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToDeleteId = this.GetFirstRecordInTable().Id;
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Rating/deletion/{ratingToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var ratingToDeleteId = this.GetFirstRecordInTable().Id;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Rating/deletion/{ratingToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            var nonExistingRatingId = -1;
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Rating/deletion/{nonExistingRatingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
