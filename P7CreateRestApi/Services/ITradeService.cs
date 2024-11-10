using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface ITradeService
    {
        public IEnumerable<TradeModel> GetAll();

        public TradeModel? GetById(int id);

        public bool Exists(int id);

        public void Add(TradeModelAdd modelAdd);

        public void Update(int id, TradeModelUpdate modelUpdate);

        public void Delete(int id);
    }
}
