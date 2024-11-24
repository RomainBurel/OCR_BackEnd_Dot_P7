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

        private UserModelAdd GetNewUserModelAdd()
        {
            return new UserModelAdd() { Email = "newuser@findexium.com", UserName = "UserNew", FullName = "The new user", Password = "NewPWD123@" };
        }

        private UserModelUpdate GetUserModelToUpdate(User user)
        {
            return new UserModelUpdate() { UserName = user.UserName, Email = user.Email, FullName = user.FullName };
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
            var nbRecords = this.NbRecordsInTable();
            await this._factory.LoginAsUser(this._httpClient);

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
            var expectedUser = this.GetFirstRecordInTable();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/User/display/{expectedUser.Id}");
            var user = await response.Content.ReadFromJsonAsync<UserModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(user);
            Assert.Equal(expectedUser.Id, user.Id);
            Assert.Equal(expectedUser.UserName, user.UserName);
            Assert.Equal(expectedUser.FullName, user.FullName);
        }

        [Fact]
        public async Task GetUserById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleUsersAsync();
            var expectedUser = this.GetFirstRecordInTable();
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
            var nonExistingUserId = "0";
            await this._factory.LoginAsUser(this._httpClient);

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
            var newUser = this.GetNewUserModelAdd();
            var nbRecordsInit = this.NbRecordsInTable();
            await this._factory.LoginAsAdmin(this._httpClient);

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
            var newUser = this.GetNewUserModelAdd();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/User/creation", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddUser_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            var newUser = this.GetNewUserModelAdd();
            this._factory.Logout(this._httpClient);

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
            var userToUpdate = this.GetFirstRecordInTable();
            var userModelUpdate = this.GetUserModelToUpdate(userToUpdate);
            userModelUpdate.UserName = "UpdatedUser";
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/User/update/{userToUpdate.Id}", userModelUpdate);
            var userUpdated = this.GetRecordById(userToUpdate.Id);

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
            var userToUpdate = this.GetFirstRecordInTable();
            var userModelUpdate = this.GetUserModelToUpdate(userToUpdate);
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
            var userToUpdate = this.GetFirstRecordInTable();
            var userModelUpdate = this.GetUserModelToUpdate(userToUpdate);
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
            var userToUpdate = this.GetFirstRecordInTable();
            var userModelUpdate = this.GetUserModelToUpdate(userToUpdate);
            userModelUpdate.UserName = "UpdatedUser";
            var nonExistingUserId = "0";
            await this._factory.LoginAsAdmin(this._httpClient);

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
            var users = this.GetAllRecordsInTable();
            var userToDeleteId = users?.Find(user => user.Email == "user1@findexium.com")?.Id;
            await this._factory.LoginAsAdmin(this._httpClient);

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
            var users = this.GetAllRecordsInTable();
            var userToDeleteId = users?.Find(user => user.Email == "user1@findexium.com")?.Id;
            await this._factory.LoginAsUser(this._httpClient);

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
            var users = this.GetAllRecordsInTable();
            var userToDeleteId = users?.Find(user => user.Email == "user1@findexium.com")?.Id;
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
            var nonExistingUserId = "-1";
            await this._factory.LoginAsAdmin(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/User/deletion/{nonExistingUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
