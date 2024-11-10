using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class BidListRepository : GenericRepository<BidList>, IBidListRepository
    {
        public BidListRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
