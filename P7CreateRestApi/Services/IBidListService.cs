using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IBidListService
    {
        public IEnumerable<BidListModel> GetAll();

        public BidListModel GetById(int id);

        public void Add(BidListModelAdd modelAdd);

        public void Update(BidListModel model);

        public void Delete(BidListModel model);
    }
}
