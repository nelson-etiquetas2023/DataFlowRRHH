using DataFlowRRHH.Models;
using DataFlowRRHH.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();
        public ServiceHorasExtras Service { get; set; }


        //checkbox de los formatos de reporte.
        [BindProperty]
        public string FormatoDoc { get; set; } = "pdf";
       


        public IndexModel(BdbioAdminSqlContext _db)
        {


            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 6, 0, 0);
            FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);
            this.db = _db;
            Service = new ServiceHorasExtras();
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
                indexday = Convert.ToInt32(p.RecordTime.DayOfWeek - 1) == -1 ? 6 : Convert.ToInt32(p.RecordTime.DayOfWeek - 1),
                p.IdUserNavigation.IdDepartmentNavigation.IdDepartment,
                DepartmentName = p.IdUserNavigation.IdDepartmentNavigation.Description,
                p.IdDeviceNavigation.IdDevice,
                p.IdUserNavigation.UserShiftNavigation,
                DeviceName = p.IdDeviceNavigation.Description,
                Horario = p.IdUserNavigation.UserShiftNavigation
               .Select(g => new
               {
                   id = g.ShiftId,
                   g.ShiftNavigation.Description,
                   g.BeginDate,
                   g.EndDate,
               })
               .Where(t => p.RecordTime >= t.BeginDate && p.RecordTime <= t.EndDate)
               .Select(w => new
               {
                   iddshift = w.id ?? 0,
                   shiftname = w.Description,
                   type_Shift = w.Description!.Contains("Nocturno") ? "N" : "D",
                   start_journal = db.ShiftDetails.Where(x => x.ShiftId == w.id && x.DayId == 0).Select(x => x.T2inHour).FirstOrDefault(),
                   end_journal = db.ShiftDetails.Where(x => x.ShiftId == w.id && x.DayId == 0).Select(x => x.T2outHour).FirstOrDefault()
               })

            }).Select(x => new CamposRegistros
            {
                IdUser = x.IdUser,
                NameUser = x.NameUser,
                RecordTime = x.RecordTime,
                IdDepartment = x.IdDepartment,
                DepartmentName = x.DepartmentName,
                IdDevice = x.IdDevice,
                IdShift = x.Horario.Select(x => x.iddshift).SingleOrDefault(),
                DeviceName = x.DeviceName,
                Type_Shift = x.Horario.Select(g => g.type_Shift).FirstOrDefault()!,
                ShiftName = x.Horario.Select(g => g.shiftname).FirstOrDefault()!,
                Indexday = x.indexday,
                Start_journal = x.Horario.Select(g => g.start_journal).FirstOrDefault(),
                End_journal = x.Horario.Select(g => g.end_journal).FirstOrDefault(),
            }).Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate);

            ListaPonches = await ponches.ToListAsync();
            //calculo de las horas extras.
            Jornadas = Service.RunReportCompleteHorasExtras(ListaPonches);




        }


        public async Task<FileContentResult> OnPostRunReports()
        {
            // Generar reporte

            var url = "";
            var typeFile = "";
            var nameFile = "";

            if (FormatoDoc == "pdf")
            {
                url = "http://192.168.10.13:8085/api/Report/UserDetails/pdf/en";
                typeFile = "application/pdf";
                nameFile = "Reporte.pdf";

            }
            if (FormatoDoc == "excel")
            {
                url = "http://192.168.10.13:8085/api/Report/UserDetails/xls/en";
                typeFile = "application/xls";
                nameFile = "Reporte.xls";

            }
            if (FormatoDoc == "word")
            {
                url = "http://192.168.10.13:8085/api/Report/UserDetails/word/en";
                typeFile = "application/word";
                nameFile = "Reporte.doc";

            }
            //Consumir la api.
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            byte[] responseBody = await response.Content.ReadAsByteArrayAsync();
            return File(responseBody, typeFile, nameFile);
        }
    }





    public class ShiftAssigned
    {
        public string Nombre { get; set; } = null!;
        public DateTime Date_Start { get; set; }
        public DateTime Date_Finish { get; set; }
    }
}