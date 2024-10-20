using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace P7CreateRestApi.Repositories
{
    public class CurvePointRepository : GenericRepository<CurvePoint>, ICurvePointRepository
    {
        public CurvePointRepository(DbContext context) : base(context)
        {
        }
    }
}
