using P7CreateRestApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace P7CreateRestApi.Services
{
    public interface IUserService
    {
        public IEnumerable<UserModel> GetAll();

        public UserModel? GetById(string id);

        public bool Exists(string id);

        public void Add(UserModelAdd modelAdd);

        public void Update(string id, UserModelUpdate modelUpdate);

        public void Delete(string id);

        public Task<JwtSecurityToken> GetUserLoginToken(LoginModel loginModel);
    }
}
