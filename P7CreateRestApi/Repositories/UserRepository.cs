using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LocalDbContext context) : base(context)
        {
        }

        public User GetById(string id)
        {
            return this._dbSet.Find(id);
        }

        public bool Exists(string id)
        {
            return this._dbSet.Find(id) != null;
        }
    }
}