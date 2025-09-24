using System.Linq.Expressions;
namespace AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filtro);
        Task<T?> GetAsync(Expression<Func<T, bool>> filtro);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> RemoveAsync(T entity);


        Task<List<T>> QueryAsync
              (Expression<Func<T, bool>>? filtro = null,
              params string[] includeProperties);


    }
}