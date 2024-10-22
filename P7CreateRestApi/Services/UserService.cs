using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public IEnumerable<UserModel> GetAll()
        {
            return this._userRepository.GetAll().Select(r => this.GetModelFromData(r));
        }

        public UserModel GetById(int id)
        {
            return this.GetModelFromData(this._userRepository.GetById(id));
        }

        public void Add(UserModelAdd modelAdd)
        {
            this._userRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(UserModel model)
        {
            this._userRepository.Update(this.GetDataFromModel(model));
        }

        public void Delete(UserModel model)
        {
            this._userRepository.Remove(this.GetDataFromModel(model));
        }

        private UserModel GetModelFromData(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        private User GetDataFromModelAdd(UserModelAdd model)
        {
            return new User()
            {
                UserName = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                Role = model.Role
            };
        }

        private User GetDataFromModel(UserModel model)
        {
            return new User()
            {
                Id = model.Id,
                UserName = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                Role = model.Role
            };
        }
    }
}
