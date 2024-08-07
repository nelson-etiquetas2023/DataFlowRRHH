
using DataFlowRRHH.Response;

namespace DataFlowRRHH.Repositories.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<ActionResponse<T>> GetByIdAsync(int id);
        IEnumerable<T> GetAllAsync();
        void AddAsync(T entity);
        Task<ActionResponse<T>> DeleteAsync(int id);
        Task<ActionResponse<T>> UpdateAsync(T entity);
    }
}
