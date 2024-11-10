using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IRatingService
    {
        public IEnumerable<RatingModel> GetAll();

        public RatingModel? GetById(int id);

        public bool Exists(int id);

        public void Add(RatingModelAdd modelAdd);

        public void Update(int id, RatingModelUpdate modelUpdate);

        public void Delete(int id);
    }
}
