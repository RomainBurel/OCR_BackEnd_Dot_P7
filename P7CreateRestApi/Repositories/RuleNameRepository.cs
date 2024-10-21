using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace P7CreateRestApi.Repositories
{
    public class RuleNameRepository : GenericRepository<RuleName>, IRuleNameRepository
    {
        public RuleNameRepository(DbContext context) : base(context)
        {
        }
    }
}
