using DataFlowRRHH.Models;
using DataFlowRRHH.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();


        [BindProperty]
        public List<ShiftAssigned> HorariosAsignados { get; set; } = new List<ShiftAssigned>();

        [BindProperty(SupportsGet = true)]
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();

        private List<CampoHorasExtras> FileReportHorasExtras { get; set; } = new();

        public IServiceGestion ServiceGestion { get; set; }
        

        public Email email { get; set; }

        //checkbox de los formatos de reporte.
        [BindProperty]
        public string FormatoDoc { get; set; } = "pdf";

        public List<Feriado> Feriados { get; set; } = new List<Feriado>();

        IConfiguration configuracion;

        public IndexModel(IServiceGestion _ServiceGestion,IConfiguration _configuracion)
        {
            ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 6, 0, 0);
            FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);

			//ToDate = new DateTime(2023, 12, 1, 6, 0, 0);
			//FromDate = ToDate.AddMonths(1).AddDays(-1).AddHours(17).AddMinutes(59).AddMilliseconds(59);

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
        public async Task OnPostSendEmailAsync() 
        {
            
            //email test
            Email emailTest = new Email
            {
                From = "devsoftware.etiquetas@gmail.com",
                To = "test.etiquetas@gmail.com",
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
                       <h4>Empresa : Tienda la Bomba. Sucursal Santo Domingo</h4>
                       </br>
                       <h3>Fecha reporte: </h3>" + DateTime.Now + @"   
                       <h3>Reporte de Tardanzas</h3>
                        <h4>Periodo de Fecha:  [Desde: 01-03-2024 Hasta: 15-03-2024]</h4>
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
                            <tbody>
                                <tr>
                                    <td class=""description"">1</td>
                                    <td>1255</td>
                                    <td>Nelson Pino</td>
                                    <td>01-03-2024</td>
                                    <td>9:21 a.m.</td>
                                    <td class=""cell-tar"">21 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">2</td>
                                    <td>0054</td>
                                    <td>Rosa Martinez</td>
                                    <td>01-03-2024</td>
                                    <td>9:18 a.m.</td>    
                                    <td class=""cell-tar"">18 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">3</td>
                                    <td>1734</td>
                                    <td>Ana Minaya</td>
                                    <td>02-03-2024</td>
                                    <td>9:27 a.m.</td>    
                                    <td class=""cell-tar"">27 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">4</td>
                                    <td>6701</td>
                                    <td>Juan Carlos Rojas</td>
                                    <td>03-03-2024</td>
                                    <td>9:31 a.m.</td>    
                                    <td class=""cell-tar"">31 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">5</td>
                                    <td>8174</td>
                                    <td>Janet Peña</td>
                                    <td>03-03-2024</td>
                                    <td>9:37 a.m.</td>    
                                    <td class=""cell-tar"">37 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">6</td>
                                    <td>8501</td>
                                    <td>Milagros Lopez</td>
                                    <td>02-03-2024</td>
                                    <td>9:41 a.m.</td>    
                                    <td class=""cell-tar"">41 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">7</td>
                                    <td>8501</td>
                                    <td>Juan Pablos Torres</td>
                                    <td>05-03-2024</td>
                                    <td>9:38 a.m.</td>    
                                    <td class=""cell-tar"">38 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">8</td>
                                    <td>1104</td>
                                    <td>Rodolfo Hernandez</td>
                                    <td>06-03-2024</td>
                                    <td>9:18 a.m.</td>    
                                    <td class=""cell-tar"">18 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">9</td>
                                    <td>1104</td>
                                    <td>Rodolfo Hernandez</td>
                                    <td>06-03-2024</td>
                                    <td>9:18 a.m.</td>    
                                    <td class=""cell-tar"">18 min.</td>
                                </tr>
                                <tr>
                                    <td class=""description"">10</td>
                                    <td>1104</td>
                                    <td>Rodolfo Hernandez</td>
                                    <td>06-03-2024</td>
                                    <td>9:18 a.m.</td>    
                                    <td class=""cell-tar"">18 min.</td>
                                </tr>
                            </tbody>
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
                            <tbody>
                                <tr>
                                    <td class=""description"">1</td>
                                    <td>1255</td>
                                    <td>Rodolfo Hernandez</td>
                                    <td class=""cell-tar"">3</td>
                                </tr>
                                <tr>
                                    <td class=""description"">2</td>
                                    <td>0054</td>
                                    <td>Nelson Pino</td>
                                    <td class=""cell-tar"">1</td>
                                    
                                </tr>
                                <tr>
                                    <td class=""description"">3</td>
                                    <td>1734</td>
                                    <td>Milagros Lopez</td>
                                    <td class=""cell-tar"">1</td>
                                </tr>
                                <tr>
                                    <td class=""description"">4</td>
                                    <td>6701</td>
                                    <td>Ana minaya</td>
                                    <td class=""cell-tar"">1</td>
                                </tr>
                            </tbody>
                        </table>



                        <h4>Departamento de Sistemas:</h4> 
                        <p>Correo: devsoftware.etiquetas@gmail.com</p>
                        <p>Telefono Contacto: 826-9550-10</p>
                        <p>Etiquetas.com.do</p>

                    </body>
                </html>"
            };
            using (var smtp = new SmtpClient("smtp.gmail.com",587)) 
            {
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
                NetworkCredential nc = new NetworkCredential("devsoftware.etiquetas@gmail.com", "spmg ejwa qqdg znoy");
                //smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                await smtp.SendMailAsync(msg);
            }             
        }
        public async Task<JsonResult> OnPostLoadPonches() 
        {
            ListaPonches = await ServiceGestion.LoadHuellasEmpleados(ToDate, FromDate);
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