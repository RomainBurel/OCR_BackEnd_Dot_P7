using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public User GetById(string id);

        public bool Exists(string id);
    }
}
