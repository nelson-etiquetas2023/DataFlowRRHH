using DataFlowRRHH.Models;
using DataFlowRRHH.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DataFlowRRHH.Service.Implementations
{
    public class ServiceFeriado : IServiceFeriado
    {
        public BdbioAdminSqlContext Context { get; set; }
        public ServiceFeriado(BdbioAdminSqlContext context)
        {
            Context = context;
        }

        public async Task<Feriado> AddFeriado(Feriado feriado)
        {
            try
            {
                await Context.Feriado.AddAsync(feriado);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            
            return(feriado);
        }

        public void DeleteFeriado(Feriado Feriado)
        {
            if (Feriado == null)
            {
                throw new ArgumentNullException(nameof(Feriado));
            }
            Context.Feriado.Remove(Feriado);
            Context.SaveChanges();
        }

        public Feriado? GetFeriadoById(int Id)
        {
            var item = Context.Feriado.Find(Id);
            return item;
        }

        public List<Feriado> GetFeriadosAll()
        {
            var lista = Context.Feriado.ToList();
            return lista;
        }

        public void UpdateFeriado(Feriado feriado)
        {
            throw new NotImplementedException();
        }
        public void SaveChanges() => Context.SaveChanges();
    }
}
