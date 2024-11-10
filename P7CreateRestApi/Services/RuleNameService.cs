using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Services
{
    public class RuleNameService : IRuleNameService
    {
        private IRuleNameRepository _ruleNameRepository;

        public RuleNameService(IRuleNameRepository ruleNameRepository)
        {
            this._ruleNameRepository = ruleNameRepository;
        }

        public IEnumerable<RuleNameModel> GetAll()
        {
            return this._ruleNameRepository.GetAll().Select(r => this.GetModelFromData(r));
        }

        public RuleNameModel? GetById(int id)
        {
            var ruleName = this._ruleNameRepository.GetById(id);
            return ruleName != null ? this.GetModelFromData(ruleName) : null;
        }

        public void Add(RuleNameModelAdd modelAdd)
        {
            this._ruleNameRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(RuleNameModel model, RuleNameModelUpdate modelUpdate)
        {
            this._ruleNameRepository.Update(this.GetDataFromModelUpdate(model, modelUpdate));
        }

        public void Delete(RuleNameModel model)
        {
            this._ruleNameRepository.Remove(this.GetDataFromModel(model));
        }

        private RuleNameModel GetModelFromData(RuleName ruleName)
        {
            return new RuleNameModel()
            {
                Id = ruleName.Id,
                Name = ruleName.Name,
                Description = ruleName.Description,
                Json = ruleName.Json,
                Template = ruleName.Template,
                SqlStr = ruleName.SqlStr,
                SqlPart = ruleName.SqlPart
            };
        }

        private RuleName GetDataFromModelAdd(RuleNameModelAdd model)
        {
            return new RuleName()
            {
                Name = model.Name,
                Description = model.Description,
                Json = model.Json,
                Template = model.Template,
                SqlStr = model.SqlStr,
                SqlPart = model.SqlPart
            };
        }

        private RuleName GetDataFromModelUpdate(RuleNameModel model, RuleNameModelUpdate modelUpdate)
        {
            var ruleName = this.GetDataFromModel(model);
            ruleName.Name = model.Name;
            ruleName.Description = model.Description;
            ruleName.Json = model.Json;
            ruleName.Template = model.Template;
            ruleName.SqlStr = model.SqlStr;
            ruleName.SqlPart = model.SqlPart;
            return ruleName;
        }

        private RuleName GetDataFromModel(RuleNameModel model)
        {
            return new RuleName()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Json = model.Json,
                Template = model.Template,
                SqlStr = model.SqlStr,
                SqlPart = model.SqlPart
            };
        }
    }
}
