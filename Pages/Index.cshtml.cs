using DataFlowRRHH.Models;
using DataFlowRRHH.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
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
        public List<ShiftAssigned> HorariosAsignados { get; set; } = new List<ShiftAssigned>();

        private List<CampoHorasExtras> FileReportHorasExtras { get; set; } = new();

        public IServiceGestion ServiceGestion { get; set; }

        public Email Email { get; set; } = null!;

        //checkbox de los formatos de reporte.
        [BindProperty]
        public string FormatoDoc { get; set; } = "pdf";

        
        readonly IConfiguration configuracion;

        [BindProperty]
        public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();

        [BindProperty]
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();

        public BdbioAdminSqlContext Context { get; set; }

        public IndexModel(IServiceGestion _ServiceGestion, IConfiguration _configuracion, BdbioAdminSqlContext context)
        {
            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 6, 0, 0);
            FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);
			ServiceGestion = _ServiceGestion;
            configuracion = _configuracion;
            Context = context;
        }

        public async Task OnGetAsync()
        {
            //query de registro de huellas de empleados.
            ListaPonches = await ServiceGestion.LoadHuellasEmpleados(ToDate, FromDate);
            
            //calculo de las horas extras.
            Jornadas = ServiceGestion.CalcularHorasExtras(ListaPonches);
        }
        public async Task OnPostSendEmailAsync() 
        {

            //query de registro de huellas de empleados.
            ListaPonches = await ServiceGestion.LoadHuellasEmpleados(ToDate, FromDate);
            //Tabla Feriados.
            //Feriados = ServiceGestion.GetDataFeriados();
            //calculo de las horas extras.
            Jornadas = ServiceGestion.CalcularHorasExtras(ListaPonches);

            //consulta de detalle de tardanzas.
            var query = from data in Jornadas
                        where data.Tardanza_Entrada > 0
                        orderby data.IdUser
                        select new 
                        {
                            it = 0,
                            id_emple = data.IdUser,
                            nombre_emple = data.Empleado,
                            fecha_ponche = data.Fecha.ToShortDateString(),
                            hora_ponche = data.Fecha.ToShortTimeString(),
                            tardanza_minutos = data.Tardanza_Entrada
                        };
            //consulta de resumen de tardanzas.
            var resumen = (from item in query.ToList()
                          group item by item.id_emple into grp
                          select new
                          {
                              id = grp.Key,
                              cantidad = grp.Count(),
                              empleado = grp.FirstOrDefault()!.nombre_emple
                          }).OrderByDescending(x => x.cantidad);




            string titulo_empresa = "Tienda la Bomda - Sucursal Santo Domingo";
            string renglones = "";
            int numfilas = 0;
            string rango_fechas = "[ " + ToDate.ToString() + " - " + FromDate.ToString() + " ]";
            foreach (var item in query) 
            {
                numfilas += 1;
                renglones += @"<tr>
                                    <td class=""description"">" + numfilas + @"</td>
                                    <td style=""text-align: center;"">" + item.id_emple + @"</td>
                                    <td>" + item.nombre_emple + @"</td>
                                    <td>" + item.fecha_ponche + @"</td>
                                    <td style=""text-align: center;"">" + item.hora_ponche + @"</td>
                                    <td class=""cell-tar"">" + item.tardanza_minutos + @" Min.</td>
                              </tr>";
            }

            int filas_resumen = 0;
            string renglones_resumen = "";
            foreach (var item in resumen) 
            {
                filas_resumen += 1;
                renglones_resumen += @"<tr>
                                         <td class=""description"">" + filas_resumen + @"</td>
                                         <td>" + item.id + @"</td>
                                         <td>"+ item.empleado + @"</td>
                                         <td class=""cell-tar"">"+ item.cantidad +@"</td>
                                       </tr>";
            }
            
            //email test
            Email emailTest = new()
            {
                From = "devsoftware.etiquetas@gmail.com",
                To = "recursoshumanos@tiendalabomba.com",
                Subject = "SISTEMA CONTROL DE ASISTENCIA - Reporte de Tardanzas: " + DateTime.Now,
                Body = @"
                <html lang=""es"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            Reportes - Dataflow
                        </title>
                        <style type=""text/css"">
                            HTML{background-color: #e8e8e8;}
                            .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                            .courses-table .description{color: #505050;}
                            .courses-table thead tr:nth-child(even){background-color: #D3F7B9;}
                            .courses-table td{border: 1px solid #D1D1D1; background-color: #F3F3F3; padding: 0 10px;}
                            .courses-table th{border: 1px solid #424242; color: #FFFFFF;text-align: left; padding: 0 10px;}
                            .green{background-color: #6B9852;}
                            .cell-tar{font-weight: bold;text-align: center;}
                            tr:nth-child(even){background-color: #D3F7B9;}
                        </style>
                    </head>
                    <body>
                       <h1>Sistema Control de Asistencia.</h1>
                       <h4>Empresa : </h4>" + titulo_empresa + @"
                       </br>
                       <h3>Fecha reporte: </h3>" + DateTime.Now + @"   
                       <h3>Reporte detalle de Tardanzas</h3>
                        <h4>Periodo de Fecha:  " + rango_fechas + @"</h4>
                       <table class=""courses-table"">
                            <thead>
                                <tr>
                                    <th class=""green"">It.</th>
                                    <th class=""green"">Id Empleado</th>
                                    <th class=""green"">Nombre de Empleado</th>
                                    <th class=""green"">Fecha</th>
                                    <th class=""green"">Ponche Entrada</th>
                                    <th class=""green"">Tardanza (Min)</th>
                                </tr>
                            </thead>
                            <tbody>"+ 
                            renglones 
                            +@"</tbody>
                        </table>
                        <h4>* Se considera tardanza a los empleados que marquen 15 minutos porterior
                              a la hora de entrada establecida para su jornada diaria</h4>
                        </br>
                        <h3>Resumen de Emplados con mas Tardanzas</h3>
                        <table class=""courses-table"">
                            <thead>
                                <tr>
                                    <th class=""green"">It.</th>
                                    <th class=""green"">Id Empleado</th>
                                    <th class=""green"">Nombre de Empleado</th>
                                    <th class=""green"">Numero Tardanzas</th>
                                </tr>
                            </thead>
                            <tbody>" + renglones_resumen + @"
                            </tbody>
                        </table>
                        <h4>Departamento de Sistemas:</h4> 
                        <p>Correo: devsoftware.etiquetas@gmail.com</p>
                        <p>Telefono Contacto: 829-695-1050</p>
                        <p>Etiquetas.com.do</p>

                    </body>
                </html>"
            };
            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            //parametros protocolo smtp
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            //smtp.PickupDirectoryLocation = @"c:\Mymails";

            //mensaje 
            var msg = new MailMessage
            {
                Body = emailTest.Body,
                Subject = emailTest.Subject,
                From = new MailAddress(emailTest.From),
                IsBodyHtml = true
            };
            msg.To.Add(emailTest.To);

            //credenciales
            NetworkCredential nc = new("devsoftware.etiquetas@gmail.com", "spmg ejwa qqdg znoy");
            //smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            await smtp.SendMailAsync(msg);
        }
        public JsonResult OnPostLoadPonches() 
        {
           
            return new JsonResult(ListaPonches);
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
            Jornadas = ServiceGestion.CalcularHorasExtras(ListaPonches);


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
            ServiceGestion.CalculoEscalasDeHorarios(FileReportHorasExtras);

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