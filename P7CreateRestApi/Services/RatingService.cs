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

        public void Add(RatingModelAdd modelAdd)
        {
            this._ratingRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(RatingModel model, RatingModelUpdate modelUpdate)
        {
            this._ratingRepository.Update(this.GetDataFromModelUpdate(model, modelUpdate));
        }

        public void Delete(RatingModel model)
        {
            this._ratingRepository.Remove(this.GetDataFromModel(model));
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

        private Rating GetDataFromModelUpdate(RatingModel model, RatingModelUpdate modelUpdate)
        {
            var ratingModel = GetDataFromModel(model);
            ratingModel.MoodysRating = model.MoodysRating;
            ratingModel.SandPRating = model.SandPRating;
            ratingModel.FitchRating = model.FitchRating;
            ratingModel.OrderNumber = model.OrderNumber;
            return ratingModel;
        }

        private Rating GetDataFromModel(RatingModel model)
        {
            return new Rating()
            {
                Id = model.Id,
                MoodysRating = model.MoodysRating,
                SandPRating = model.SandPRating,
                FitchRating = model.FitchRating,
                OrderNumber = model.OrderNumber
            };
        }
    }
}
