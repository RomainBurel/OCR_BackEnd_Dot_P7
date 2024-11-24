using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class UserControllerTest : GenericControllerTest<User>
    {
        public UserControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<UserModelAdd> GetUsers()
        {
            return new List<UserModelAdd> {
                new UserModelAdd() { Email = "user1@findexium.com", UserName = "User1", FullName = "The user 1", Password = "Pwd123AA@" },
                new UserModelAdd() { Email = "user2@findexium.com", UserName = "User2", FullName = "The user 2", Password = "Pwd123AB@" },
                new UserModelAdd() { Email = "user3@findexium.com", UserName = "User3", FullName = "The user 3", Password = "Pwd123AC@" },
                new UserModelAdd() { Email = "user4@findexium.com", UserName = "User4", FullName = "The user 4", Password = "Pwd123AD@" }
            };
        }

        private UserModelAdd NewUser()
        {
            return new UserModelAdd() { Email = "newuser@findexium.com", UserName = "UserNew", FullName = "The new user", Password = "NewPWD123@" };
        }

        private async Task SeedSampleUsersAsync()
        {
            await this.ClearTable();
            using (var scope = this._factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                foreach (var user in this.GetUsers())
                {
                    await SeedData.AddUser(userManager, user.UserName, user.Email, user.Password, "User");
                }
            }
        }

        protected override async Task ClearTable()
        {
            using (var scope = this._factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                foreach (var user in this.GetUsers())
                {
                    await SeedData.RemoveUser(userManager, user.Email, "User");
                }
            }
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var nbRecords = this.NbRecordsInTable();

            // Act
            var response = await this._httpClient.GetAsync("/User/list");
            var users = await response.Content.ReadFromJsonAsync<List<UserModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(users);
            Assert.Equal(nbRecords, users.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/User/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var expectedUser = users[0];

            // Act
            var response = await this._httpClient.GetAsync($"/User/display/{expectedUser.Id}");
            var user = await response.Content.ReadFromJsonAsync<UserModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(user);
            Assert.Equal(expectedUser.Id, user.Id);
            Assert.Equal(expectedUser.UserName, user.UserName);
            Assert.Equal(expectedUser.FullName, user.FullName);
            Assert.Equal(expectedUser.Password, user.Password);
        }

        [Fact]
        public async Task GetUserById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var expectedUser = users[0];
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/User/display/{expectedUser.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var nonExistingUserId = users.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.GetAsync($"/User/display/{nonExistingUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddUser_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var newUser = this.NewUser();
            var nbRecordsInit = this.NbRecordsInTable();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/User/creation", newUser);
            var nbRecordsAfterAdd = this.NbRecordsInTable();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(nbRecordsAfterAdd, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddUser_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this._factory.LoginAsUser(this._httpClient);
            var newUser = this.NewUser();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/User/creation", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddUser_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newUser = this.NewUser();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/User/creation", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToUpdate = users[0];
            var userModelUpdate = new UserModelUpdate()
            {
                UserName = userToUpdate.UserName,
                Email = userToUpdate.Email,
                FullName = userToUpdate.FullName
            };
            userModelUpdate.UserName = "UpdatedUser";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/User/update/{userToUpdate.Id}", userModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/User/display/{userToUpdate.Id}");
            var userUpdated = await responseUpdated.Content.ReadFromJsonAsync<UserModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(userUpdated.UserName, userModelUpdate.UserName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToUpdate = users[0];
            var userModelUpdate = new UserModelUpdate()
            {
                UserName = userToUpdate.UserName,
                Email = userToUpdate.Email,
                FullName = userToUpdate.FullName
            };
            userModelUpdate.UserName = "UpdatedUser";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/User/update/{userToUpdate.Id}", userModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToUpdate = users[0];
            var userModelUpdate = new UserModelUpdate()
            {
                UserName = userToUpdate.UserName,
                Email = userToUpdate.Email,
                FullName = userToUpdate.FullName
            };
            userModelUpdate.UserName = "UpdatedUser";
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/User/update/{userToUpdate.Id}", userModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToUpdate = users[0];
            var userModelUpdate = new UserModelUpdate()
            {
                UserName = userToUpdate.UserName,
                Email = userToUpdate.Email,
                FullName = userToUpdate.FullName
            };
            userModelUpdate.UserName = "UpdatedUser";
            var nonExistingUserId = "0";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/User/update/{nonExistingUserId}", userModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToDeleteId = users?.Find(user => user.Email == "user1@findexium.com")?.Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/User/deletion/{userToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToDeleteId = users[0].Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/User/deletion/{userToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var userToDeleteId = users[0].Id;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/User/deletion/{userToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/User/list");
            var users = await responseAll.Content.ReadFromJsonAsync<List<UserModel>>();
            var nonExistingUserId = users.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/User/deletion/{nonExistingUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
