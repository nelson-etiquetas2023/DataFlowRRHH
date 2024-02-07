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

        public List<Feriado> Feriados { get; set; } = new List<Feriado>();

        IConfiguration configuracion;

        public IndexModel(IServiceGestion _ServiceGestion,IConfiguration _configuracion)
        {
            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 6, 0, 0);
            FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);
            ServiceGestion = _ServiceGestion;
            configuracion = _configuracion;
        }

        public async Task OnGetAsync()
        {
            //query de registro de huellas de empleados.
            ListaPonches = await ServiceGestion.LoadHuellasEmpleados(ToDate, FromDate);

            //Tabla Feriados.
            Feriados = ServiceGestion.GetDataFeriados();

            //calculo de las horas extras.
            Jornadas = ServiceGestion.CalcularHorasExtras(ListaPonches,Feriados);
          
        }

        public async Task<FileContentResult> OnPostRunReports()
        {

            // extraer los datos de los endpoint desde appsetting.json
            //Cambiar aqui las url dependiendo del servidor dependiendo donse se desplegara.
            string conn = "EndPointUrlReportsEtiquetas";
            //----------------------------------------------------------------------------//

            // Generar reporte

            var url = "";
            var typeFile = "";
            var nameFile = "";

            if (FormatoDoc == "pdf")
            {
                url = configuracion.GetSection(conn).GetSection("EndPointPdf").Value;
                typeFile = "application/pdf";
                nameFile = "Reporte.pdf";
            }
            if (FormatoDoc == "excel")
            {
                url = configuracion.GetSection(conn).GetSection("EndPointExcel").Value;
                typeFile = "application/xls";
                nameFile = "Reporte.xls";
            }
            if (FormatoDoc == "word")
            {
                url = configuracion.GetSection(conn).GetSection("EndPointWord").Value;
                typeFile = "application/word";
                nameFile = "Reporte.doc";
            }

            //Consultas de horas extras.
            ListaPonches = await ServiceGestion.LoadHuellasEmpleados(ToDate, FromDate);
            Jornadas = ServiceGestion.CalcularHorasExtras(ListaPonches,Feriados);


            foreach (var item in Jornadas)
            {

                double horas_extras = Convert.ToDouble(((DateTime)(item.Mark4_Dt == null ? item.Horario_salida : item.Mark4_Dt) - (DateTime)item.Horario_salida).TotalHours);
                double salarioHora = ServiceGestion.ObtenerSalarioxHora(item.IdUser);
                //deteccion de las tardanzas.

                DateTime dt_HorarioEntrada = new(item.Horario_salida.Year, item.Horario_salida.Month,
                    item.Horario_salida.Day,Convert.ToInt16(item.Start_journal_hour),0,0);
                
                // en la entrada de la jornada.
                item.Lapso_tardanza_entrada =  (((DateTime)(item.Mark1_Dt == null ? DateTime.Today : item.Mark1_Dt)).Subtract(dt_HorarioEntrada)).TotalMinutes;
                // tardanza en la hora de comida.
                item.Lapso_tardanza_almuerzo = (((DateTime)(item.Mark3_Dt == null ? DateTime.Today : item.Mark3_Dt)).Subtract(((DateTime)(item.Mark2_Dt == null ? DateTime.Today : item.Mark2_Dt)))).TotalMinutes;

                
                
                

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
                    Hora_Entrada = item.Start_journal_hour.ToString(),
                    Hora_Salida = item.End_journal.ToString(),
                    M1 = item.Mark1,
                    M2 = item.Mark2,
                    M3 = item.Mark3,
                    M4 = item.Mark4,
                    Mark1_Dt = item.Mark1_Dt,
                    Mark2_Dt = item.Mark2_Dt,
                    Mark3_Dt = item.Mark3_Dt,
                    Mark4_Dt = item.Mark4_Dt,
                    Horas_trabajadas = item.Horas_Jornada,
                    Horas_Extras = horas_extras,
                    Salario = salarioHora,
                    Marcas = item.Ponches,
                    lapso_tardanza_entrada = item.Lapso_tardanza_entrada < 0 ? 0 : item.Lapso_tardanza_entrada,
                    tardanza_entrada_jornada =  item.Lapso_tardanza_entrada > 15,
                    lapso_tardanza_almuerzo = item.Lapso_tardanza_almuerzo,
                    tardanza_almuerzo_jornada = item.Lapso_tardanza_almuerzo > 60
                });
            }

            //Calculo de Condiciones por escalas Hoprarios.
            ServiceGestion.CalculoEscalasDeHorarios(FileReportHorasExtras, Feriados);

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