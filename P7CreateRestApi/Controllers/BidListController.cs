using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidListService _bidListService;
        private ILogger<BidListController> _logger;

        public BidListController(ILogger<BidListController> logger, IBidListService bidListService)
        {
            _logger = logger;
            _bidListService = bidListService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All bidList requested");
            return Ok(this._bidListService.GetAll());
        }

        [HttpGet]
        [Route("display/{id}")]
        public IActionResult GetBidListById(int bidListId)
        {
            _logger.LogInformation("BidList with id {bidListId} requested", bidListId);
            var bidlist = this._bidListService.GetById(bidListId);

            if (bidlist == null)
            {
                _logger.LogWarning("BidList with id {bidListId} not found", bidListId);
                return NotFound($"BidList with id {bidListId} not found for update");
            }

            _logger.LogInformation("BidList with id {bidListId} found", bidListId);
            return Ok(bidlist);
        }

        [HttpPost]
        [Route("creation/{id}")]
        public IActionResult AddBidList([FromBody] BidListModelAdd bidListModel)
        {
            _logger.LogInformation("BidList add requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for bidList add");
                return BadRequest("Invalid model for bidList add");
            }

            _bidListService.Add(bidListModel);
            _logger.LogInformation("BidList add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateBidList(int bidListId, [FromBody] BidListModel bidListModel)
        {
            _logger.LogInformation("BidList update requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for bidList update");
                return BadRequest("Invalid model for bidList update");
            }

            var existingBidListModel = _bidListService.GetById(bidListId);
            if (existingBidListModel == null)
            {
                _logger.LogWarning("BidList with id {bidListId} not found for update", bidListModel);
                return NotFound($"BidList with id {bidListId} not found for update");
            }

            _bidListService.Update(bidListModel);
            _logger.LogInformation("BidList update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{id}")]
        public IActionResult DeleteBidList(int bidListId)
        {
            _logger.LogInformation("BidList delete requested");

            var bidListModel = _bidListService.GetById(bidListId);
            if (bidListModel == null)
            {
                _logger.LogWarning("BidList with id {bidListId} not found for deletion", bidListId);
                return NotFound($"BidList with id {bidListId} not found for deletion");
            }

            _bidListService.Delete(bidListModel);
            _logger.LogInformation("BidList delete successfull");
            return Ok();
        }
    }
}