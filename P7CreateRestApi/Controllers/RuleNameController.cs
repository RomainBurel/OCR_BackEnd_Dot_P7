using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleNameService _ruleNameService;
        private ILogger<RuleNameController> _logger;

        public RuleNameController(ILogger<RuleNameController> logger, IRuleNameService ruleNameService)
        {
            _logger = logger;
            _ruleNameService = ruleNameService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("All ruleName requested");
            return Ok(this._ruleNameService.GetAll());
        }

        [HttpGet]
        [Route("display/{ruleNameId}")]
        public IActionResult GetRuleNameById(int ruleNameId)
        {
            _logger.LogInformation("RuleName with id {ruleNameId} requested", ruleNameId);
            var ruleName = this._ruleNameService.GetById(ruleNameId);

            if (ruleName == null)
            {
                _logger.LogWarning("RuleName with id {ruleNameId} not found", ruleNameId);
                return NotFound($"RuleName with id {ruleNameId} not found for update");
            }

            _logger.LogInformation("RuleName with id {ruleNameId} found", ruleNameId);
            return Ok(ruleName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("creation")]
        public IActionResult AddRuleName([FromBody] RuleNameModelAdd ruleNameModel)
        {
            _logger.LogInformation("RuleName add requested");
            _ruleNameService.Add(ruleNameModel);
            _logger.LogInformation("RuleName add successfull");

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("update/{ruleNameId}")]
        public IActionResult UpdateRuleName(int ruleNameId, [FromBody] RuleNameModelUpdate ruleNameModelUpdate)
        {
            _logger.LogInformation("RuleName update requested");

            if (!this._ruleNameService.Exists(ruleNameId))
            {
                _logger.LogWarning("RuleName with id {ruleNameId} not found for update", ruleNameModelUpdate);
                return NotFound($"RuleName with id {ruleNameId} not found for update");
            }

            _ruleNameService.Update(ruleNameId, ruleNameModelUpdate);
            _logger.LogInformation("RuleName update successfull");
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("deletion/{ruleNameId}")]
        public IActionResult DeleteRuleName(int ruleNameId)
        {
            _logger.LogInformation("RuleName delete requested");

            if (!this._ruleNameService.Exists(ruleNameId))
            {
                _logger.LogWarning("RuleName with id {ruleNameId} not found for deletion", ruleNameId);
                return NotFound($"RuleName with id {ruleNameId} not found for deletion");
            }

            _ruleNameService.Delete(ruleNameId);
            _logger.LogInformation("RuleName delete successfull");
            return Ok();
        }
    }
}