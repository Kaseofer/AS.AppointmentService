using AS.AppointmentService.Infrastructure.Logger;
using AS.AppointmentService.Infrastructure.Persistence.Context;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AS.AppointmentService.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AgendaSaludDBContext _context;
        protected readonly DbSet<T> _dbSet;

        private readonly IAppLogger<GenericRepository<T>> _logger;

        public GenericRepository(AgendaSaludDBContext context, IAppLogger<GenericRepository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filtro)
        {
            return await _dbSet.Where(filtro).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                /// Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError("AddAsync: Error al dar de alta la entidad", ex, entity);

                throw;
            }
        }


        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateAsync", ex, entity);

                return false;
            }
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("EliminarAsync", ex, entity);
                return false;
            }
        }


        public async Task<T?> GetAsync(Expression<Func<T, bool>> filtro)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(filtro);

            return entity;
        }
        public async Task<List<T>> QueryAsync
            (Expression<Func<T, bool>>? filtro = null,
            params string[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            // Aplicar includes dinámicos
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            // Aplicar filtro si existe
            if (filtro != null)
            {
                query = query.Where(filtro);
            }

            return await query.ToListAsync();
        }

    }
}