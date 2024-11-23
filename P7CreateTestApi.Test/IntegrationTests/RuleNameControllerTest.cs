using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class RuleNameControllerTest : GenericController<RuleName>
    {
        public RuleNameControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<RuleName> GetRuleName()
        {
            return new List<RuleName> {
                new RuleName() { Id = 1, Name = "Rule1", Description = "The rule 1", Json = "Json1", Template = "Template1", SqlStr = "1", SqlPart = "1" },
                new RuleName() { Id = 2, Name = "Rule2", Description = "The rule 2", Json = "Json2", Template = "Template2", SqlStr = "2", SqlPart = "2" },
                new RuleName() {Id = 3, Name = "Rule3", Description = "The rule 3", Json = "Json3", Template = "Template3", SqlStr = "3", SqlPart = "3"},
                new RuleName() {Id = 4, Name = "Rule4", Description = "The rule 4", Json = "Json4", Template = "Template4", SqlStr = "4", SqlPart = "4"}
            };
        }

        private RuleNameModelAdd NewRuleName()
        {
            return new RuleNameModelAdd() { Name = "New Rule", Description = "The new rule", Json = "Json", Template = "Template", SqlStr = "0", SqlPart = "0" };
        }

        private async Task SeedSampleRuleNamesAsync()
        {
            await this.ClearDatabase();
            await this._factory.LoginAsAdmin(this._httpClient);

            var ruleNames = GetRuleName();
            foreach (var ruleName in ruleNames)
            {
                await this._httpClient.PostAsJsonAsync("/RuleName/creation", new RuleNameModelAdd
                {
                    Name = ruleName.Name,
                    Description = ruleName.Description,
                    Json = ruleName.Json,
                    Template = ruleName.Template,
                    SqlStr = ruleName.SqlStr,
                    SqlPart = ruleName.SqlPart
                });
            }

            this._factory.Logout(_httpClient);
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await response.Content.ReadFromJsonAsync<List<RuleNameModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(ruleNames);
            Assert.Equal(this.GetRuleName().Count, ruleNames.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/RuleName/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRuleNameById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var expectedRuleName = ruleNames[0];

            // Act
            var response = await this._httpClient.GetAsync($"/RuleName/display/{expectedRuleName.Id}");
            var ruleName = await response.Content.ReadFromJsonAsync<RuleNameModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(ruleName);
            Assert.Equal(expectedRuleName.Id, ruleName.Id);
            Assert.Equal(expectedRuleName.Name, ruleName.Name);
            Assert.Equal(expectedRuleName.Description, ruleName.Description);
        }

        [Fact]
        public async Task GetRuleNameById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var expectedRuleName = ruleNames[0];
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/RuleName/display/{expectedRuleName.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRuleNameById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var nonExistingRuleNameId = ruleNames.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.GetAsync($"/RuleName/display/{nonExistingRuleNameId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddRuleName_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var nbRecordsInit = this.GetRuleName().Count;
            var newRuleName = this.NewRuleName();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/RuleName/creation", newRuleName);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(ruleNames.Count, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddRuleName_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            await this._factory.LoginAsUser(this._httpClient);
            var newRuleName = this.NewRuleName();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/RuleName/creation", newRuleName);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRuleName_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newRuleName = this.NewRuleName();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/RuleName/creation", newRuleName);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRuleName_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToUpdate = ruleNames[0];
            var ruleNameModelUpdate = new RuleNameModelUpdate()
            {
                Name = ruleNameToUpdate.Name,
                Description = ruleNameToUpdate.Description,
                Json = ruleNameToUpdate.Json,
                Template = ruleNameToUpdate.Template,
                SqlStr = ruleNameToUpdate.SqlStr,
                SqlPart = ruleNameToUpdate.SqlPart
            };
            ruleNameModelUpdate.Name = "Updated rule";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/RuleName/update/{ruleNameToUpdate.Id}", ruleNameModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/RuleName/display/{ruleNameToUpdate.Id}");
            var ruleNameUpdated = await responseUpdated.Content.ReadFromJsonAsync<RuleNameModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(ruleNameUpdated.Name, ruleNameModelUpdate.Name);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRuleName_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToUpdate = ruleNames[0];
            var ruleNameModelUpdate = new RuleNameModelUpdate()
            {
                Name = ruleNameToUpdate.Name,
                Description = ruleNameToUpdate.Description,
                Json = ruleNameToUpdate.Json,
                Template = ruleNameToUpdate.Template,
                SqlStr = ruleNameToUpdate.SqlStr,
                SqlPart = ruleNameToUpdate.SqlPart
            };
            ruleNameModelUpdate.Name = "Updated rule";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/RuleName/update/{ruleNameToUpdate.Id}", ruleNameModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRuleName_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToUpdate = ruleNames[0];
            var ruleNameModelUpdate = new RuleNameModelUpdate()
            {
                Name = ruleNameToUpdate.Name,
                Description = ruleNameToUpdate.Description,
                Json = ruleNameToUpdate.Json,
                Template = ruleNameToUpdate.Template,
                SqlStr = ruleNameToUpdate.SqlStr,
                SqlPart = ruleNameToUpdate.SqlPart
            };
            ruleNameModelUpdate.Name = "Updated rule";
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/RuleName/update/{ruleNameToUpdate.Id}", ruleNameModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRuleName_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToUpdate = ruleNames[0];
            var ruleNameModelUpdate = new RuleNameModelUpdate()
            {
                Name = ruleNameToUpdate.Name,
                Description = ruleNameToUpdate.Description,
                Json = ruleNameToUpdate.Json,
                Template = ruleNameToUpdate.Template,
                SqlStr = ruleNameToUpdate.SqlStr,
                SqlPart = ruleNameToUpdate.SqlPart
            };
            ruleNameModelUpdate.Name = "Updated rule";
            var nonExistingRuleNameId = ruleNames.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/RuleName/update/{nonExistingRuleNameId}", ruleNameModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRuleName_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToDeleteId = ruleNames[0].Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/RuleName/deletion/{ruleNameToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRuleName_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToDeleteId = ruleNames[0].Id;

            // Act
            var response = await this._httpClient.DeleteAsync($"/RuleName/deletion/{ruleNameToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRuleName_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var ruleNameToDeleteId = ruleNames[0].Id;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/RuleName/deletion/{ruleNameToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRuleName_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleRuleNamesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/RuleName/list");
            var ruleNames = await responseAll.Content.ReadFromJsonAsync<List<RuleNameModel>>();
            var nonExistingRuleNameId = ruleNames.Max(r => r.Id) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/RuleName/deletion/{nonExistingRuleNameId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
