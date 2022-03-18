using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.TodoItems.Shared.Exceptions;

namespace TodoApp.DataAccess
{
    public abstract class EfCoreSqlRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        private readonly TContext _context;

        protected EfCoreSqlRepository(TContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> GetAsync(long id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(entity))
            {
                throw new NotFoundException("Item may be removed");
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        private bool TodoItemExists(TEntity entity) =>
            _context.Set<TEntity>().Find(entity) == null;
    }
}
