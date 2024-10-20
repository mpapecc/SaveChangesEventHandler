using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Api.Repositories.Implementation
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetById(Guid id)
        {
            return _context.Set<T>().Where(entity => entity.Id == id);
        }

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentOutOfRangeException(nameof(entity));

            entity.CreationDate = DateTime.UtcNow;
            entity.LastUpdateDate = DateTime.UtcNow;

            _context.Set<T>().Add(entity);

        }

        public void Update(T entity)
        {
            entity.LastUpdateDate = DateTime.UtcNow;

            _context.Set<T>().Update(entity);
        }

        public void Delete(Guid id)
        {
            var entity = _context.Set<T>().FirstOrDefault(e => e.Id == id);

            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(List<T> entities)
        {
            _context.RemoveRange(entities);
        }

        public void UpdateRange(List<T> entities)
        {
            entities.ForEach(x => x.LastUpdateDate = DateTime.UtcNow);
            _context.UpdateRange(entities);
        }

        public void AddRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreationDate = DateTime.UtcNow;
                entity.LastUpdateDate = DateTime.UtcNow;
            }

            _context.AddRange(entities);
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
