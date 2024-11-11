using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        private ILogger<TradeController> _logger;

        public TradeController(ILogger<TradeController> logger, ITradeService tradeService)
        {
            _logger = logger;
            _tradeService = tradeService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All trade requested");
            return Ok(this._tradeService.GetAll());
        }

        [HttpGet]
        [Route("display/{tradeId}")]
        public IActionResult GetTradeById(int tradeId)
        {
            _logger.LogInformation("Trade with id {tradeId} requested", tradeId);
            var trade = this._tradeService.GetById(tradeId);

            if (trade == null)
            {
                _logger.LogWarning("Trade with id {tradeId} not found", tradeId);
                return NotFound($"Trade with id {tradeId} not found for update");
            }

            _logger.LogInformation("Trade with id {tradeId} found", tradeId);
            return Ok(trade);
        }

        [HttpPost]
        [Route("creation")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddTrade([FromBody] TradeModelAdd tradeModel)
        {
            _logger.LogInformation("Trade add requested");
            _tradeService.Add(tradeModel);
            _logger.LogInformation("Trade add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{tradeId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTrade(int tradeId, [FromBody] TradeModelUpdate tradeModelUpdate)
        {
            _logger.LogInformation("Trade update requested");

            if (!this._tradeService.Exists(tradeId))
            {
                _logger.LogWarning("Trade with id {tradeId} not found for update", tradeModelUpdate);
                return NotFound($"Trade with id {tradeId} not found for update");
            }

            _tradeService.Update(tradeId, tradeModelUpdate);
            _logger.LogInformation("Trade update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{tradeId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTrade(int tradeId)
        {
            _logger.LogInformation("Trade delete requested");

            if (!this._tradeService.Exists(tradeId))
            {
                _logger.LogWarning("Trade with id {tradeId} not found for deletion", tradeId);
                return NotFound($"Trade with id {tradeId} not found for deletion");
            }

            _tradeService.Delete(tradeId);
            _logger.LogInformation("Trade delete successfull");
            return Ok();
        }
    }
}