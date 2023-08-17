using DataFlowRRHH.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataFlowRRHH.Service
{
    public interface IServiceGestion
    {
        Task<List<CamposRegistros>> LoadRegistroMarcasHuellasEmpleados(DateTime ToDate, DateTime FromDate);
        List<Jornada> RunReportCompleteHorasExtras(List<CamposRegistros> listaponches);
        double ObtenerSalarioxHora(int userid);
    }
    public class ServiceGestion : IServiceGestion
    {
        //conexion con entityframwork
        public BdbioAdminSqlContext db;
        public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();

        //Conexion con ado.net
        public DataTable DtHorasExtras = new();
        public SqlConnection micomm;

        public ServiceGestion(BdbioAdminSqlContext _db)
        {
            db = _db;
            micomm = new SqlConnection
            {
                ConnectionString = @"Data Source=SERVER-ETIQUETA;Initial Catalog=BDBioAdminSQL;User Id=Npino;Password=Jossycar5%;TrustServerCertificate=True;"
            };

        }
        public async Task<List<CamposRegistros>> LoadRegistroMarcasHuellasEmpleados(DateTime ToDate, DateTime FromDate)
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

            return ListaPonches = await ponches.ToListAsync();
        }
        public List<Jornada> RunReportCompleteHorasExtras(List<CamposRegistros> listaponches)
        {
            //hay que traer la entrada y salida del los horarios asignados.
            //TimeSpan Entrada_Horario = new(18, 0, 0);
            //TimeSpan Salida_Horario = new(6, 0, 0);
            //TimeSpan tsresult = Entrada_Horario.Subtract(Salida_Horario);

            var jornadas = (from q in listaponches
                            group q by new
                            {
                                q.IdUser,
                                fecha = string.IsNullOrEmpty(q.Type_Shift).Equals("N") ?
                                             q.RecordTime.AddHours(15).Date :
                                             q.RecordTime.Date
                            } into grp
                            select new Jornada
                            {
                                IdUser = grp.Key.IdUser,
                                Empleado = grp.FirstOrDefault()!.NameUser,
                                Dpto = grp.FirstOrDefault()!.DepartmentName,
                                Fecha = grp.FirstOrDefault()!.RecordTime,
                                Horario_salida = new DateTime(grp.FirstOrDefault()!.RecordTime.Year,
                                grp.FirstOrDefault()!.RecordTime.Month,
                                grp.FirstOrDefault()!.RecordTime.Day, grp.FirstOrDefault()!.End_journal, 0, 0),
                                Mark1 = Convert.ToString(grp.FirstOrDefault()!.RecordTime.ToShortTimeString()),
                                Mark2 = grp.Count() > 1 ? Convert.ToString(grp.ElementAtOrDefault(1)!.RecordTime.ToShortTimeString()) : "",
                                Mark3 = grp.Count() > 2 ? Convert.ToString(grp.ElementAtOrDefault(2)!.RecordTime.ToShortTimeString()) : "",
                                Mark4 = grp.Count() > 3 ? Convert.ToString(grp.ElementAtOrDefault(3)!.RecordTime.ToShortTimeString()) : "",

                                Mark1_Dt = grp.FirstOrDefault()!.RecordTime,
                                Mark2_Dt = grp.ElementAtOrDefault(1)?.RecordTime,
                                Mark3_Dt = grp.ElementAtOrDefault(2)?.RecordTime,
                                Mark4_Dt = grp.ElementAtOrDefault(3)?.RecordTime,
                                Ponches = grp.Count(),
                                Horas_Jornada = Convert.ToDouble((grp.LastOrDefault()!.RecordTime - grp.FirstOrDefault()!.RecordTime).TotalHours),
                                Type_shift = grp.FirstOrDefault()!.Type_Shift,
                                ShiftName = grp.FirstOrDefault()!.ShiftName,
                                Start_journal = grp.FirstOrDefault()!.Start_journal,
                                End_journal = grp.FirstOrDefault()!.End_journal,
                            }).OrderBy(o => o.IdUser).ToList();


            return jornadas;
        }
        public void RunReportDetailsHorasExtras(List<Record> listaponches)
        {
            if (listaponches == null)
            {
                return;
            }
            //definir las columnas del reporte
            DtHorasExtras.Columns.Add("UserId", typeof(int));
            DtHorasExtras.Columns.Add("UserName", typeof(string));
            DtHorasExtras.Columns.Add("Jornada", typeof(string));
            DtHorasExtras.Columns.Add("Departamento", typeof(string));
            DtHorasExtras.Columns.Add("FechaMarca", typeof(DateTime));
            DtHorasExtras.Columns.Add("HorasExtras", typeof(decimal));
            DtHorasExtras.Columns.Add("Factor", typeof(int));
            DtHorasExtras.Columns.Add("Salario", typeof(double));
            DtHorasExtras.Columns.Add("SalarioFraccion", typeof(double));
            DtHorasExtras.Columns.Add("Monto", typeof(double));
            ServiceHorasExtras she = new();
            //reporte horas extras
            foreach (var item in listaponches)
            {
                int userid = Convert.ToInt16(item.IdUser);
                int shiftf = she.ObtenerHorarioEmpleado(userid);
                int indexday = Convert.ToInt16(item.RecordTime.DayOfWeek - 1);
                if (indexday == -1) indexday = 6;
                item.RecordTime.ToString("dddd");
                //Parametros de los horarios
                ShiftAssingEmployeeRow hfp = she.ObtenerParametrosHorarios(shiftf, indexday);
                //guarde la jornada en external reference.
                item.IdUserNavigation.ExternalReference = "de: " + hfp.Jornada_Start + " a: " + hfp.Jornada_End;
                item.IdUserNavigation.IdDepartmentNavigation.Description = she.GetDepartamento(userid);
                //nivel 1 de condiciones
                var ch1_str1 = item.RecordTime.ToShortDateString() + " " + hfp.Cal1_Start + " :00:00 PM";
                var ch1_str2 = item.RecordTime.ToShortDateString() + " " + hfp.Cal1_End + " :00:00 PM";
                DateTime ch1_date1 = Convert.ToDateTime(ch1_str1);
                DateTime ch1_date2 = Convert.ToDateTime(ch1_str2);
                int ch1_factor = Convert.ToInt16(hfp.Cal1_Factor);
                TimeSpan ch1 = ch1_date2 - ch1_date1;
                //nivel 2 de condiciones
                var ch2_str1 = item.RecordTime.ToShortDateString() + " " + hfp.Cal2_Start + " :00:00 PM";
                var ch2_str2 = item.RecordTime.ToShortDateString() + " " + hfp.Cal2_End + " :00:00 PM";
                DateTime ch2_date1 = Convert.ToDateTime(ch2_str1);
                DateTime ch2_date2 = Convert.ToDateTime(ch2_str2);
                int ch2_factor = Convert.ToInt16(hfp.Cal2_Factor);
                TimeSpan ch2 = ch2_date2 - ch2_date1;
                //nivel 3 de condiciones
                var ch3_str1 = item.RecordTime.ToShortDateString() + " " + hfp.Cal3_Start + " :00:00 PM";
                var ch3_str2 = item.RecordTime.ToShortDateString() + " " + hfp.Cal3_End + " :00:00 PM";
                DateTime ch3_date1 = Convert.ToDateTime(ch3_str1);
                DateTime ch3_date2 = Convert.ToDateTime(ch3_str2);
                int ch3_factor = Convert.ToInt16(hfp.Cal3_Factor);
                TimeSpan ch3 = ch3_date2 - ch3_date1;

                //hay horas extras que calcular
                if (item.RecordTime > ch1_date1)
                {
                    TimeSpan het = item.RecordTime - ch1_date1;
                    Boolean run;
                    //Obtener el salario x Hora.
                    double sh = she.ObtenerSalarioxHora(userid);

                    // Condiciones del primer nivel.
                    if (het >= ch1)
                    {
                        DataRow row = DtHorasExtras.NewRow();
                        row["UserId"] = item.IdUser;
                        row["UserName"] = item.IdUserNavigation.Name;
                        row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                        row["Jornada"] = item.IdUserNavigation.ExternalReference;
                        row["FechaMarca"] = item.RecordTime.ToShortDateString();
                        double horas_extras = (ch1.TotalMinutes / 60);
                        row["HorasExtras"] = horas_extras;
                        row["Factor"] = ch1_factor;
                        row["Salario"] = sh;
                        double salario_factor = (sh * ch1_factor) / 100;
                        row["SalarioFraccion"] = salario_factor;
                        row["Monto"] = Math.Round((salario_factor * horas_extras), 2, MidpointRounding.AwayFromZero);
                        DtHorasExtras.Rows.Add(row);
                        run = true;
                    }
                    else
                    {
                        DataRow row = DtHorasExtras.NewRow();
                        row["UserId"] = item.IdUser;
                        row["UserName"] = item.IdUserNavigation.Name;
                        row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                        row["Jornada"] = item.IdUserNavigation.ExternalReference;
                        row["FechaMarca"] = item.RecordTime.ToShortDateString();
                        double horas_extras = Math.Round((het.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                        row["HorasExtras"] = horas_extras;
                        row["Factor"] = ch1_factor;
                        row["Salario"] = sh;
                        double salario_factor = (sh * ch1_factor) / 100;
                        row["SalarioFraccion"] = salario_factor;
                        row["Monto"] = Math.Round((salario_factor * horas_extras), 2, MidpointRounding.AwayFromZero);
                        DtHorasExtras.Rows.Add(row);
                        run = false;
                    }

                    // Condiciones de Segundo Nivel.
                    if (run)
                    {
                        TimeSpan het1 = item.RecordTime - ch1_date2;
                        if (het1 >= ch2)
                        {
                            DataRow row = DtHorasExtras.NewRow();
                            row["UserId"] = item.IdUser;
                            row["UserName"] = item.IdUserNavigation.Name;
                            row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                            row["Jornada"] = item.IdUserNavigation.ExternalReference;
                            row["FechaMarca"] = item.RecordTime.ToShortDateString();
                            double horas_extras = Math.Round((het1.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                            row["HorasExtras"] = horas_extras;
                            row["Factor"] = ch2_factor;
                            row["Salario"] = sh;
                            double salario_factor = (sh * ch2_factor) / 100;
                            row["SalarioFraccion"] = salario_factor;
                            row["Monto"] = Math.Round((salario_factor * horas_extras), 2, MidpointRounding.AwayFromZero);
                            DtHorasExtras.Rows.Add(row);
                            run = true;
                        }
                        else
                        {
                            DataRow row = DtHorasExtras.NewRow();
                            row["UserId"] = item.IdUser;
                            row["UserName"] = item.IdUserNavigation.Name;
                            row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                            row["Jornada"] = item.IdUserNavigation.ExternalReference;
                            row["FechaMarca"] = item.RecordTime.ToShortDateString();
                            double horas_extras = Math.Round((het1.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                            row["HorasExtras"] = horas_extras;
                            row["Factor"] = ch2_factor;
                            row["Salario"] = sh;
                            double salario_factor = (sh * ch2_factor) / 100;
                            row["SalarioFraccion"] = salario_factor;
                            row["Monto"] = Math.Round((salario_factor * horas_extras), 2, MidpointRounding.AwayFromZero);
                            DtHorasExtras.Rows.Add(row);
                            run = false;
                        }
                    }

                    // Condiciones de Tercer Nivel.
                    if (run)
                    {
                        TimeSpan het2 = item.RecordTime - ch3_date1;
                        if (het2 >= ch2)
                        {
                            DataRow row = DtHorasExtras.NewRow();
                            row["UserId"] = item.IdUser;
                            row["UserName"] = item.IdUserNavigation.Name;
                            row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                            row["Jornada"] = item.IdUserNavigation.ExternalReference; row["FechaMarca"] = item.RecordTime.ToShortDateString();
                            row["HorasExtras"] = (ch2.TotalMinutes / 60);
                            row["Factor"] = ch3_factor;
                            row["Salario"] = sh;
                            row["SalarioFraccion"] = (sh * ch3_factor) / 100;
                            DtHorasExtras.Rows.Add(row);
                            run = true;
                        }
                        else
                        {
                            DataRow row = DtHorasExtras.NewRow();
                            row["UserId"] = item.IdUser;
                            row["UserName"] = item.IdUserNavigation.Name;
                            row["Departamento"] = item.IdUserNavigation.IdDepartmentNavigation.Description;
                            row["Jornada"] = item.IdUserNavigation.ExternalReference;
                            row["FechaMarca"] = item.RecordTime.ToShortDateString();
                            row["HorasExtras"] = Math.Round((het2.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                            row["Factor"] = ch3_factor;
                            row["Salario"] = sh;
                            row["SalarioFraccion"] = (sh * ch3_factor) / 100;
                            DtHorasExtras.Rows.Add(row);
                            run = false;
                        }
                    }
                }
            }
        }
        public int ObtenerHorarioEmpleado(int userid)
        {
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,
                CommandText = "SELECT ShiftId FROM [BDBioAdminSQL].[dbo].[UserShift] WHERE IdUser=@p1"
            };
            SqlParameter p1 = new("@p1", userid);
            comando.Parameters.Add(p1);
            micomm.Open();
            int result = Convert.ToInt16(comando.ExecuteScalar());
            micomm.Close();
            return result;
        }
        public ShiftAssingEmployeeRow ObtenerParametrosHorarios(int shiftid, int dayid)
        {
            //parametros de horario fijo.
            ShiftAssingEmployeeRow hfp = new();
            //comando sql
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,
                CommandText = "SELECT b.IdUser,c.Name,a.ShiftId,DayId,description,type,t2inhour,t2outhour," +
            "t2overtime1beginhour,t2overtime1endhour,t2overtime1factor,t2overtime2beginhour,t2overtime2endhour," +
            "t2overtime2factor,t2overtime3beginhour,t2overtime3endhour,t2overtime3factor,t2overtime4beginhour," +
            "t2overtime4endhour,t2overtime4factor,t2overtime5beginhour,t2overtime5endhour,t2overtime5factor " +
            "FROM [BDBioAdminSQL].[dbo].[ShiftDetail] a left join [BDBioAdminSQL].[dbo].[UserShift] b " +
            "on a.ShiftId = b.ShiftId left join [BDBioAdminSQL].[dbo].[User] c on c.IdUser = b.IdUser " +
            "where a.ShiftId=@p1 and DayId=@p2 order by DayId"
            };
            SqlParameter p1 = new("@p1", shiftid);
            SqlParameter p2 = new("@p2", dayid);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            micomm.Open();
            comando.ExecuteNonQuery();
            SqlDataAdapter da = new();
            DataTable dt = new();
            da.SelectCommand = comando;
            da.Fill(dt);
            // validar que tenga registros.
            if (dt.Rows.Count > 0)
            {
                hfp.Jornada_Start = dt.Rows[0]["t2inhour"].ToString()!;
                hfp.Jornada_End = dt.Rows[0]["t2outhour"].ToString()!;
                hfp.Cal1_Start = dt.Rows[0]["t2overtime1beginhour"].ToString()!;
                hfp.Cal1_End = dt.Rows[0]["t2overtime1endhour"].ToString()!;
                hfp.Cal1_Factor = dt.Rows[0]["t2overtime1factor"].ToString()!;
                hfp.Cal2_Start = dt.Rows[0]["t2overtime2beginhour"].ToString()!;
                hfp.Cal2_End = dt.Rows[0]["t2overtime2endhour"].ToString()!;
                hfp.Cal2_Factor = dt.Rows[0]["t2overtime2factor"].ToString()!;
                hfp.Cal3_Start = dt.Rows[0]["t2overtime3beginhour"].ToString()!;
                hfp.Cal3_End = dt.Rows[0]["t2overtime3endhour"].ToString()!;
                hfp.Cal3_Factor = dt.Rows[0]["t2overtime3factor"].ToString()!;
                hfp.Cal4_Start = dt.Rows[0]["t2overtime4beginhour"].ToString()!;
                hfp.Cal4_End = dt.Rows[0]["t2overtime4endhour"].ToString()!;
                hfp.Cal4_Factor = dt.Rows[0]["t2overtime4factor"].ToString()!;
                hfp.Cal5_Start = dt.Rows[0]["t2overtime5beginhour"].ToString()!;
                hfp.Cal5_End = dt.Rows[0]["t2overtime5endhour"].ToString()!;
                hfp.Cal5_Factor = dt.Rows[0]["t2overtime5factor"].ToString()!;
            }
            micomm.Close();
            return hfp;
        }
        public double ObtenerSalarioxHora(int userid)
        {
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,
                CommandText = "SELECT HourSalary FROM [BDBioAdminSQL].[dbo].[User] WHERE IdUser=@p1"
            };
            SqlParameter p1 = new("@p1", userid);
            comando.Parameters.Add(p1);
            micomm.Open();
            double SalarioHora = Convert.ToDouble(comando.ExecuteScalar());
            micomm.Close();
            return SalarioHora;
        }
        public List<ShifthAssingEmployeeDetails> HorariosAsignadosxEmpleados()
        {
            DataTable dt = new();
            //comando sql para traer los empleados del bioadmin.
            dt.Clear();
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text
            };
            comando.CommandText = comando.CommandText = "SELECT a.[IdUser],(ISNULL(b.[UserShiftId], '')) as [UserShiftId]," +
                "(ISNULL(b.[ShiftId], '')) as [ShiftId], (ISNULL(d.Description, '')) as [Description]," +
                "a.[IdentificationNumber],a.[Name],a.[Gender],a.[Title],[Birthday],[PhoneNumber],[MobileNumber],[Address]," +
                "[ExternalReference],a.[IdDepartment],d.Description as Departamento,[Position],[Active],[Picture],[PictureOrientation]" +
                ",[Privilege],[HourSalary],[Password],[PreferredIdLanguage],[Email],a.[Comment],[ProximityCard]" +
                ",[LastRecord],[LastLogin],[CreatedBy],[CreatedDatetime],[ModifiedBy],[ModifiedDatetime]" +
                ",[AdministratorType],[IdProfile],[DevPassword],[UseShift],[SendSMS],[SMSPhone],[TemplateCode]" +
                ",[ApplyExceptionPermition],[ExceptionPermitionBegin],[ExceptionPermitionEnd] FROM[BDBioAdminSQL].[dbo].[User] a " +
                "LEFT JOIN[BDBioAdminSQL].[dbo].[UserShift] b ON a.IdUser = b.IdUser " +
                "LEFT JOIN[BDBioAdminSQL].[dbo].[Shift] c ON b.ShiftId = c.ShiftId " +
                "LEFT JOIN[BDBioAdminSQL].[dbo].[Department] d ON a.[IdDepartment] = d.[IdDepartment] ";

            micomm.Open();
            comando.ExecuteNonQuery();
            SqlDataAdapter da = new()
            {
                SelectCommand = comando
            };
            da.Fill(dt);

            List<ShifthAssingEmployeeDetails> horarios;

            horarios = dt.AsEnumerable().Select(x => new ShifthAssingEmployeeDetails
            {
                IdUser = x.Field<int>("IdUser"),
                Description = x.Field<string>("Description")!,
                Name = x.Field<string>("Name")!,
                UserShiftId = x.Field<int>("UserShiftId"),
                ShiftId = x.Field<int>("ShiftId"),
                HourSalary = x.Field<decimal>("HourSalary")
            }).ToList();

            return horarios;





        }
        public string GetDepartamento(int userid)
        {
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,

                CommandText =
            "SELECT b.description FROM [BDBioAdminSQL].[dbo].[User] a " +
            "LEFT JOIN [BDBioAdminSQL].[dbo].[Department] b ON a.IdDepartment = b.IdDepartment " +
            "WHERE a.IdUser=@p1"
            };
            SqlParameter p1 = new("@p1", userid);
            comando.Parameters.Add(p1);
            micomm.Open();
            string result = Convert.ToString(comando.ExecuteScalar())!;
            micomm.Close();
            return result;
        }
    }
}
