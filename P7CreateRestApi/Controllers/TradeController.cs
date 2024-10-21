using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
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
        [Route("display/{id}")]
        public IActionResult GetTradeById(int tradeId)
        {
            _logger.LogInformation("Trade with id {tradeId} requested", tradeId);
            var bidlist = this._tradeService.GetById(tradeId);

            if (bidlist == null)
            {
                _logger.LogWarning("Trade with id {tradeId} not found", tradeId);
                return NotFound($"Trade with id {tradeId} not found for update");
            }

            _logger.LogInformation("Trade with id {tradeId} found", tradeId);
            return Ok(bidlist);
        }

        [HttpPost]
        [Route("creation/{id}")]
        public IActionResult AddTrade([FromBody] TradeModelAdd tradeModel)
        {
            _logger.LogInformation("Trade add requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for trade add");
                return BadRequest("Invalid model for trade add");
            }

            _tradeService.Add(tradeModel);
            _logger.LogInformation("Trade add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateTrade(int tradeId, [FromBody] TradeModel tradeModel)
        {
            _logger.LogInformation("Trade update requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for trade update");
                return BadRequest("Invalid model for trade update");
            }

            var existingTradeModel = _tradeService.GetById(tradeId);
            if (existingTradeModel == null)
            {
                _logger.LogWarning("Trade with id {tradeId} not found for update", tradeModel);
                return NotFound($"Trade with id {tradeId} not found for update");
            }

            _tradeService.Update(tradeModel);
            _logger.LogInformation("Trade update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{id}")]
        public IActionResult DeleteTrade(int tradeId)
        {
            _logger.LogInformation("Trade delete requested");

            var tradeModel = _tradeService.GetById(tradeId);
            if (tradeModel == null)
            {
                _logger.LogWarning("Trade with id {tradeId} not found for deletion", tradeId);
                return NotFound($"Trade with id {tradeId} not found for deletion");
            }

            _tradeService.Delete(tradeModel);
            _logger.LogInformation("Trade delete successfull");
            return Ok();
        }
    }
}