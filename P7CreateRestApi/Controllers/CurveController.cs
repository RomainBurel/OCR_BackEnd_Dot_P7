using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly ICurvePointService _curvePointService;
        private ILogger<CurveController> _logger;

        public CurveController(ILogger<CurveController> logger, ICurvePointService curvePointService)
        {
            _logger = logger;
            _curvePointService = curvePointService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All curvePoint requested");
            return Ok(this._curvePointService.GetAll());
        }

        [HttpGet]
        [Route("display/{id}")]
        public IActionResult GetCurvePointById(int curvePointId)
        {
            _logger.LogInformation("CurvePoint with id {curvePointId} requested", curvePointId);
            var bidlist = this._curvePointService.GetById(curvePointId);

            if (bidlist == null)
            {
                _logger.LogWarning("CurvePoint with id {curvePointId} not found", curvePointId);
                return NotFound($"CurvePoint with id {curvePointId} not found for update");
            }

            _logger.LogInformation("CurvePoint with id {curvePointId} found", curvePointId);
            return Ok(bidlist);
        }

        [HttpPost]
        [Route("creation/{id}")]
        public IActionResult AddCurvePoint([FromBody] CurvePointModelAdd curvePointModel)
        {
            _logger.LogInformation("CurvePoint add requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for curvePoint add");
                return BadRequest("Invalid model for curvePoint add");
            }

            _curvePointService.Add(curvePointModel);
            _logger.LogInformation("CurvePoint add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateCurvePoint(int curvePointId, [FromBody] CurvePointModel curvePointModel)
        {
            _logger.LogInformation("CurvePoint update requested");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for curvePoint update");
                return BadRequest("Invalid model for curvePoint update");
            }

            var existingCurvePointModel = _curvePointService.GetById(curvePointId);
            if (existingCurvePointModel == null)
            {
                _logger.LogWarning("CurvePoint with id {curvePointId} not found for update", curvePointModel);
                return NotFound($"CurvePoint with id {curvePointId} not found for update");
            }

            _curvePointService.Update(curvePointModel);
            _logger.LogInformation("CurvePoint update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{id}")]
        public IActionResult DeleteCurvePoint(int curvePointId)
        {
            _logger.LogInformation("CurvePoint delete requested");

            var curvePointModel = _curvePointService.GetById(curvePointId);
            if (curvePointModel == null)
            {
                _logger.LogWarning("CurvePoint with id {curvePointId} not found for deletion", curvePointId);
                return NotFound($"CurvePoint with id {curvePointId} not found for deletion");
            }

            _curvePointService.Delete(curvePointModel);
            _logger.LogInformation("CurvePoint delete successfull");
            return Ok();
        }
    }
}