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

        private RatingModelAdd NewRating()
        {
            return new RatingModelAdd() { FitchRating = "Ficth", MoodysRating = "Mood", SandPRating = "SandP", OrderNumber = 12 };
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
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await response.Content.ReadFromJsonAsync<List<RatingModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(ratings);
            Assert.Equal(this.GetRatings().Count, ratings.Count);
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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var expectedRating = ratings[0];

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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var expectedRating = ratings[0];
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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var nonExistingRatingId = ratings.Max(r => r.Id) + 1;

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
            await this._factory.LoginAsAdmin(this._httpClient);
            var nbRecordsInit = this.GetRatings().Count;
            var newRating = this.NewRating();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Rating/creation", newRating);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(ratings.Count, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddRating_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            await this._factory.LoginAsUser(this._httpClient);
            var newRating = this.NewRating();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Rating/creation", newRating);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRating_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newRating = this.NewRating();

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
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToUpdate = ratings[0];
            var ratingModelUpdate = new RatingModelUpdate()
            {
                FitchRating = ratingToUpdate.FitchRating,
                MoodysRating = ratingToUpdate.MoodysRating,
                SandPRating = ratingToUpdate.SandPRating,
                OrderNumber = ratingToUpdate.OrderNumber
            };
            ratingModelUpdate.FitchRating = "UpdatedFitch";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Rating/update/{ratingToUpdate.Id}", ratingModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/Rating/display/{ratingToUpdate.Id}");
            var ratingUpdated = await responseUpdated.Content.ReadFromJsonAsync<RatingModel>();

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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToUpdate = ratings[0];
            var ratingModelUpdate = new RatingModelUpdate()
            {
                FitchRating = ratingToUpdate.FitchRating,
                MoodysRating = ratingToUpdate.MoodysRating,
                SandPRating = ratingToUpdate.SandPRating,
                OrderNumber = ratingToUpdate.OrderNumber
            };
            ratingModelUpdate.FitchRating = "Updated Fitch";

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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToUpdate = ratings[0];
            var ratingModelUpdate = new RatingModelUpdate()
            {
                FitchRating = ratingToUpdate.FitchRating,
                MoodysRating = ratingToUpdate.MoodysRating,
                SandPRating = ratingToUpdate.SandPRating,
                OrderNumber = ratingToUpdate.OrderNumber
            };
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
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToUpdate = ratings[0];
            var ratingModelUpdate = new RatingModelUpdate()
            {
                FitchRating = ratingToUpdate.FitchRating,
                MoodysRating = ratingToUpdate.MoodysRating,
                SandPRating = ratingToUpdate.SandPRating,
                OrderNumber = ratingToUpdate.OrderNumber
            };
            ratingModelUpdate.FitchRating = "UpdatedFitch";
            var nonExistingRatingId = ratings.Max(r => r.Id) + 1;

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
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToDeleteId = ratings[0].Id;

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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToDeleteId = ratings[0].Id;

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
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var ratingToDeleteId = ratings[0].Id;
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
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await responseAll.Content.ReadFromJsonAsync<List<RatingModel>>();
            var nonExistingRatingId = ratings.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Rating/deletion/{nonExistingRatingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
