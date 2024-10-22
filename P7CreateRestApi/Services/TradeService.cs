using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class TradeService : ITradeService
    {
        private ITradeRepository _tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            this._tradeRepository = tradeRepository;
        }

        public IEnumerable<TradeModel> GetAll()
        {
            return this._tradeRepository.GetAll().Select(r => this.GetModelFromData(r));
        }

        public TradeModel GetById(int id)
        {
            return this.GetModelFromData(this._tradeRepository.GetById(id));
        }

        public void Add(TradeModelAdd modelAdd)
        {
            this._tradeRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(TradeModel model)
        {
            this._tradeRepository.Update(this.GetDataFromModel(model));
        }

        public void Delete(TradeModel model)
        {
            this._tradeRepository.Remove(this.GetDataFromModel(model));
        }

        private TradeModel GetModelFromData(Trade trade)
        {
            return new TradeModel()
            {
                TradeId = trade.TradeId,
                Account = trade.Account,
                AccountType = trade.AccountType,
                BuyQuantity = trade.BuyQuantity,
                SellQuantity = trade.SellQuantity,
                BuyPrice = trade.BuyPrice,
                SellPrice = trade.SellPrice,
                TradeDate = trade.TradeDate,
                TradeSecurity = trade.TradeSecurity,
                TradeStatus = trade.TradeStatus,
                Trader = trade.Trader,
                Benchmark = trade.Benchmark,
                Book = trade.Book,
                CreationName = trade.CreationName,
                CreationDate = trade.CreationDate,
                RevisionName = trade.RevisionName,
                RevisionDate = trade.RevisionDate,
                DealName = trade.DealName,
                DealType = trade.DealType,
                SourceListId = trade.SourceListId,
                Side = trade.Side
            };
        }

        private Trade GetDataFromModelAdd(TradeModelAdd model)
        {
            return new Trade()
            {
                Account = model.Account,
                AccountType = model.AccountType,
                BuyQuantity = model.BuyQuantity,
                SellQuantity = model.SellQuantity,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                TradeDate = model.TradeDate,
                TradeSecurity = model.TradeSecurity,
                TradeStatus = model.TradeStatus,
                Trader = model.Trader,
                Benchmark = model.Benchmark,
                Book = model.Book,
                CreationName = model.CreationName,
                CreationDate = model.CreationDate,
                RevisionName = model.RevisionName,
                RevisionDate = model.RevisionDate,
                DealName = model.DealName,
                DealType = model.DealType,
                SourceListId = model.SourceListId,
                Side = model.Side
            };
        }

        private Trade GetDataFromModel(TradeModel model)
        {
            return new Trade()
            {
                TradeId = model.TradeId,
                Account = model.Account,
                AccountType = model.AccountType,
                BuyQuantity = model.BuyQuantity,
                SellQuantity = model.SellQuantity,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                TradeDate = model.TradeDate,
                TradeSecurity = model.TradeSecurity,
                TradeStatus = model.TradeStatus,
                Trader = model.Trader,
                Benchmark = model.Benchmark,
                Book = model.Book,
                CreationName = model.CreationName,
                CreationDate = model.CreationDate,
                RevisionName = model.RevisionName,
                RevisionDate = model.RevisionDate,
                DealName = model.DealName,
                DealType = model.DealType,
                SourceListId = model.SourceListId,
                Side = model.Side
            };
        }
    }
}
