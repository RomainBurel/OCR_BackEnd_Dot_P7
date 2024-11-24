using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace P7CreateRestApi.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._userRepository = userRepository;
            this._userManager = userManager;
            this._roleManager = roleManager;
            _configuration = configuration;
        }

        public IEnumerable<UserModel> GetAll()
        {
            return this._userRepository.GetAll().Select(r => this.GetModelFromData(r));
        }

        public UserModel? GetById(string id)
        {
            var user = this._userRepository.GetById(id);
            return user != null ? this.GetModelFromData(user) : null;
        }

        public bool Exists(string id)
        {
            return this._userRepository.Exists(id);
        }

        public void Add(UserModelAdd modelAdd)
        {
            var user = this.GetDataFromModelAdd(modelAdd);
            var userResult = this._userManager.CreateAsync(user, modelAdd.Password).GetAwaiter().GetResult();

            if (!userResult.Succeeded)
            {
                throw new Exception("Failed to create '" + modelAdd.UserName + "' user");
            }

            this._userManager.AddToRoleAsync(user, "User").GetAwaiter().GetResult();
        }

        public void Update(string id, UserModelUpdate modelUpdate)
        {
            this._userRepository.Update(this.GetDataFromModelUpdate(id, modelUpdate));
        }

        public void Delete(string id)
        {
            this._userRepository.Remove(this._userRepository.GetById(id));
        }

        public async Task<JwtSecurityToken> GetUserLoginToken(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                return this.GenerateJwtToken(user, roles);
            }

            return null;
        }

        private JwtSecurityToken GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var userRole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

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
                Email = user.Email,
                FullName = user.FullName
            };
        }

        private User GetDataFromModelAdd(UserModelAdd model)
        {
            return new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
            };
        }

        private User GetDataFromModelUpdate(string id, UserModelUpdate modelUpdate)
        {
            var user = this._userRepository.GetById(id);
            user.UserName = modelUpdate.UserName;
            user.Email = modelUpdate.Email;
            user.FullName = modelUpdate.FullName;
            return user;
        }
    }
}
