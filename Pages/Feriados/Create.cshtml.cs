using DataFlowRRHH.Models;
using DataFlowRRHH.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace DataFlowRRHH.Pages.Feriados
{
    public class CreateModel : PageModel
    {
        public Feriado Feriado { get; set; } = null!;
        [BindProperty]
        public DateTime StartDate { get; set; }
        public DateTime EndtDate { get; set; }
        IServiceFeriado ServiceFeriado { get; }
        public CreateModel(IServiceFeriado serviceFeriado)
        {
            StartDate = DateTime.Now;
            EndtDate = DateTime.Now;
            ServiceFeriado = serviceFeriado;
        }
        public void OnGet()
        {


        }
        public async Task<IActionResult> OnPost(Feriado Feriado)
        {
            await ServiceFeriado.AddFeriado(Feriado);
            return this.RedirectToPage("./Index");
        }
    }
}