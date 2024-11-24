using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class BidListControllerTest : GenericControllerTest<BidList>
    {
        public BidListControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<BidList> GetBidLists()
        {
            return new List<BidList> {
                new BidList() { BidListId = 1, Account = "Account1", CreationDate = DateTime.Now, BidType = "Current",
                    Benchmark = "Benchmark1", Commentary = "Commentary1", BidSecurity = "BidSecurity1", BidStatus = "BidStatus1",
                    Trader = "Trader1", Book = "Book1", CreationName = "BidList1", DealName = "Deal1", DealType = "Type1",
                    SourceListId = "Source1", Side = "Side1"},
                new BidList() { BidListId = 2, Account = "Account2", CreationDate = DateTime.Now, BidType = "Current",
                    Benchmark = "Benchmark2", Commentary = "Commentary2", BidSecurity = "BidSecurity2", BidStatus = "BidStatus2",
                    Trader = "Trader2", Book = "Book2", CreationName = "BidList2", DealName = "Deal2", DealType = "Type2",
                    SourceListId = "Source2", Side = "Side2"},
                new BidList() { BidListId = 3, Account = "Account3", CreationDate = DateTime.Now, BidType = "Current",
                    Benchmark = "Benchmark3", Commentary = "Commentary3", BidSecurity = "BidSecurity3", BidStatus = "BidStatus3",
                    Trader = "Trader3", Book = "Book3", CreationName = "BidList3", DealName = "Deal3", DealType = "Type3",
                    SourceListId = "Source3", Side = "Side3"},
                new BidList() { BidListId = 4, Account = "Account4", CreationDate = DateTime.Now, BidType = "Current",
                    Benchmark = "Benchmark4", Commentary = "Commentary4", BidSecurity = "BidSecurity4", BidStatus = "BidStatus4",
                    Trader = "Trader4", Book = "Book4", CreationName = "BidList4", DealName = "Deal4", DealType = "Type4",
                    SourceListId = "Source4", Side = "Side4"},
            };
        }

        private BidListModelAdd NewBidList()
        {
            return new BidListModelAdd()
            {
                Account = "New Account", CreationDate = DateTime.Now, BidType = "New Current", Benchmark = "New Benchmark",
                Commentary = "New Commentary", BidSecurity = "New BidSecurity", BidStatus = "New BidStatus", Trader = "New Trader",
                Book = "New Book", CreationName = "New BidList", DealName = "New Deal", DealType = "New Type",
                SourceListId = "New Source", Side = "New Side"
            };
        }

        private async Task SeedSampleBidListsAsync()
        {
            await this.ClearTable();
            await this.FillTable(this.GetBidLists());
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await response.Content.ReadFromJsonAsync<List<BidListModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(bidLists);
            Assert.Equal(this.GetBidLists().Count, bidLists.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/BidList/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetBidListById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var expectedBidList = bidLists[0];

            // Act
            var response = await this._httpClient.GetAsync($"/BidList/display/{expectedBidList.BidListId}");
            var bidList = await response.Content.ReadFromJsonAsync<BidListModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(bidList);
            Assert.Equal(expectedBidList.BidListId, bidList.BidListId);
            Assert.Equal(expectedBidList.Account, bidList.Account);
            Assert.Equal(expectedBidList.CreationDate, bidList.CreationDate);
            Assert.Equal(expectedBidList.BidType, bidList.BidType);
        }

        [Fact]
        public async Task GetBidListById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var expectedBidList = bidLists[0];
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/BidList/display/{expectedBidList.BidListId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetBidListById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var nonExistingBidListId = bidLists.Max(r => r.BidListId) + 1;

            // Act
            var response = await this._httpClient.GetAsync($"/BidList/display/{nonExistingBidListId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddBidList_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var nbRecordsInit = this.GetBidLists().Count;
            var newBidList = this.NewBidList();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/BidList/creation", newBidList);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(bidLists.Count, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddBidList_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            await this._factory.LoginAsUser(this._httpClient);
            var newBidList = this.NewBidList();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/BidList/creation", newBidList);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddBidList_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newBidList = this.NewBidList();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/BidList/creation", newBidList);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBidList_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToUpdate = bidLists[0];
            var bidListModelUpdate = new BidListModelUpdate()
            {
                Account = bidListToUpdate.Account,
                BidType = bidListToUpdate.BidType,
                Benchmark = bidListToUpdate.Benchmark,
                Commentary = bidListToUpdate.Commentary,
                BidSecurity = bidListToUpdate.BidSecurity,
                BidStatus = bidListToUpdate.BidStatus,
                Trader = bidListToUpdate.Trader,
                Book = bidListToUpdate.Book,
                RevisionName = "First revision",
                DealName = bidListToUpdate.DealName,
                DealType = bidListToUpdate.DealType,
                SourceListId = bidListToUpdate.SourceListId,
                Side = bidListToUpdate.Side
            };
            bidListModelUpdate.BidType = "Update";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/BidList/update/{bidListToUpdate.BidListId}", bidListModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/BidList/display/{bidListToUpdate.BidListId}");
            var bidListUpdated = await responseUpdated.Content.ReadFromJsonAsync<BidListModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(bidListUpdated.BidType, bidListModelUpdate.BidType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBidList_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToUpdate = bidLists[0];
            var bidListModelUpdate = new BidListModelUpdate()
            {
                Account = bidListToUpdate.Account,
                BidType = bidListToUpdate.BidType,
                Benchmark = bidListToUpdate.Benchmark,
                Commentary = bidListToUpdate.Commentary,
                BidSecurity = bidListToUpdate.BidSecurity,
                BidStatus = bidListToUpdate.BidStatus,
                Trader = bidListToUpdate.Trader,
                Book = bidListToUpdate.Book,
                RevisionName = "First revision",
                DealName = bidListToUpdate.DealName,
                DealType = bidListToUpdate.DealType,
                SourceListId = bidListToUpdate.SourceListId,
                Side = bidListToUpdate.Side
            };
            bidListModelUpdate.BidType = "Update";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/BidList/update/{bidListToUpdate.BidListId}", bidListModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBidList_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToUpdate = bidLists[0];
            var bidListModelUpdate = new BidListModelUpdate()
            {
                Account = bidListToUpdate.Account,
                BidType = bidListToUpdate.BidType,
                Benchmark = bidListToUpdate.Benchmark,
                Commentary = bidListToUpdate.Commentary,
                BidSecurity = bidListToUpdate.BidSecurity,
                BidStatus = bidListToUpdate.BidStatus,
                Trader = bidListToUpdate.Trader,
                Book = bidListToUpdate.Book,
                RevisionName = "First revision",
                DealName = bidListToUpdate.DealName,
                DealType = bidListToUpdate.DealType,
                SourceListId = bidListToUpdate.SourceListId,
                Side = bidListToUpdate.Side
            };
            bidListModelUpdate.BidType = "Update";
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/BidList/update/{bidListToUpdate.BidListId}", bidListModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBidList_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToUpdate = bidLists[0];
            var bidListModelUpdate = new BidListModelUpdate()
            {
                Account = bidListToUpdate.Account,
                BidType = bidListToUpdate.BidType,
                Benchmark = bidListToUpdate.Benchmark,
                Commentary = bidListToUpdate.Commentary,
                BidSecurity = bidListToUpdate.BidSecurity,
                BidStatus = bidListToUpdate.BidStatus,
                Trader = bidListToUpdate.Trader,
                Book = bidListToUpdate.Book,
                RevisionName = "First revision",
                DealName = bidListToUpdate.DealName,
                DealType = bidListToUpdate.DealType,
                SourceListId = bidListToUpdate.SourceListId,
                Side = bidListToUpdate.Side
            };
            bidListModelUpdate.BidType = "Update";
            var nonExistingBidListId = bidLists.Max(r => r.BidListId) + 1;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/BidList/update/{nonExistingBidListId}", bidListModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBidList_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToDeleteId = bidLists[0].BidListId;

            // Act
            var response = await this._httpClient.DeleteAsync($"/BidList/deletion/{bidListToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBidList_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToDeleteId = bidLists[0].BidListId;

            // Act
            var response = await this._httpClient.DeleteAsync($"/BidList/deletion/{bidListToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBidList_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var bidListToDeleteId = bidLists[0].BidListId;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/BidList/deletion/{bidListToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBidList_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleBidListsAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/BidList/list");
            var bidLists = await responseAll.Content.ReadFromJsonAsync<List<BidListModel>>();
            var nonExistingBidListId = bidLists.Max(r => r.BidListId) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/BidList/deletion/{nonExistingBidListId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
