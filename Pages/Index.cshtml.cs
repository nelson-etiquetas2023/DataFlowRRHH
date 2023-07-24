using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DataFlowRRHH.Models;
using Microsoft.EntityFrameworkCore;
using DataFlowRRHH.Service;
using System.Data;

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

		// parametro de busqueda de la lista de ponches.
		[BindProperty(SupportsGet = true)]
		public string? SearchString { get; set; }

		//datos de los horarios asignados a los empleados
		[BindProperty(SupportsGet = true)]
        public int IdEmployee { get; set; }

		[BindProperty]
		public string NameEmployee { get; set; } = "";

        [BindProperty]
        public decimal Salario { get; set; }

		[BindProperty]
		public string Departamento { get; set; } = "";

		[BindProperty]
		public List<ShiftAssigned> HorariosAsignados { get; set; } = new List<ShiftAssigned>();


        public IndexModel(BdbioAdminSqlContext _db)
		{
			ToDate = DateTime.Now;
			FromDate = DateTime.Now;
            this.db = _db;
		}

		public async Task OnGetAsync()
		{
            //prueba de las funciones de calculo de horas extras.
            ServiceHorasExtras she = new();
			var horario = she.ObtenerHorarioEmpleado(1);
			var parametros1 = she.ObtenerParametrosHorarios(1,0);
            var parametros2 = she.ObtenerParametrosHorarios(1,1);
            var parametros3 = she.ObtenerParametrosHorarios(1,2);
			var sueldo = she.ObtenerSalarioxHora(2);
			DataTable empleados = she.HorariosAsignadosxEmpleados();



            //query 
            var listaPonches = from ponches in db.Records
			           .Include(x => x.IdUserNavigation)
			           .Include(x => x.IdDeviceNavigation)
			           .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
                       .Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate)
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
			if (IdEmployee == 0) return;

			// Consulta de los horarios asignados a los empleados.
			var horarios = await db.Users.Select(x => new
			{
				Iduser = x.IdUser,
				EmpleadoName = x.Name,
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
			}).FirstOrDefaultAsync(p => p.Iduser == IdEmployee);

			NameEmployee = horarios!.EmpleadoName;
			Salario = horarios.Salario;
			Departamento = horarios.DepartamentoName;
			
			var horariosAsig = horarios.Horarios.Select(x => new
			{
				x.ShiftId,
				x.Name,
				x.BeginDate,
				x.EndDate
			}).ToList();

			foreach (var item in horariosAsig) 
			{
                ShiftAssigned record = new()
				{
					Nombre = item.Name!,
					Date_Start = Convert.ToDateTime(item.BeginDate),
                    Date_Finish = Convert.ToDateTime(item.EndDate)
                };
				HorariosAsignados.Add(record);
			}
        }
    }
	public class ShiftAssigned 
	{
		public string Nombre { get; set; } = null!;
        public DateTime Date_Start { get; set; }
        public DateTime Date_Finish { get; set; }
    }
}