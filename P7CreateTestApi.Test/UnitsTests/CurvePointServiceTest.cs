using Dot.Net.WebApi.Domain;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class CurvePointServiceTest
    {
        private readonly Mock<ICurvePointRepository> _mockRepository;
        private readonly CurvePointService _curvePointService;

        public CurvePointServiceTest()
        {
            _mockRepository = new Mock<ICurvePointRepository>();
            _curvePointService = new CurvePointService(_mockRepository.Object);
        }

        private List<CurvePoint> GetCurvePoint()
        {
            return new List<CurvePoint> {
                new CurvePoint() { Id = 1, CurveId = 1, Term = 1.0, CreationDate = DateTime.Now },
                new CurvePoint() { Id = 2, CurveId = 2, Term = 1.2, CreationDate = DateTime.Now },
                new CurvePoint() { Id = 3, CurveId = 3, Term = 1.4, CreationDate = DateTime.Now },
                new CurvePoint() { Id = 4, CurveId = 4, Term = 1.6, CreationDate = DateTime.Now }
            };
        }

        [Fact]
        public void GetAll_ShouldReturnListOfCurvePointModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetCurvePoint());

            // Act
            var result = _curvePointService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, b => b.Id == 1 && b.Term == 1.0);
            Assert.Contains(result, b => b.Id == 2 && b.Term == 1.2);
            Assert.Contains(result, b => b.Id == 3 && b.Term == 1.4);
            Assert.Contains(result, b => b.Id == 4 && b.Term == 1.6);
        }

        [Fact]
        public void GetById_ShouldReturnCurvePoint_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetCurvePoint()[0]);

            // Act
            var result = _curvePointService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1.0, result.Term);
        }

        [Fact]
        public async Task Add_ShouldCall_RepositoryAdd()
        {
            // Arrange
            var newCurvePointModel = new CurvePointModelAdd { Term = 3.2, CurvePointValue = 3.1 };

            // Act
            _curvePointService.Add(newCurvePointModel);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.Is<CurvePoint>(b => b.Term == 3.2 && b.CurvePointValue == 3.1)), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetCurvePoint()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<CurvePoint>()));
            var curvePointUpdated = new CurvePointModelUpdate { Term = 3.2, CurvePointValue = 3.1 };

            // Act
            var curvePointModel = _curvePointService.GetById(1);
            _curvePointService.Update(curvePointModel, curvePointUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<CurvePoint>(b => b.Term == 3.2 && b.CurvePointValue == 3.1)), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetCurvePoint()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<CurvePoint>()));

            // Act
            var curvePointModel = _curvePointService.GetById(1);
            _curvePointService.Delete(curvePointModel);

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<CurvePoint>(b => b.Id == 1)), Times.Once);
        }
    }
}
