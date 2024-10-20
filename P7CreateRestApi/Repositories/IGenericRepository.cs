namespace P7CreateRestApi.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public T GetById(int id);

        public IEnumerable<T> GetAll();

        public void Add(T entity);

        public void Remove(T entity);

        public void Update(T entity);

        public void SaveChanges();
    }
}
