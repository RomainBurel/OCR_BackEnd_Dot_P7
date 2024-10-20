using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IBidListService
    {
        public IEnumerable<BidListModel> GetAll();

        public BidListModel GetById(int id);

        public void Add(BidListModelAdd bidList);

        public void Update(BidListModel bidList);

        public void Delete(BidListModel bidList);
    }
}
