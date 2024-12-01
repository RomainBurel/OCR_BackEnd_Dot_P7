using Dot.Net.WebApi.Domain;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class RuleNameServiceTest
    {
        private readonly Mock<IRuleNameRepository> _mockRepository;
        private readonly RuleNameService _ruleNameService;

        public RuleNameServiceTest()
        {
            _mockRepository = new Mock<IRuleNameRepository>();
            _ruleNameService = new RuleNameService(_mockRepository.Object);
        }

        private List<RuleName> GetRuleName()
        {
            return new List<RuleName> {
                new RuleName() { Id = 1, Name = "RuleName1", Description = "The rule 1" },
                new RuleName() { Id = 2, Name = "RuleName2", Description = "The rule 2" },
                new RuleName() { Id = 3, Name = "RuleName3", Description = "The rule 3" },
                new RuleName() { Id = 4, Name = "RuleName4", Description = "The rule 4" }
            };
        }

        [Fact]
        public void GetAll_ShouldReturnListOfRuleNameModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetRuleName());

            // Act
            var result = _ruleNameService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, r => r.Id == 1 && r.Name == "RuleName1");
            Assert.Contains(result, r => r.Id == 2 && r.Name == "RuleName2");
            Assert.Contains(result, r => r.Id == 3 && r.Name == "RuleName3");
            Assert.Contains(result, r => r.Id == 4 && r.Name == "RuleName4");
        }

        [Fact]
        public void GetById_ShouldReturnRuleName_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRuleName()[0]);

            // Act
            var result = _ruleNameService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("RuleName1", result.Name);
        }

        [Fact]
        public async Task Add_ShouldCall_RepositoryAdd()
        {
            // Arrange
            var newRuleNameModel = new RuleNameModelAdd { Name = "RuleNameNew" };

            // Act
            _ruleNameService.Add(newRuleNameModel);

            // Assert
            _mockRepository.Verify(repo => repo.Add(It.Is<RuleName>(r => r.Name == "RuleNameNew")), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRuleName()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<RuleName>()));
            var ruleNameUpdated = new RuleNameModelUpdate { Name = "RuleNameUpdated" };

            // Act
            _ruleNameService.Update(1, ruleNameUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<RuleName>(r => r.Name == "RuleNameUpdated")), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById(1)).Returns(GetRuleName()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<RuleName>()));

            // Act
            var ruleNameModel = _ruleNameService.GetById(1);
            _ruleNameService.Delete(1);

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<RuleName>(r => r.Id == 1)), Times.Once);
        }
    }
}
