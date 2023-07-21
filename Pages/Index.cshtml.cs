using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DataFlowRRHH.Models;
using Microsoft.EntityFrameworkCore;

namespace DataFlowRRHH.Pages
{
	public class IndexModel : PageModel
	{
		readonly BdbioAdminSqlContext db;
		public IEnumerable<Record> ListaPonches { get; set; } = new List<Record>();

		[BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
		public DateTime ToDate { get; set; }

		[BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
		public DateTime FromDate { get; set; }

		[BindProperty(SupportsGet = true)]
		public string? SearchString { get; set; }

		[BindProperty(SupportsGet = true)]
        public int IdEmployee { get; set; }


        public IndexModel(BdbioAdminSqlContext _db)
		{
			ToDate = DateTime.Now;
			FromDate = DateTime.Now;
            this.db = _db;
		}


		public async Task OnGetAsync()
		{
			

             var listaPonches = from ponches in db.Records
			           .Include(x => x.IdUserNavigation)
			           .Include(x => x.IdDeviceNavigation)
			           .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
			           .OrderBy(x => x.IdUser).ThenBy(x => x.RecordTime)
			           select ponches;

			//Entra por aqui si es una busqueda por parametro.           
			if (!string.IsNullOrEmpty(SearchString))
			{
				listaPonches = from ponches in db.Records
					   .Include(u => u.IdUserNavigation)
					   .Include(d => d.IdDeviceNavigation)
					   .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
					   .Where(u => u.IdUserNavigation.Name.Contains(SearchString) || 
					   u.IdDeviceNavigation.Description.Contains(SearchString) ||
					   u.IdUserNavigation.IdDepartmentNavigation.Description.Contains(SearchString))
					   .OrderBy(x => x.IdUser).ThenBy(x => x.CreatedDate)
				       select ponches;
			}
			ListaPonches = await listaPonches.ToListAsync();
		}
		public async Task OnPostSubmit() 
		{
           
            var listaPonches = from ponches in db.Records
							   .Include(x => x.IdUserNavigation)
							   .Include(x => x.IdDeviceNavigation)
							   .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
                               .Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate)
                               select ponches;
            
            ListaPonches = await listaPonches.ToListAsync();
        }
		public async Task OnPostSearchProfileEmployee() 
		{
			
			// Consulta de los horarios asignados a los empleados.
			var horarios = await db.Users.Select(x => new { 
				Iduser = x.IdUser,
				Nombre = x.Name,
				Salario = x.HourSalary,
				Horarios = x.UserShiftNavigation.Select(x => new 
				{ 
					x.ShiftId, 
					Name = x.ShiftNavigation.Description,
					x.BeginDate,
					x.EndDate
				}),
				x.IdDepartment,
				DepartamentoName = x.IdDepartmentNavigation.Description
			}).FirstOrDefaultAsync(p => p.Iduser == IdEmployee );



		}


    }
}