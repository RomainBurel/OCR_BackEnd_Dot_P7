using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class RatingService : IRatingService
    {
        private IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            this._ratingRepository = ratingRepository;
        }

        public IEnumerable<RatingModel> GetAll()
        {
            return this._ratingRepository.GetAll().Select(r => this.GetModelFromData(r));
        }

        public RatingModel? GetById(int id)
        {
            var rating = this._ratingRepository.GetById(id);
            return rating != null ? this.GetModelFromData(rating) : null;
        }

        public bool Exists(int id)
        {
            return this._ratingRepository.Exists(id);
        }

        public void Add(RatingModelAdd modelAdd)
        {
            this._ratingRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(int id, RatingModelUpdate modelUpdate)
        {
            this._ratingRepository.Update(this.GetDataFromModelUpdate(id, modelUpdate));
        }

        public void Delete(int id)
        {
            this._ratingRepository.Remove(this._ratingRepository.GetById(id));
        }

        private RatingModel GetModelFromData(Rating rating)
        {
            return new RatingModel()
            {
                Id = rating.Id,
                MoodysRating = rating.MoodysRating,
                SandPRating = rating.SandPRating,
                FitchRating = rating.FitchRating,
                OrderNumber = rating.OrderNumber
            };
        }

        private Rating GetDataFromModelAdd(RatingModelAdd model)
        {
            return new Rating()
            {
                MoodysRating = model.MoodysRating,
                SandPRating = model.SandPRating,
                FitchRating = model.FitchRating,
                OrderNumber = model.OrderNumber
            };
        }

        private Rating GetDataFromModelUpdate(int id, RatingModelUpdate modelUpdate)
        {
            var ratingModel = this._ratingRepository.GetById(id);
            ratingModel.MoodysRating = modelUpdate.MoodysRating;
            ratingModel.SandPRating = modelUpdate.SandPRating;
            ratingModel.FitchRating = modelUpdate.FitchRating;
            ratingModel.OrderNumber = modelUpdate.OrderNumber;
            return ratingModel;
        }
    }
}
