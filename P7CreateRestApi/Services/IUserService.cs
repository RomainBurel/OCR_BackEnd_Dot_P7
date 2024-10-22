using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IUserService
    {
        public IEnumerable<UserModel> GetAll();

        public UserModel GetById(int id);

        public void Add(UserModelAdd modelAdd);

        public void Update(UserModel model);

        public void Delete(UserModel model);
    }
}
