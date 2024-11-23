using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using System.Net;
using System.Net.Http.Json;

namespace P7CreateTestApi.Test.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class TradeControllerTest : GenericController<Trade>
    {
        public TradeControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        private List<Trade> GetTrade()
        {
            return new List<Trade> {
                new Trade() { TradeId = 1, Account = "Account1", CreationDate = DateTime.Now, AccountType = "Current",
                    Benchmark = "Benchmark1", TradeSecurity = "TradeSecurity1", TradeStatus = "TradeStatus1",
                    Trader = "Trader1", Book = "Book1", CreationName = "TradeList1", DealName = "Deal1", DealType = "Type1",
                    SourceListId = "Source1", Side = "Side1"},
                new Trade() { TradeId = 2, Account = "Account2", CreationDate = DateTime.Now, AccountType = "Current",
                    Benchmark = "Benchmark2", TradeSecurity = "TradeSecurity2", TradeStatus = "TradeStatus2",
                    Trader = "Trader2", Book = "Book2", CreationName = "TradeList2", DealName = "Deal2", DealType = "Type2",
                    SourceListId = "Source2", Side = "Side2"},
                new Trade() { TradeId = 3, Account = "Account3", CreationDate = DateTime.Now, AccountType = "Current",
                    Benchmark = "Benchmark3", TradeSecurity = "TradeSecurity3", TradeStatus = "TradeStatus3",
                    Trader = "Trader3", Book = "Book3", CreationName = "TradeList3", DealName = "Deal3", DealType = "Type3",
                    SourceListId = "Source3", Side = "Side3"},
                new Trade() { TradeId = 4, Account = "Account4", CreationDate = DateTime.Now, AccountType = "Current",
                    Benchmark = "Benchmark4", TradeSecurity = "TradeSecurity4", TradeStatus = "TradeStatus4",
                    Trader = "Trader4", Book = "Book4", CreationName = "TradeList4", DealName = "Deal4", DealType = "Type4",
                    SourceListId = "Source4", Side = "Side4"},
            };
        }

        private TradeModelAdd NewTrade()
        {
            return new TradeModelAdd()
            {
                Account = "Account1", CreationDate = DateTime.Now, AccountType = "Current", Benchmark = "Benchmark1",
                TradeSecurity = "TradeSecurity1", TradeStatus = "TradeStatus1", Trader = "Trader1", Book = "Book1",
                CreationName = "TradeList1", DealName = "Deal1", DealType = "Type1", SourceListId = "Source1", Side = "Side1"
            };
        }

        private async Task SeedSampleTradesAsync()
        {
            await this.ClearDatabase();
            await this._factory.LoginAsAdmin(this._httpClient);

            var trades = GetTrade();
            foreach (var trade in trades)
            {
                await this._httpClient.PostAsJsonAsync("/Trade/creation", new TradeModelAdd
                {
                    Account = trade.Account,
                    AccountType = trade.AccountType,
                    Benchmark = trade.Benchmark,
                    TradeSecurity = trade.TradeSecurity,
                    TradeStatus = trade.TradeStatus,
                    Trader = trade.Trader,
                    Book = trade.Book,
                    CreationDate = trade.CreationDate,
                    CreationName = trade.CreationName,
                    DealName = trade.DealName,
                    DealType = trade.DealType,
                    SourceListId = trade.SourceListId,
                    Side = trade.Side
                });
            }

            this._factory.Logout(_httpClient);
        }

        [Fact]
        public async Task GetAll_AsLoggedUser_ShouldReturn_AllRecords()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Trade/list");
            var trades = await response.Content.ReadFromJsonAsync<List<TradeModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(trades);
            Assert.Equal(this.GetTrade().Count, trades.Count);
        }

        [Fact]
        public async Task GetAll_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync("/Trade/list");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTradeById_AsLoggedUser_ShouldReturn_Record()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var expectedTrade = trades[0];

            // Act
            var response = await this._httpClient.GetAsync($"/Trade/display/{expectedTrade.TradeId}");
            var trade = await response.Content.ReadFromJsonAsync<TradeModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(trade);
            Assert.Equal(expectedTrade.TradeId, trade.TradeId);
        }

        [Fact]
        public async Task GetTradeById_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var expectedTrade = trades[0];
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.GetAsync($"/Trade/display/{expectedTrade.TradeId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTradeById_AsLoggedUser_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var nonExistingTradeId = trades.Max(r => r.TradeId) + 1;

            // Act
            var response = await this._httpClient.GetAsync($"/Trade/display/{nonExistingTradeId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddTrade_AsLoggedAdmin_ShouldIncrease_NbRecords()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var nbRecordsInit = this.GetTrade().Count;
            var newTrade = this.NewTrade();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Trade/creation", newTrade);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(trades.Count, nbRecordsInit + 1);
        }

        [Fact]
        public async Task AddTrade_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            await this._factory.LoginAsUser(this._httpClient);
            var newTrade = this.NewTrade();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Trade/creation", newTrade);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTrade_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            this._factory.Logout(this._httpClient);
            var newTrade = this.NewTrade();

            // Act
            var response = await this._httpClient.PostAsJsonAsync("/Trade/creation", newTrade);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTrade_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToUpdate = trades[0];
            var tradeModelUpdate = new TradeModelUpdate()
            {
                Account = tradeToUpdate.Account,
                AccountType = tradeToUpdate.AccountType,
                Benchmark = tradeToUpdate.Benchmark,
                TradeSecurity = tradeToUpdate.TradeSecurity,
                TradeStatus = tradeToUpdate.TradeStatus,
                Trader = tradeToUpdate.Trader,
                Book = tradeToUpdate.Book,
                RevisionName = "First revision",
                RevisionDate = DateTime.Now,
                DealName = tradeToUpdate.DealName,
                DealType = tradeToUpdate.DealType,
                SourceListId = tradeToUpdate.SourceListId,
                Side = tradeToUpdate.Side
            };
            tradeModelUpdate.Account = "Updated Account";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Trade/update/{tradeToUpdate.TradeId}", tradeModelUpdate);
            var responseUpdated = await this._httpClient.GetAsync($"/Trade/display/{tradeToUpdate.TradeId}");
            var tradeUpdated = await responseUpdated.Content.ReadFromJsonAsync<TradeModel>();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(tradeUpdated.Account, tradeModelUpdate.Account);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTrade_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToUpdate = trades[0];
            var tradeModelUpdate = new TradeModelUpdate()
            {
                Account = tradeToUpdate.Account,
                AccountType = tradeToUpdate.AccountType,
                Benchmark = tradeToUpdate.Benchmark,
                TradeSecurity = tradeToUpdate.TradeSecurity,
                TradeStatus = tradeToUpdate.TradeStatus,
                Trader = tradeToUpdate.Trader,
                Book = tradeToUpdate.Book,
                RevisionName = "First revision",
                RevisionDate = DateTime.Now,
                DealName = tradeToUpdate.DealName,
                DealType = tradeToUpdate.DealType,
                SourceListId = tradeToUpdate.SourceListId,
                Side = tradeToUpdate.Side
            };
            tradeModelUpdate.Account = "Updated Account";

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Trade/update/{tradeToUpdate.TradeId}", tradeModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTrade_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToUpdate = trades[0];
            var tradeModelUpdate = new TradeModelUpdate()
            {
                Account = tradeToUpdate.Account,
                AccountType = tradeToUpdate.AccountType,
                Benchmark = tradeToUpdate.Benchmark,
                TradeSecurity = tradeToUpdate.TradeSecurity,
                TradeStatus = tradeToUpdate.TradeStatus,
                Trader = tradeToUpdate.Trader,
                Book = tradeToUpdate.Book,
                RevisionName = "First revision",
                RevisionDate = DateTime.Now,
                DealName = tradeToUpdate.DealName,
                DealType = tradeToUpdate.DealType,
                SourceListId = tradeToUpdate.SourceListId,
                Side = tradeToUpdate.Side
            };
            tradeModelUpdate.Account = "Updated Account";
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Trade/update/{tradeToUpdate.TradeId}", tradeModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTrade_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToUpdate = trades[0];
            var tradeModelUpdate = new TradeModelUpdate()
            {
                Account = tradeToUpdate.Account,
                AccountType = tradeToUpdate.AccountType,
                Benchmark = tradeToUpdate.Benchmark,
                TradeSecurity = tradeToUpdate.TradeSecurity,
                TradeStatus = tradeToUpdate.TradeStatus,
                Trader = tradeToUpdate.Trader,
                Book = tradeToUpdate.Book,
                RevisionName = "First revision",
                RevisionDate = DateTime.Now,
                DealName = tradeToUpdate.DealName,
                DealType = tradeToUpdate.DealType,
                SourceListId = tradeToUpdate.SourceListId,
                Side = tradeToUpdate.Side
            };
            tradeModelUpdate.Account = "Updated Account";
            var nonExistingTradeId = trades.Max(r => r.TradeId) + 1;

            // Act
            var response = await this._httpClient.PutAsJsonAsync($"/Trade/update/{nonExistingTradeId}", tradeModelUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_AsLoggedAdmin_ShouldReturn_Ok()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToDeleteId = trades[0].TradeId;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Trade/deletion/{tradeToDeleteId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_AsLoggedUser_ShouldReturn_Forbidden()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToDeleteId = trades[0].TradeId;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Trade/deletion/{tradeToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_NoLoggedUser_ShouldReturn_Unauthorized()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsUser(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var tradeToDeleteId = trades[0].TradeId;
            this._factory.Logout(this._httpClient);

            // Act
            var response = await this._httpClient.DeleteAsync($"/Trade/deletion/{tradeToDeleteId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_AsLoggedAdmin_ForNonExistingId_ShouldReturn_NotFound()
        {
            // Arrange
            await this.SeedSampleTradesAsync();
            await this._factory.LoginAsAdmin(this._httpClient);
            var responseAll = await this._httpClient.GetAsync("/Trade/list");
            var trades = await responseAll.Content.ReadFromJsonAsync<List<TradeModel>>();
            var nonExistingTradeId = trades.Max(r => r.TradeId) + 1;

            // Act
            var response = await this._httpClient.DeleteAsync($"/Trade/deletion/{nonExistingTradeId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
