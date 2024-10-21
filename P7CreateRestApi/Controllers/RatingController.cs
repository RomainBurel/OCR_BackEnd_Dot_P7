using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private ILogger<RatingController> _logger;

        public RatingController(ILogger<RatingController> logger, IRatingService ratingService)
        {
            _logger = logger;
            _ratingService = ratingService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All rating requested");
            return Ok(this._ratingService.GetAll());
        }

        [HttpGet]
        [Route("display/{id}")]
        public IActionResult GetRatingById(int ratingId)
        {
            _logger.LogInformation("Rating with id {ratingId} requested", ratingId);
            var bidlist = this._ratingService.GetById(ratingId);

            if (bidlist == null)
            {
                _logger.LogWarning("Rating with id {ratingId} not found", ratingId);
                return NotFound($"Rating with id {ratingId} not found for update");
            }

            _logger.LogInformation("Rating with id {ratingId} found", ratingId);
            return Ok(bidlist);
        }

        [HttpPost]
        [Route("creation/{id}")]
        public IActionResult AddRating([FromBody] RatingModelAdd ratingModel)
        {
            _logger.LogInformation("Rating add requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for rating add");
                return BadRequest("Invalid model for rating add");
            }

            _ratingService.Add(ratingModel);
            _logger.LogInformation("Rating add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateRating(int ratingId, [FromBody] RatingModel ratingModel)
        {
            _logger.LogInformation("Rating update requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for rating update");
                return BadRequest("Invalid model for rating update");
            }

            var existingRatingModel = _ratingService.GetById(ratingId);
            if (existingRatingModel == null)
            {
                _logger.LogWarning("Rating with id {ratingId} not found for update", ratingModel);
                return NotFound($"Rating with id {ratingId} not found for update");
            }

            _ratingService.Update(ratingModel);
            _logger.LogInformation("Rating update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{id}")]
        public IActionResult DeleteRating(int ratingId)
        {
            _logger.LogInformation("Rating delete requested");

            var ratingModel = _ratingService.GetById(ratingId);
            if (ratingModel == null)
            {
                _logger.LogWarning("Rating with id {ratingId} not found for deletion", ratingId);
                return NotFound($"Rating with id {ratingId} not found for deletion");
            }

            _ratingService.Delete(ratingModel);
            _logger.LogInformation("Rating delete successfull");
            return Ok();
        }
    }
}