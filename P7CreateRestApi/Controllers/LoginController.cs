using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IUserService userService, IConfiguration configuration, ILogger<LoginController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var userToken = await _userService.GetUserLoginToken(model);
            if (userToken != null)
            {
                _logger.LogInformation("Successfull login for user with mail: {email}", model.Email);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(userToken),
                    expiration = userToken.ValidTo
                });
            }

            _logger.LogWarning("Login failed for user with mail: {email}", model.Email);
            return Unauthorized();
        }            
    }
}