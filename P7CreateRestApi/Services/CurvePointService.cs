using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class CurvePointService : ICurvePointService
    {
        private ICurvePointRepository _curvePointRepository;

        public CurvePointService(ICurvePointRepository curvePointRepository)
        {
            this._curvePointRepository = curvePointRepository;
        }

        public IEnumerable<CurvePointModel> GetAll()
        {
            return this._curvePointRepository.GetAll().Select(b => this.GetModelFromData(b));
        }

        public CurvePointModel GetById(int id)
        {
            return this.GetModelFromData(this._curvePointRepository.GetById(id));
        }

        public void Add(CurvePointModelAdd model)
        {
            this._curvePointRepository.Add(this.GetDataFromModelAdd(model));
        }

        public void Update(CurvePointModel model)
        {
            this._curvePointRepository.Update(this.GetDataFromModel(model));
        }

        public void Delete(CurvePointModel model)
        {
            this._curvePointRepository.Remove(this.GetDataFromModel(model));
        }

        private CurvePointModel GetModelFromData(CurvePoint curvePoint)
        {
            return new CurvePointModel()
            {
                Id = curvePoint.Id,
                CurveId = curvePoint.CurveId,
                AsOfDate = curvePoint.AsOfDate,
                Term = curvePoint.Term,
                CurvePointValue = curvePoint.CurvePointValue,
                CreationDate = curvePoint.CreationDate
            };
        }

        private CurvePoint GetDataFromModelAdd(CurvePointModelAdd model)
        {
            return new CurvePoint()
            {
                CurveId = model.CurveId,
                AsOfDate = model.AsOfDate,
                Term = model.Term,
                CurvePointValue = model.CurvePointValue,
                CreationDate = model.CreationDate
            };
        }

        private CurvePoint GetDataFromModel(CurvePointModel model)
        {
            return new CurvePoint()
            {
                Id = model.Id,
                CurveId = model.CurveId,
                AsOfDate = model.AsOfDate,
                Term = model.Term,
                CurvePointValue = model.CurvePointValue,
                CreationDate = model.CreationDate
            };
        }
    }
}
