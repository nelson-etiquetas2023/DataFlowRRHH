using DataFlowRRHH.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataFlowRRHH.Repositories.Contracts;


namespace DataFlowRRHH.Pages.Usuario
{
    public class IndexModel : PageModel
    {
        public IEnumerable<UsuarioModel> Usuarios { get; set; } = null!;
        public IGenericRepository<UsuarioModel> _repository { get; }

        public IndexModel(IGenericRepository<UsuarioModel> repository)
        {
           _repository = repository;
        }

        public void OnGet()
        {
            Usuarios = _repository.GetAllAsync();
        }
    }
}
