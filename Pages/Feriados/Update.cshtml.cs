using DataFlowRRHH.Models;
using DataFlowRRHH.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace DataFlowRRHH.Pages.Feriados
{
    public class UpdateModel : PageModel
    {
        [BindProperty]
        public Feriado EditFeriado { get; set; } = null!;
        public IServiceFeriado ServiceFeriado { get; set; }
        public UpdateModel(IServiceFeriado serviceFeriado)
        {
            ServiceFeriado = serviceFeriado;   
        }
        public void OnGet(int id)
        {
            var feriado = ServiceFeriado.GetFeriadoById(id);
            if (feriado is not null) 
            {
                EditFeriado = new Feriado()
                {
                    IdException = feriado.IdException,
                    Description = feriado.Description,  
                    BeginingDate = feriado.BeginingDate,    
                    EndingDate = feriado.EndingDate,    
                    PaymentFactor = feriado.PaymentFactor,  
                    Comment = feriado.Comment
                };
            }
        }
        public IActionResult OnPost() 
        {
            if (EditFeriado is not null) 
            {
                var existingFeriado = ServiceFeriado.GetFeriadoById(EditFeriado.IdException);
                if (existingFeriado is not null) 
                {
                    existingFeriado.Description = EditFeriado.Description;
                    existingFeriado.BeginingDate = EditFeriado.BeginingDate;
                    existingFeriado.EndingDate = EditFeriado.EndingDate;
                    existingFeriado.Comment = EditFeriado.Comment;
                    existingFeriado.PaymentFactor = EditFeriado.PaymentFactor;
                    existingFeriado.Recurring = EditFeriado.Recurring;
                    ServiceFeriado.SaveChanges();
                }
            }
            return this.RedirectToPage("./Index");
        }
    }
}
