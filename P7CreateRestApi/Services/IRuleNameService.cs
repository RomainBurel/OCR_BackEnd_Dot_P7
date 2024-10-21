using P7CreateRestApi.Models;

namespace P7CreateRestApi.Services
{
    public interface IRuleNameService
    {
        public IEnumerable<RuleNameModel> GetAll();

        public RuleNameModel GetById(int id);

        public void Add(RuleNameModelAdd modelAdd);

        public void Update(RuleNameModel model);

        public void Delete(RuleNameModel model);
    }
}
