namespace UniversityAPI.Repositories.Base
{
    public interface IBaseRepository<TEntity>
    {
        public Task<List<TEntity>> Get();
        public Task<TEntity?> Get(int id);
        public Task<int> Create(TEntity entity);
        public void Update(TEntity entity);
        public Task Delete(int id);
    }
}
