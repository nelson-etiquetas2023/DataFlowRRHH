using DataFlowRRHH.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataFlowRRHH.Service.Contracts
{
    public interface IServiceFeriado
    {
        List<Feriado> GetFeriadosAll();
        Feriado? GetFeriadoById(int Id);
        Task<Feriado> AddFeriado(Feriado feriado);
        void UpdateFeriado(Feriado feriado);
        void DeleteFeriado(Feriado Feriado);
        void SaveChanges();
    }
}
