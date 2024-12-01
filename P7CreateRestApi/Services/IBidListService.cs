using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IBidListService
    {
        public IEnumerable<BidListModel> GetAll();

        public BidListModel? GetById(int id);

        public bool Exists(int id);

        public void Add(BidListModelAdd modelAdd);

        public void Update(int id, BidListModelUpdate modelUpdate);

        public void Delete(int id);
    }
}
