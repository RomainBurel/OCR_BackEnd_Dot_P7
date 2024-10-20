using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class BidListService : IBidListService
    {
        private IBidListRepository _bidListRepository;

        public BidListService(IBidListRepository bidListRepository)
        {
            this._bidListRepository = bidListRepository;
        }

        public IEnumerable<BidListModel> GetAll()
        {
            return this._bidListRepository.GetAll().Select(b => this.GetModelFromData(b));
        }

        public BidListModel GetById(int id)
        {
            return this.GetModelFromData(this._bidListRepository.GetById(id));
        }

        public void Add(BidListModelAdd model)
        {
            this._bidListRepository.Add(this.GetDataFromModelAdd(model));
        }

        public void Update(BidListModel model)
        {
            this._bidListRepository.Update(this.GetDataFromModel(model));
        }

        public void Delete(BidListModel model)
        {
            this._bidListRepository.Remove(this.GetDataFromModel(model));
        }

        private BidListModel GetModelFromData(BidList bidList)
        {
            return new BidListModel()
            {
                BidListId = bidList.BidListId,
                Account = bidList.Account,
                BidType = bidList.BidType,
                BidQuantity = bidList.BidQuantity,
                AskQuantity = bidList.AskQuantity,
                Bid = bidList.Bid,
                Ask = bidList.Ask,
                Benchmark = bidList.Benchmark,
                BidListDate = bidList.BidListDate,
                Commentary = bidList.Commentary,
                BidSecurity = bidList.BidSecurity,
                BidStatus = bidList.BidStatus,
                Trader = bidList.Trader,
                Book = bidList.Book,
                CreationName = bidList.CreationName,
                CreationDate = bidList.CreationDate,
                RevisionName = bidList.RevisionName,
                RevisionDate = bidList.RevisionDate,
                DealName = bidList.DealName,
                DealType = bidList.DealType,
                SourceListId = bidList.SourceListId,
                Side = bidList.Side
            };
        }

        private BidList GetDataFromModelAdd(BidListModelAdd model)
        {
            return new BidList()
            {
                Account = model.Account,
                BidType = model.BidType,
                BidQuantity = model.BidQuantity,
                AskQuantity = model.AskQuantity,
                Bid = model.Bid,
                Ask = model.Ask,
                Benchmark = model.Benchmark,
                BidListDate = model.BidListDate,
                Commentary = model.Commentary,
                BidSecurity = model.BidSecurity,
                BidStatus = model.BidStatus,
                Trader = model.Trader,
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

        private BidList GetDataFromModel(BidListModel model)
        {
            return new BidList()
            {
                BidListId = model.BidListId,
                Account = model.Account,
                BidType = model.BidType,
                BidQuantity = model.BidQuantity,
                AskQuantity = model.AskQuantity,
                Bid = model.Bid,
                Ask = model.Ask,
                Benchmark = model.Benchmark,
                BidListDate = model.BidListDate,
                Commentary = model.Commentary,
                BidSecurity = model.BidSecurity,
                BidStatus = model.BidStatus,
                Trader = model.Trader,
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
