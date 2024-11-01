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
            var curvePoint = this._curvePointService.GetById(curvePointId);

            if (curvePoint == null)
            {
                _logger.LogWarning("CurvePoint with id {curvePointId} not found", curvePointId);
                return NotFound($"CurvePoint with id {curvePointId} not found for update");
            }

            _logger.LogInformation("CurvePoint with id {curvePointId} found", curvePointId);
            return Ok(curvePoint);
        }

        [HttpPost]
        [Route("creation")]
        public IActionResult AddCurvePoint([FromBody] CurvePointModelAdd curvePointModel)
        {
            _logger.LogInformation("CurvePoint add requested");
            _curvePointService.Add(curvePointModel);
            _logger.LogInformation("CurvePoint add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateCurvePoint(int curvePointId, [FromBody] CurvePointModelUpdate curvePointModelUpdate)
        {
            _logger.LogInformation("CurvePoint update requested");

            var existingCurvePointModel = _curvePointService.GetById(curvePointId);
            if (existingCurvePointModel == null)
            {
                _logger.LogWarning("CurvePoint with id {curvePointId} not found for update", curvePointModelUpdate);
                return NotFound($"CurvePoint with id {curvePointId} not found for update");
            }

            _curvePointService.Update(existingCurvePointModel, curvePointModelUpdate);
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