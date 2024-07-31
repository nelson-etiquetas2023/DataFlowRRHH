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
        public IServiceFeriado _serviceFeriado { get; set; }
        public UpdateModel(IServiceFeriado serviceFeriado)
        {
            _serviceFeriado = serviceFeriado;   
        }
        public void OnGet(int id)
        {
            var feriado = _serviceFeriado.GetFeriadoById(id);
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
        public async Task<IActionResult> OnPost() 
        {
            if (EditFeriado is not null) 
            {
                var existingFeriado = _serviceFeriado.GetFeriadoById(EditFeriado.IdException);
                if (existingFeriado is not null) 
                {
                    existingFeriado.Description = EditFeriado.Description;
                    existingFeriado.BeginingDate = EditFeriado.BeginingDate;
                    existingFeriado.EndingDate = EditFeriado.EndingDate;
                    existingFeriado.Comment = EditFeriado.Comment;
                    existingFeriado.PaymentFactor = EditFeriado.PaymentFactor;
                    existingFeriado.Recurring = EditFeriado.Recurring;
                    _serviceFeriado.SaveChanges();
                }
            }
            return this.RedirectToPage("./Index");
        }
    }
}
