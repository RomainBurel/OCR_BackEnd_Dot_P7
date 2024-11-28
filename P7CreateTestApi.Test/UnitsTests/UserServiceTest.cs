using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;

namespace P7CreateTestApi.Test.UnitsTests
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _mockRepository = new Mock<IUserRepository>();
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            _userService = new UserService(_mockRepository.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object);
        }

        private List<User> GetUser()
        {
            return new List<User> {
                new User() { Id = "1", UserName = "User1" },
                new User() { Id = "2", UserName = "User2" },
                new User() { Id = "3", UserName = "User3" },
                new User() { Id = "4", UserName = "User4" }
            };
        }

        [Fact]
        public void GetAll_ShouldReturnListOfUserModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(GetUser());
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new string[] { "User" });

            // Act
            var result = _userService.GetAll();

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, u => u.Id == "1" && u.UserName == "User1");
            Assert.Contains(result, u => u.Id == "2" && u.UserName == "User2");
            Assert.Contains(result, u => u.Id == "3" && u.UserName == "User3");
            Assert.Contains(result, u => u.Id == "4" && u.UserName == "User4");
        }

        [Fact]
        public void GetById_ShouldReturnUser_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById("1")).Returns(GetUser()[0]);
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new string[] { "User" });

            // Act
            var result = _userService.GetById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("User1", result.UserName);
        }

        [Fact]
        public async Task Add_ShouldCall_UserManagerCreateAndAddRole()
        {
            // Arrange
            var newUserModel = new UserModelAdd { Email = "usernew@findexium.com", UserName = "UserNew", FullName = "UserNewFullName", Password = "UserNew123@" }; 
            _mockUserManager.Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), newUserModel.Password)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<User>(), "User")).ReturnsAsync(IdentityResult.Success);

            // Act
            _userService.Add(newUserModel);

            // Assert
            _mockUserManager.Verify(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(userManager => userManager.AddToRoleAsync(It.IsAny<User>(), "User"), Times.Once);
        }

        [Fact]
        public void Update_ShouldCall_RepositoryUpdate()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById("1")).Returns(GetUser()[0]);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            var userUpdated = new UserModelUpdate { UserName = "UserUpdated" };

            // Act
            _userService.Update("1", userUpdated);

            // Assert
            _mockRepository.Verify(repo => repo.Update(It.Is<User>(u => u.UserName == "UserUpdated")), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCall_RepositoryRemove()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetById("1")).Returns(GetUser()[0]);
            _mockRepository.Setup(repo => repo.Remove(It.IsAny<User>()));
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new string[] { "User" });

            // Act
            var userModel = _userService.GetById("1");
            _userService.Delete("1");

            // Assert
            _mockRepository.Verify(repo => repo.Remove(It.Is<User>(u => u.Id == "1")), Times.Once);
        }
    }
}
