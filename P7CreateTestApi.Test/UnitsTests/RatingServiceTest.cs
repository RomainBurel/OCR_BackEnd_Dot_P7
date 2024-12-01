using Dot.Net.WebApi.Domain;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class RatingServiceTest
    {
        private readonly Mock<IRatingRepository> _mockRepository;
        private readonly RatingService _ratingService;

        public RatingServiceTest()
        {
            _mockRepository = new Mock<IRatingRepository>();
            _ratingService = new RatingService(_mockRepository.Object);
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

        [Fact]
        public void GetAll_ShouldReturnListOfRatingModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetRating());

            // Act
            var result = _ratingService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, r => r.Id == 1 && r.MoodysRating == "Moodys1");
            Assert.Contains(result, r => r.Id == 2 && r.MoodysRating == "Moodys2");
            Assert.Contains(result, r => r.Id == 3 && r.MoodysRating == "Moodys3");
            Assert.Contains(result, r => r.Id == 4 && r.MoodysRating == "Moodys4");
        }

        [Fact]
        public void GetById_ShouldReturnRating_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRating()[0]);

            // Act
            var result = _ratingService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Moodys1", result.MoodysRating);
        }

        [Fact]
        public async Task Add_ShouldCall_RepositoryAdd()
        {
            // Arrange
            var newRatingModel = new RatingModelAdd { MoodysRating = "MoodysNew" };

            // Act
            _ratingService.Add(newRatingModel);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.Is<Rating>(r => r.MoodysRating == "MoodysNew")), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRating()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Rating>()));
            var ratingUpdated = new RatingModelUpdate { MoodysRating = "MoodysUpdated" };

            // Act
            _ratingService.Update(1, ratingUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<Rating>(r => r.MoodysRating == "MoodysUpdated")), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRating()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<Rating>()));

            // Act
            var ratingModel = _ratingService.GetById(1);
            _ratingService.Delete(1);

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<Rating>(r => r.Id == 1)), Times.Once);
        }
    }
}
