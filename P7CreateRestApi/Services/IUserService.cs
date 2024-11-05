using P7CreateRestApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace P7CreateRestApi.Services
{
    public interface IUserService
    {
        public IEnumerable<UserModel> GetAll();

        public UserModel GetById(int id);

        public void Add(UserModelAdd modelAdd);

        public void Update(UserModel model);

        public void Delete(UserModel model);

        public Task<JwtSecurityToken> GetUserLoginToken(LoginModel loginModel);
    }
}
