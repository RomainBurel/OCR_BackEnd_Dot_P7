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

        public bool Exists(int id)
        {
            return this._ruleNameRepository.Exists(id);
        }

        public void Add(RuleNameModelAdd modelAdd)
        {
            this._ruleNameRepository.Add(this.GetDataFromModelAdd(modelAdd));
        }

        public void Update(int id, RuleNameModelUpdate modelUpdate)
        {
            this._ruleNameRepository.Update(this.GetDataFromModelUpdate(id, modelUpdate));
        }

        public void Delete(int id)
        {
            this._ruleNameRepository.Remove(this._ruleNameRepository.GetById(id));
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

        private RuleName GetDataFromModelUpdate(int id, RuleNameModelUpdate modelUpdate)
        {
            var ruleName = this._ruleNameRepository.GetById(id);
            ruleName.Name = modelUpdate.Name;
            ruleName.Description = modelUpdate.Description;
            ruleName.Json = modelUpdate.Json;
            ruleName.Template = modelUpdate.Template;
            ruleName.SqlStr = modelUpdate.SqlStr;
            ruleName.SqlPart = modelUpdate.SqlPart;
            return ruleName;
        }
    }
}
