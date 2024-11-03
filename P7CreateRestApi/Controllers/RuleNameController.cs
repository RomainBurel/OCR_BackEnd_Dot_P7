using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;

namespace Dot.Net.WebApi.Controllers
{
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
        [Route("display/{id}")]
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

        [HttpPost]
        [Route("creation")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddRuleName([FromBody] RuleNameModelAdd ruleNameModel)
        {
            _logger.LogInformation("RuleName add requested");
            _ruleNameService.Add(ruleNameModel);
            _logger.LogInformation("RuleName add successfull");

            return Ok();
        }

        [HttpPut]
        [Route("update/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateRuleName(int ruleNameId, [FromBody] RuleNameModel ruleNameModel)
        {
            _logger.LogInformation("RuleName update requested");

            var existingRuleNameModel = _ruleNameService.GetById(ruleNameId);
            if (existingRuleNameModel == null)
            {
                _logger.LogWarning("RuleName with id {ruleNameId} not found for update", ruleNameModel);
                return NotFound($"RuleName with id {ruleNameId} not found for update");
            }

            _ruleNameService.Update(ruleNameModel);
            _logger.LogInformation("RuleName update successfull");
            return Ok();
        }

        [HttpDelete]
        [Route("deletion/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteRuleName(int ruleNameId)
        {
            _logger.LogInformation("RuleName delete requested");

            var ruleNameModel = _ruleNameService.GetById(ruleNameId);
            if (ruleNameModel == null)
            {
                _logger.LogWarning("RuleName with id {ruleNameId} not found for deletion", ruleNameId);
                return NotFound($"RuleName with id {ruleNameId} not found for deletion");
            }

            _ruleNameService.Delete(ruleNameModel);
            _logger.LogInformation("RuleName delete successfull");
            return Ok();
        }
    }
}