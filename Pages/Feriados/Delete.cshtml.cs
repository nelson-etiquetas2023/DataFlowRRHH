using DataFlowRRHH.Models;
using DataFlowRRHH.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataFlowRRHH.Pages.Feriados
{
    public class DeleteModel : PageModel
    {
        public IServiceFeriado ServiceFeriado { get; set; }
        [BindProperty]
        public Feriado DeleteFeriado { get; set; } = null!;


        public DeleteModel(IServiceFeriado ServiceFeriado)
        {
            this.ServiceFeriado = ServiceFeriado;
            DeleteFeriado = new();
        }

        public void OnGet(int id)
        {
            var item = ServiceFeriado.GetFeriadoById(id);
            if (item is not null) 
            {
                DeleteFeriado.IdException = item.IdException;
                DeleteFeriado.Description = item.Description;   
                DeleteFeriado.BeginingDate = item.BeginingDate;
                DeleteFeriado.EndingDate = item.EndingDate; 
                DeleteFeriado.PaymentFactor = item.PaymentFactor;
                DeleteFeriado.Recurring = item.Recurring;
                DeleteFeriado.Comment = item.Comment;
            }
        }
        public IActionResult OnPost() 
        {
            var deleteItem = ServiceFeriado.GetFeriadoById(DeleteFeriado.IdException);
            if (deleteItem is not null) 
            {
                ServiceFeriado.DeleteFeriado(deleteItem);
                ServiceFeriado.SaveChanges();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
