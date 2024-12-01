using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        public TradeRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
