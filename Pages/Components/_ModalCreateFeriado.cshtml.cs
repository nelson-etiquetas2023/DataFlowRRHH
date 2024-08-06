using Microsoft.AspNetCore.Mvc.RazorPages;
using DataFlowRRHH.Models;


namespace DataFlowRRHH.Pages
{
    public class _ModalCreateFeriadoModel : PageModel
    {
        public BdbioAdminSqlContext _context { get; set; }
        public _ModalCreateFeriadoModel(BdbioAdminSqlContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
           
        }
        public void OnPostCreateFeriado()
        {

        }
    }
}
