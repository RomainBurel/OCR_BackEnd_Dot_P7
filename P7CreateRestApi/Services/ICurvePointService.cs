using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface ICurvePointService
    {
        public IEnumerable<CurvePointModel> GetAll();

        public CurvePointModel GetById(int id);

        public void Add(CurvePointModelAdd modelAdd);

        public void Update(CurvePointModel model);

        public void Delete(CurvePointModel model);
    }
}
