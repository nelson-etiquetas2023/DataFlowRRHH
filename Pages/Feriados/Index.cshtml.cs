using DataFlowRRHH.Models;
using DataFlowRRHH.Service.Contracts;
using DataFlowRRHH.Service.Implementations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataFlowRRHH.Pages.Feriados
{
    public class IndexModel : PageModel
    {
        public List<Feriado> Feriados { get; set; } = null!;
        public Feriado Feriado { get; set; } = null!;
        IServiceFeriado ServiceFeriado { get; set; }

        public IndexModel(IServiceFeriado serviceFerido)
        {
            ServiceFeriado = serviceFerido;
        }

        public void OnGet()
        {
            Feriados = ServiceFeriado.GetFeriadosAll();
        }
    }
}
