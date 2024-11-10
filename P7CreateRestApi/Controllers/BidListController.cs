using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
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
        [Route("display/{bidListId}")]
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
        [Route("creation")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddBidList([FromBody] BidListModelAdd bidListModel)
        {
            _logger.LogInformation("BidList add requested");
            _bidListService.Add(bidListModel);
            _logger.LogInformation("BidList add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{bidListId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateBidList(int bidListId, [FromBody] BidListModelUpdate bidListModelUpdate)
        {
            _logger.LogInformation("BidList update requested");

            if (!_bidListService.Exists(bidListId))
            {
                _logger.LogWarning("BidList with id {bidListId} not found for update", bidListModelUpdate);
                return NotFound($"BidList with id {bidListId} not found for update");
            }

            _bidListService.Update(bidListId, bidListModelUpdate);
            _logger.LogInformation("BidList update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{bidListId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBidList(int bidListId)
        {
            _logger.LogInformation("BidList delete requested");

            if (!this._bidListService.Exists(bidListId))
            {
                _logger.LogWarning("BidList with id {bidListId} not found for deletion", bidListId);
                return NotFound($"BidList with id {bidListId} not found for deletion");
            }

            _bidListService.Delete(bidListId);
            _logger.LogInformation("BidList delete successfull");
            return Ok();
        }
    }
}