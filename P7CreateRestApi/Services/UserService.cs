using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace P7CreateRestApi.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private UserManager<User> _userManager;
        private IConfiguration _configuration;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, IConfiguration configuration)
        {
            this._userRepository = userRepository;
            this._userManager = userManager;
            _configuration = configuration;
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

        public async Task<JwtSecurityToken> GetUserLoginToken(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return this.GenerateJwtToken(user);
            }

            return null;
        }

        private JwtSecurityToken GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role) // Role in Token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
        }

        private UserModel GetModelFromData(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        private User GetDataFromModelAdd(UserModelAdd model)
        {
            return new User()
            {
                UserName = model.UserName,
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
                FullName = model.FullName,
                Role = model.Role
            };
        }
    }
}
