using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DataFlowRRHH.Models;
using Microsoft.EntityFrameworkCore;
using DataFlowRRHH.Service;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;


namespace DataFlowRRHH.Pages
{
	public class IndexModel : PageModel
	{
		readonly BdbioAdminSqlContext db;
		public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();

		[BindProperty(SupportsGet = true), DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
		public DateTime ToDate { get; set; } 

        [BindProperty(SupportsGet = true), DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
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

        [BindProperty]
        public List<Jornada> jornadas { get; set; } = new List<Jornada>();



        public ServiceHorasExtras service { get; set; }
        public IndexModel(BdbioAdminSqlContext _db)
		{
            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            FromDate = ToDate.AddMonths(1).AddDays(-1);
            this.db = _db;
            service = new ServiceHorasExtras();
		}

		public async Task OnGetAsync()
		{
            
            //1.-query de ponches
            var ponches = db.Records
            .Select(p => new
            {
                p.IdUser,
                NameUser = p.IdUserNavigation.Name,
                p.RecordTime,
                p.IdUserNavigation.IdDepartmentNavigation.IdDepartment,
                DepartmentName = p.IdUserNavigation.IdDepartmentNavigation.Description,
                p.IdDeviceNavigation.IdDevice,
                DeviceName = p.IdDeviceNavigation.Description,
                Horario = p.IdUserNavigation.UserShiftNavigation
               .Select(g => new { g.ShiftId, g.ShiftNavigation.Description, g.BeginDate, g.EndDate })
               .Where(t => p.RecordTime >= t.BeginDate && p.RecordTime <= t.EndDate)
               .Select(x => new { type_Shift = x.Description!.Contains("Nocturno") ? "N" : "D" })
            }).Select(x => new CamposRegistros
            {
                IdUser = x.IdUser,
                NameUser = x.NameUser,
                RecordTime = x.RecordTime,
                IdDepartment = x.IdDepartment,
                DepartmentName = x.DepartmentName,
                IdDevice = x.IdDevice,
                DeviceName = x.DeviceName,
                Type_Shift = x.Horario.Select(g => g.type_Shift).FirstOrDefault()!
            }).Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate);



            ListaPonches = await ponches.ToListAsync();
            jornadas = service.RunReportCompleteHorasExtras(ListaPonches);




            //         //Entra por aqui si es una busqueda por parametro.           
            //         if (!string.IsNullOrEmpty(SearchString))
            //{
            //	listaPonches = from ponches in db.Records
            //		   .Include(u => u.IdUserNavigation)
            //		   .Include(d => d.IdDeviceNavigation)
            //		   .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
            //		   .Where(u => u.IdUserNavigation.Name.Contains(SearchString) || 
            //		    u.IdDeviceNavigation.Description.Contains(SearchString) ||
            //		    u.IdUserNavigation.IdDepartmentNavigation.Description.Contains(SearchString))
            //                    .OrderBy(x => x.IdUser).ThenBy(x => x.CreatedDate)
            //	       select ponches;
            //}


            //query de busqueda de horario de empleado.







        }
        public void OnPostSubmit() 
		{
           
   //         var listaPonches = from ponches in db.Records
			//				   .Include(x => x.IdUserNavigation)
			//				   .Include(x => x.IdDeviceNavigation)
			//				   .Include(x => x.IdUserNavigation.IdDepartmentNavigation)
   //                            .Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate)
   //                            select ponches;

		
			//ListaPonches = await listaPonches.ToListAsync();
        }

		private void LoadShift() 
		{
            if (IdEmployee == 0) return;

            // Consulta de los horarios asignados a los empleados.
            var horarios = db.Users.Select(x => new
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
            }).FirstOrDefault(p => p.Iduser == IdEmployee);

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