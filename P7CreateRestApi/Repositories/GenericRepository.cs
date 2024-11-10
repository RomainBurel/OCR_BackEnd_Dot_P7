using Dot.Net.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace P7CreateRestApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LocalDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(LocalDbContext context)
        {
            this._context = context;
            this._dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return this._dbSet.ToList();
        }

        public T GetById(int id)
        {
            return this._dbSet.Find(id);
        }

        public bool Exists(int id)
        {
            return this._dbSet.Find(id) != null;
        }

        public void Add(T entity)
        {
            this._dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(T entity)
        {
            this._dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            this._context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }
    }
}
