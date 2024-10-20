namespace Contacts.Api.Repositories.Abstraction
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetById(Guid id);

        IQueryable<T> GetAll();

        void Add(T entity);

        void AddRange(List<T> entities);

        void Update(T entity);

        void UpdateRange(List<T> entities);

        void Delete(Guid id);

        void DeleteRange(List<T> entities);

        bool Commit();
    }
}
