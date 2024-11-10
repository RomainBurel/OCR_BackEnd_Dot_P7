using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface ICurvePointService
    {
        public IEnumerable<CurvePointModel> GetAll();

        public CurvePointModel? GetById(int id);

        public bool Exists(int id);

        public void Add(CurvePointModelAdd modelAdd);

        public void Update(int id, CurvePointModelUpdate modelUpdate);

        public void Delete(int id);
    }
}
