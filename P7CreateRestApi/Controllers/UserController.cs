using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
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
        [Route("display/{id}")]
        public IActionResult GetUserById(int userId)
        {
            _logger.LogInformation("User with id {userId} requested", userId);
            var bidlist = this._userService.GetById(userId);

            if (bidlist == null)
            {
                _logger.LogWarning("User with id {userId} not found", userId);
                return NotFound($"User with id {userId} not found for update");
            }

            _logger.LogInformation("User with id {userId} found", userId);
            return Ok(bidlist);
        }

        [HttpPost]
        [Route("creation/{id}")]
        public IActionResult AddUser([FromBody] UserModelAdd userModel)
        {
            _logger.LogInformation("User add requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for user add");
                return BadRequest("Invalid model for user add");
            }

            _userService.Add(userModel);
            _logger.LogInformation("User add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateUser(int userId, [FromBody] UserModel userModel)
        {
            _logger.LogInformation("User update requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for user update");
                return BadRequest("Invalid model for user update");
            }

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
        [Route("deletion/{id}")]
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