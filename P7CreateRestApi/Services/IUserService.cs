using P7CreateRestApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace P7CreateRestApi.Services
{
    public interface IUserService
    {
        public IEnumerable<UserModel> GetAll();

        public UserModel? GetById(int id);

        public bool Exists(int id);

        public void Add(UserModelAdd modelAdd);

        public void Update(int id, UserModelUpdate modelUpdate);

        public void Delete(int id);

        public Task<JwtSecurityToken> GetUserLoginToken(LoginModel loginModel);
    }
}
