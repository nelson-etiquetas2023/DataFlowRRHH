using DataFlowRRHH.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataFlowRRHH.Service.Contracts
{
    public interface IService<T> where T : class
    {
        Task<IActionResult> Add(T entity);
        Task<IActionResult> Update(T entity);
        Task<IActionResult> Delete(int id);
        Task<List<T>> GetUsuariosAll();
        Task<T> GetUsuarioById(int id);
        Task<IActionResult> SaveChanges(T entity);
    }
}
