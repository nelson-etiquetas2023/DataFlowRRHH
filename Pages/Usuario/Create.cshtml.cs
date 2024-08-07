using DataFlowRRHH.Models;
using DataFlowRRHH.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace DataFlowRRHH.Pages.Usuario
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public UsuarioModel Usuario { get; set; } = null!;
        public List<TipoUsuario> TipoUsuarios { get; set; } = new List<TipoUsuario>();
        public List<Departament> OptionsDepart { get; set; } = new List<Departament>();
        public IGenericRepository<UsuarioModel> _repository { get; set; }

        public CreateModel(IGenericRepository<UsuarioModel> repository)
        {
            _repository = repository;
            
        }

        public void OnGet()
        {
            LoadOptions();
        }
        public void OnPost()
        {
            Usuario.Active = true;
            _repository.AddAsync(Usuario);
        }

        private void LoadOptions() 
        {
            TipoUsuarios = new()
            {
                new TipoUsuario { Id=1,Name="Administrador" },
                new TipoUsuario { Id=2,Name="User" },
                new TipoUsuario { Id=3,Name="Invitado" }
            };
            OptionsDepart = new()
            {
                new Departament { Id=1,Name="Sistemas"},
                new Departament { Id=2,Name="Recursos Humanos"},
                new Departament { Id=3,Name="Almacen"},
                new Departament { Id=4,Name="Administracion"},
                new Departament { Id=5,Name="Atencion al cliente"}
            };
        }
    }
    
    public class TipoUsuario 
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

    }
    public class Departament 
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
