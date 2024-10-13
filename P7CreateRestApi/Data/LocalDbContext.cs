using Microsoft.EntityFrameworkCore;
using Dot.Net.WebApi.Domain;

namespace Dot.Net.WebApi.Data
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<User>? Users { get; set; } = null;
        public DbSet<BidList>? BidLists { get; set; } = null;
        public DbSet<CurvePoint>? CurvePoints { get; set; } = null;
        public DbSet<Rating>? Ratings { get; set; } = null;
        public DbSet<RuleName>? RuleNames { get; set; } = null;
        public DbSet<Trade>? Trades { get; set; } = null;
    }
}