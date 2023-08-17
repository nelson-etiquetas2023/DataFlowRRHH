using DataFlowRRHH.Models;
using DataFlowRRHH.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataFlowRRHH.Pages
{
    public class IndexModel : PageModel
    {
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
        public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();


        [BindProperty]
        public List<ShiftAssigned> HorariosAsignados { get; set; } = new List<ShiftAssigned>();

        [BindProperty(SupportsGet = true)]
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();

        private List<CampoHorasExtras> FileReportHorasExtras { get; set; } = new();

        public IServiceGestion ServiceGestion { get; set; }


        //checkbox de los formatos de reporte.
        [BindProperty]
        public string FormatoDoc { get; set; } = "pdf";



        public IndexModel(IServiceGestion _ServiceGestion)
        {
            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 6, 0, 0);
            FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);
            ServiceGestion = _ServiceGestion;
        }

        public async Task OnGetAsync()
        {
            //query de registro de huellas de empleados.
            ListaPonches = await ServiceGestion.LoadRegistroMarcasHuellasEmpleados(ToDate, FromDate);

            //calculo de las horas extras.
            Jornadas = ServiceGestion.RunReportCompleteHorasExtras(ListaPonches);
        }

        public async Task<FileContentResult> OnPostRunReports()
        {
            // Generar reporte

            var url = "";
            var typeFile = "";
            var nameFile = "";

            if (FormatoDoc == "pdf")
            {

                url = "https://localhost:5001/api/ReporteHorasExtras/reporthe/pdf";
                typeFile = "application/pdf";
                nameFile = "Reporte.pdf";

            }
            if (FormatoDoc == "excel")
            {
                url = "https://localhost:5001/api/ReporteHorasExtras/reporthe/xls";
                typeFile = "application/xls";
                nameFile = "Reporte.xls";

            }
            if (FormatoDoc == "word")
            {
                url = "https://localhost:5001/api/ReporteHorasExtras/reporthe/word";
                typeFile = "application/word";
                nameFile = "Reporte.doc";
            }

            //Consultas de horas extras.
            ListaPonches = await ServiceGestion.LoadRegistroMarcasHuellasEmpleados(ToDate, FromDate);
            Jornadas = ServiceGestion.RunReportCompleteHorasExtras(ListaPonches);


            foreach (var item in Jornadas)
            {

                double horas_extras = Convert.ToDouble(((DateTime)(item.Mark4_Dt == null ? item.Horario_salida : item.Mark4_Dt) - (DateTime)item.Horario_salida).TotalHours);
                double salarioHora = ServiceGestion.ObtenerSalarioxHora(item.IdUser);

                //string format = @"h\:mm\:ss";
                //TimeSpan time_start = TimeSpan.Parse(TimeString);
                //TimeSpan.TryParseExact(TimeString,format,CultureInfo.InvariantCulture,out time_start);
                //int hour = Convert.ToInt16(item.Mark1.Substring(1, 2));
                //int minutes = Convert.ToInt16(item.Mark1.Substring(4, 2));
                //int seconds = 0;
                //DateTime marca_entrada = new(year, month, day, hour, minutes, seconds);
                FileReportHorasExtras.Add(new CampoHorasExtras
                {
                    UserId = item.IdUser.ToString(),
                    UserName = item.Empleado,
                    Departamento = item.Dpto,
                    Fecha_Marcaje = item.Fecha,
                    Horario_Asignado = item.ShiftName,
                    Hora_Entrada = item.Start_journal.ToString(),
                    Hora_Salida = item.End_journal.ToString(),
                    M1 = item.Mark1,
                    M2 = item.Mark2,
                    M3 = item.Mark3,
                    M4 = item.Mark4,
                    Horas_trabajadas = item.Horas_Jornada,
                    Horas_Extras = horas_extras,
                    Salario = salarioHora,
                    Marcas = item.Ponches
                });
            }

            //consumir la api de reportes.

            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(FileReportHorasExtras);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
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