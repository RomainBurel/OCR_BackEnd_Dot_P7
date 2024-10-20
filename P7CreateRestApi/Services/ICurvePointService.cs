using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface ICurvePointService
    {
        public IEnumerable<CurvePointModel> GetAll();

        public CurvePointModel GetById(int id);

        public void Add(CurvePointModelAdd CurvePoint);

        public void Update(CurvePointModel CurvePoint);

        public void Delete(CurvePointModel CurvePoint);
    }
}
