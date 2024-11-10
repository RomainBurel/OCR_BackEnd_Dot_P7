using Dot.Net.WebApi.Domain;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class BidListServiceTest
    {
        private readonly Mock<IBidListRepository> _mockRepository;
        private readonly BidListService _bidListService;

        public BidListServiceTest()
        {
            _mockRepository = new Mock<IBidListRepository>();
            _bidListService = new BidListService(_mockRepository.Object);
        }

        private List<BidList> GetBidList()
        {
            return new List<BidList> {
                new BidList() { BidListId = 1, Account = "Client1", BidType = "Principal", BidQuantity = 1200d, AskQuantity = 1000d },
                new BidList() { BidListId = 2, Account = "Client2", BidType = "Principal", BidQuantity = 2400d, AskQuantity = 2000d },
                new BidList() { BidListId = 3, Account = "Client3", BidType = "Principal", BidQuantity = 4800d, AskQuantity = 3000d },
                new BidList() { BidListId = 4, Account = "Client4", BidType = "Principal", BidQuantity = 9600d, AskQuantity = 4000d }
            };
        }

        [Fact]
        public void GetAll_ShouldReturnListOfBidListModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetBidList());

            // Act
            var result = _bidListService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, b => b.BidListId == 1 && b.Account == "Client1");
            Assert.Contains(result, b => b.BidListId == 2 && b.Account == "Client2");
            Assert.Contains(result, b => b.BidListId == 3 && b.Account == "Client3");
            Assert.Contains(result, b => b.BidListId == 4 && b.Account == "Client4");
        }

        [Fact]
        public void GetById_ShouldReturnBidList_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetBidList()[0]);

            // Act
            var result = _bidListService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BidListId);
            Assert.Equal("Client1", result.Account);
        }

        [Fact]
        public async Task Add_ShouldCall_RepositoryAdd()
        {
            // Arrange
            var newBidListModel = new BidListModelAdd { Account = "NewAccount", BidType = "NewType" };

            // Act
            _bidListService.Add(newBidListModel);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.Is<BidList>(b => b.Account == "NewAccount" && b.BidType == "NewType")), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetBidList()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<BidList>()));
            var bidListUpdated = new BidListModelUpdate { Account = "UpdatedAccount", BidType = "UpdatedType" };

            // Act
            var bidListModel = _bidListService.GetById(1);
            _bidListService.Update(bidListModel, bidListUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<BidList>(b => b.Account == "UpdatedAccount" && b.BidType == "UpdatedType")), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetBidList()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<BidList>()));

            // Act
            var bidListModel = _bidListService.GetById(1);
            _bidListService.Delete(bidListModel);

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<BidList>(b => b.Account == "Client1")), Times.Once);
        }
    }
}