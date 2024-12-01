using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class RuleNameRepository : GenericRepository<RuleName>, IRuleNameRepository
    {
        public RuleNameRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
