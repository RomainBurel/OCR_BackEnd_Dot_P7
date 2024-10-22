using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace P7CreateRestApi.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}