using Dot.Net.WebApi.Domain;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class TradeServiceTest
    {
        private readonly Mock<ITradeRepository> _mockRepository;
        private readonly TradeService _tradeService;

        public TradeServiceTest()
        {
            _mockRepository = new Mock<ITradeRepository>();
            _tradeService = new TradeService(_mockRepository.Object);
        }

        private List<Trade> GetTrade()
        {
            return new List<Trade> {
                new Trade() { TradeId = 1, Account = "Client1" },
                new Trade() { TradeId = 2, Account = "Client2" },
                new Trade() { TradeId = 3, Account = "Client3" },
                new Trade() { TradeId = 4, Account = "Client4" }
            };
        }

        [Fact]
        public void GetAll_ShouldReturnListOfTradeModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetTrade());

            // Act
            var result = _tradeService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, t => t.TradeId == 1 && t.Account == "Client1");
            Assert.Contains(result, t => t.TradeId == 2 && t.Account == "Client2");
            Assert.Contains(result, t => t.TradeId == 3 && t.Account == "Client3");
            Assert.Contains(result, t => t.TradeId == 4 && t.Account == "Client4");
        }

        [Fact]
        public void GetById_ShouldReturnTrade_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetTrade()[0]);

            // Act
            var result = _tradeService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TradeId);
            Assert.Equal("Client1", result.Account);
        }

        [Fact]
        public async Task Add_ShouldCall_RepositoryAdd()
        {
            // Arrange
            var newTradeModel = new TradeModelAdd { Account = "ClientNew" };

            // Act
            _tradeService.Add(newTradeModel);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.Is<Trade>(t => t.Account == "ClientNew")), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetTrade()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Trade>()));
            var tradeUpdated = new TradeModelUpdate { Account = "ClientUpdated" };

            // Act
            _tradeService.Update(1, tradeUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<Trade>(t => t.Account == "ClientUpdated")), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetTrade()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<Trade>()));

            // Act
            var tradeModel = _tradeService.GetById(1);
            _tradeService.Delete(1);

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<Trade>(t => t.TradeId == 1)), Times.Once);
        }
    }
}
