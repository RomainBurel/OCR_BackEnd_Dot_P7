using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    public class RatingControllerTest: GenericController<Rating>
    {
        public RatingControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<Rating> GetRating()
        {
            return new List<Rating> {
                new Rating() { Id = 1, MoodysRating = "Moodys1", SandPRating = "Sand1", FitchRating = "Fitch1" },
                new Rating() { Id = 2, MoodysRating = "Moodys2", SandPRating = "Sand4", FitchRating = "Fitch2" },
                new Rating() { Id = 3, MoodysRating = "Moodys3", SandPRating = "Sand3", FitchRating = "Fitch3" },
                new Rating() { Id = 4, MoodysRating = "Moodys4", SandPRating = "Sand4", FitchRating = "Fitch4" }
            };
        }

        private async Task SeedSampleRatingsAsync()
        {
            await this.ClearDatabase();
            await this._factory.LoginAsAdmin(this._httpClient);

            var ratings = GetRating();
            foreach (var rating in ratings)
            {
                await this._httpClient.PostAsJsonAsync("/Rating/creation", new RatingModelAdd
                {
                    MoodysRating = rating.MoodysRating,
                    SandPRating = rating.SandPRating,
                    FitchRating = rating.FitchRating,
                    OrderNumber = rating.OrderNumber
                });
            }

            this._factory.LogoutUser(_httpClient);
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleRatingsAsync();
            await _factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Rating/list");
            var ratings = await response.Content.ReadFromJsonAsync<List<RatingModel>>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(ratings);
            Assert.Equal(this.GetRating().Count, ratings.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.LogoutUser(this._httpClient);

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
            await _factory.LoginAsUser(this._httpClient);
            var expectedRating = this.GetRating()[0];

            // Act
            var response = await this._httpClient.GetAsync($"/Rating/display/{expectedRating.Id}");
            var rating = await response.Content.ReadFromJsonAsync<RatingModel>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(rating);
            Assert.Equal(expectedRating.Id, rating.Id);
            Assert.Equal(expectedRating.MoodysRating, rating.MoodysRating);
            Assert.Equal(expectedRating.SandPRating, rating.SandPRating);
            Assert.Equal(expectedRating.FitchRating, rating.FitchRating);
            Assert.Equal(expectedRating.OrderNumber, rating.OrderNumber);
        }
    }
}
