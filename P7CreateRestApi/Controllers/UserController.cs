using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All user requested");
            return Ok(this._userService.GetAll());
        }

        [HttpGet]
        [Route("display/{userId}")]
        public IActionResult GetUserById(int userId)
        {
            _logger.LogInformation("User with id {userId} requested", userId);
            var user = this._userService.GetById(userId);

            if (user == null)
            {
                _logger.LogWarning("User with id {userId} not found", userId);
                return NotFound($"User with id {userId} not found for update");
            }

            _logger.LogInformation("User with id {userId} found", userId);
            return Ok(user);
        }

        [HttpPost]
        [Route("creation")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser([FromBody] UserModelAdd userModel)
        {
            _logger.LogInformation("User add requested");
            _userService.Add(userModel);
            _logger.LogInformation("User add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(int userId, [FromBody] UserModel userModel)
        {
            _logger.LogInformation("User update requested");

            var existingUserModel = _userService.GetById(userId);
            if (existingUserModel == null)
            {
                _logger.LogWarning("User with id {userId} not found for update", userModel);
                return NotFound($"User with id {userId} not found for update");
            }

            _userService.Update(userModel);
            _logger.LogInformation("User update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int userId)
        {
            _logger.LogInformation("User delete requested");

            var userModel = _userService.GetById(userId);
            if (userModel == null)
            {
                _logger.LogWarning("User with id {userId} not found for deletion", userId);
                return NotFound($"User with id {userId} not found for deletion");
            }

            _userService.Delete(userModel);
            _logger.LogInformation("User delete successfull");
            return Ok();
        }
    }
}