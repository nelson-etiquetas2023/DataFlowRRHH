using DataFlowRRHH.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.Xml;

namespace DataFlowRRHH.Service
{
    public interface IServiceGestion
    {
        Task<List<CamposRegistros>> LoadHuellasEmpleados(DateTime ToDate, DateTime FromDate);
        List<Jornada> CalcularHorasExtras(List<CamposRegistros> listaponches, List<Feriado> feriados);
        double ObtenerSalarioxHora(int userid);
        int ObtenerHorarioEmpleado(int userid);
        ShiftAssingEmployeeRow ObtenerParametrosHorarios(int shiftid, int dayid);
        List<CampoHorasExtras> CalculoEscalasDeHorarios(List<CampoHorasExtras> data, List<Feriado> feriados);
        List<Feriado> GetDataFeriados();
    }
    public class ServiceGestion : IServiceGestion
    {
        //conexion con entityframwork
        public BdbioAdminSqlContext db;
        public List<CamposRegistros> ListaPonches { get; set; } = new List<CamposRegistros>();

        //Conexion con ado.net
        //public DataTable DtHorasExtras = new();
        public SqlConnection micomm;

        //Obtener el connectionString del appsetting.json
        readonly IConfiguration configuration;
        public DateTime _todateQuery { get; set; }
        public DateTime _fromdateQuery { get; set; }
        public List<Jornada> jornadas { get; set; } = new List<Jornada>();

        public ServiceGestion(BdbioAdminSqlContext _db,IConfiguration _configuration)
        {
            configuration = _configuration;
            db = _db;
            micomm = new SqlConnection
            {
                ConnectionString = configuration.GetSection("ConnectionStrings").GetSection("SettingEtiquetas").Value  
            };
        }
        //Etapa 1 de la Consulta.
        public async Task<List<CamposRegistros>> LoadHuellasEmpleados(DateTime ToDate, DateTime FromDate)
        {

            _todateQuery = ToDate;
            _fromdateQuery = FromDate;

            // esta funcion trae todos los registros de las marcas de los empleados por dia.
            // es la primera parte necesaria para calculas las horas extras.

            var ponches = db.Records
            .Select(p => new
            {
                p.IdUser,
                NameUser = p.IdUserNavigation.Name,
                salario = p.IdUserNavigation.HourSalary,
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
                   p.RecordTime,
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
                   start_journal_hour = db.ShiftDetails.Where(x => x.ShiftId == w.id ).Select(x=> x.T2inHour).FirstOrDefault(),
                   end_journal_hour = db.ShiftDetails.Where(x => x.ShiftId == w.id).Select(x => x.T2outHour).FirstOrDefault(),
                   start_journal_minutes = db.ShiftDetails.Where(x => x.ShiftId == w.id).Select(x => x.T2inMinute).FirstOrDefault(),
                   end_journal_minutes = db.ShiftDetails.Where(x => x.ShiftId == w.id).Select(x => x.T2outMinute).FirstOrDefault(),
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
                salario_hora = x.salario,
                DeviceName = x.DeviceName,
                Type_Shift = x.Horario.Select(g => g.type_Shift).FirstOrDefault()!,
                ShiftName = x.Horario.Select(g => g.shiftname).FirstOrDefault()!,
                Indexday = x.indexday,
                Start_journal_hour = x.Horario.Select(g => g.start_journal_hour).FirstOrDefault(),
                Start_journal_minutes = x.Horario.Select(g => g.start_journal_minutes).FirstOrDefault(),
                End_journal_hour = x.Horario.Select(g => g.end_journal_hour).FirstOrDefault(),
                End_journal_minutes = x.Horario.Select(g => g.end_journal_minutes).FirstOrDefault(),

            }).Where(f => f.RecordTime >= ToDate && f.RecordTime <= FromDate).OrderBy(f => f.RecordTime);

            return ListaPonches = await ponches.ToListAsync();
        }

        public int IndexDay(DateTime fecha)
        {
            int idxday = 0;
            if (fecha.DayOfWeek == DayOfWeek.Saturday) idxday = 5;
            return idxday;
        }

        //Etapa 2 de la Consulta.
        //----------------------//
        public List<Jornada> CalcularHorasExtras(List<CamposRegistros> listaponches, List<Feriado> feriados)
        {
            
            jornadas = (from q in listaponches
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
                                IdShift = grp.FirstOrDefault()!.IdShift,
                                ShiftName = grp.FirstOrDefault()!.ShiftName,
                                ShiftStart = new TimeSpan(grp.FirstOrDefault()!.Start_journal_hour, grp.FirstOrDefault()!.Start_journal_minutes, 0),
                                ShiftEnd = new TimeSpan(grp.FirstOrDefault()!.End_journal_hour, grp.FirstOrDefault()!.End_journal_minutes, 0),
                                End_journal = grp.FirstOrDefault()!.End_journal,
                                IndexDay = grp.FirstOrDefault()!.Indexday,
                                End_journal_hour = grp.FirstOrDefault()!.End_journal_hour,
                                End_journal_minutes = grp.FirstOrDefault()!.End_journal_minutes,
                                Horas_Extras = Convert.ToDouble((grp.LastOrDefault()!.RecordTime.AddMinutes(-15) - new DateTime(grp.FirstOrDefault()!.RecordTime.Year,
                                grp.FirstOrDefault()!.RecordTime.Month,
                                grp.FirstOrDefault()!.RecordTime.Day, grp.FirstOrDefault()!.End_journal_hour, grp.FirstOrDefault()!.End_journal_minutes, 0)).TotalHours),
                                sueldo_hora = grp.FirstOrDefault()!.salario_hora,
                                DiaSemana = (grp.FirstOrDefault()!.RecordTime).DayOfWeek.ToString()
        }).OrderBy(o => o.IdUser).ToList();

            //iteracion sobre las filas de la data
            //-----------------------------------//
            foreach (var item in jornadas) 
            {
                //asignar la entrada y salida del horario asignado hour_in // hour_out x dia en horario
                int[] values_shifth_start_end_day = StartEndDayShift(item.IdShift, item.IndexDay, item.Fecha);

                item.ShiftStart = new TimeSpan(values_shifth_start_end_day[0], values_shifth_start_end_day[1], 0);
                item.Start_journal_minutes = values_shifth_start_end_day[1];
                item.ShiftEnd = new TimeSpan(values_shifth_start_end_day[2], values_shifth_start_end_day[3], 0);
                item.End_journal_minutes = values_shifth_start_end_day[3];


                //checkear los dias no marcado por empleado en todo el mes.
                CheckAusenciasMonthEmployee();

                //dia de la semana español
                item.DiaSemana = ConverirDiaSemanaEspañol(item.DiaSemana);

                //Redondear horas extras.
                item.Horas_Extras = Math.Round(item.Horas_Extras, 2, MidpointRounding.AwayFromZero);

                //Validar horas extras.
                if (item.Horas_Jornada == 0 || item.Horas_Extras < 0 || item.ShiftName==null || item.Ponches > 5) 
                {
                    item.Horas_Extras = 0;
                }
                //Calculos de horas al 35.
                if (item.Horas_Extras > 0) 
                {
                    item.factor = 35;
                    item.fr_sueldo = (item.factor * Convert.ToDouble(item.sueldo_hora))/100;
                    item.MontoHeDiario = Math.Round((item.Horas_Extras * item.fr_sueldo),2,MidpointRounding.AwayFromZero);
                }
                //Calculo de la salida de horario

                TimeSpan ShifthEnd = new(item.End_journal_hour, item.End_journal_minutes, 0);

                //calculo de los feriados laborados.
                //-----------------------------------//
                item.Feriado = VerificarDiaFeriado(item.Fecha,feriados);
                if (item.Feriado) {
                    item.factor100 = 100;
                    //calculo al 35% cambia al 100%
                    item.factor = 100;
                    item.fr_sueldo = Convert.ToDouble(item.sueldo_hora);
                    item.MontoHeDiario = Math.Round((item.fr_sueldo * item.Horas_Extras),2,MidpointRounding.AwayFromZero);
                    item.horas100 = Math.Round(item.Horas_Jornada, 2, MidpointRounding.AwayFromZero);
                    item.Montoal100 = Math.Round((Convert.ToDouble(item.sueldo_hora) * item.Horas_Jornada), 2, MidpointRounding.AwayFromZero);
                }

                //calculo de los dias libres laborados.
                //-----------------------------------//
                item.DayFree = DayFreeWeek(item.IdShift,item.IndexDay);

                if (item.DayFree) {
                   item.factor100 = 100;
                   //calculo al 35% cambia al 100%
                   item.factor = 100;
                   item.fr_sueldo = Convert.ToDouble(item.sueldo_hora);
                   item.MontoHeDiario = Math.Round((item.fr_sueldo * item.Horas_Extras),2,MidpointRounding.AwayFromZero);
                    item.horas100 = Math.Round(item.Horas_Jornada,2,MidpointRounding.AwayFromZero);
                    item.Montoal100 = Math.Round((Convert.ToDouble(item.sueldo_hora) * item.Horas_Jornada),2,MidpointRounding.AwayFromZero);
                } 
            }

            //calculo domingos
            CalculoDomingos(jornadas);

            return jornadas;
        }

        public DateTime[] CheckAusenciasMonthEmployee() 
        {

            //parametros primero y ultimo de mes 
            int DayStartMonth = (int) _todateQuery.Day;
            int DayEndMonth = (int) _fromdateQuery.Day;
            int monthQuery = (int)_todateQuery.Month;
            int yearQuery = (int)_todateQuery.Year;
            int idEployee = 204;
           
            //matriz para almacenar los dias de ausencia.
            DateTime[] DaysAusencias = new DateTime[DayEndMonth];

            //recorre todo el mes de la consulta.
            int dayCounter = 0;
            //filtrado de ponches por empleados
            var ponches_empleado = jornadas.Where(x => x.IdUser == idEployee);

            for (int i = DayStartMonth; i <= DayEndMonth; i++) 
            {
                
                dayCounter += 1;
                DateTime DayAusence = new DateTime(yearQuery, monthQuery, dayCounter);

                var check = ponches_empleado.Where(x => x.Fecha.Day == DayAusence.Day);

                if (!check.Any()) 
                {
                    DaysAusencias[dayCounter - 1] = DayAusence;
                }        
            }

            //Limpiar los demas dias con valores por defecto por fecha.
            int yearDefault = 0001;
            DaysAusencias = DaysAusencias.Where(x => x.Year != yearDefault).ToArray();

            return DaysAusencias;
        }

        public Boolean CalculoDomingos(List<Jornada> jornadas) {

            //Calculo de los domingos

            //agrupar la data por empleado.
            var grupos = jornadas.GroupBy(x => x.IdUser);
            string tipo_descanso = "";

            //encabezado
            foreach (var grupo in grupos)
            {
                tipo_descanso = CalculateDayFree(grupo.FirstOrDefault()!.ShiftName);
                int numDomingos = 0;
                //detalle
                foreach (var journal in grupo)
                {
                    journal.Tipo_Descanso = tipo_descanso;
                    if (journal.DiaSemana == "Domingo")
                    {
                        numDomingos++;
                        //los que descansan 1 dia comnpleto.
                        if (journal.Tipo_Descanso == "full day.")
                        {
                            if (numDomingos > 2)
                            {
                                UpdateCalculateListDomingos(journal, jornadas);
                            }
                            else {
                                // no se calcula
                                journal.factor100 = 0;
                                //calculo al 35% cambia al 100%
                                journal.factor = 0;
                                journal.fr_sueldo = 0;
                                journal.MontoHeDiario = 0;
                                journal.horas100 = 0;
                                journal.Montoal100 = 0;
                            }
                        }

                        //los que descansas medio dia.
                        if (journal.Tipo_Descanso == "medio dia.")
                        {
                            UpdateCalculateListDomingos(journal, jornadas);
                        }
                    }
                    

                }
            }

            return true;
        }

        public void UpdateCalculateListDomingos(Jornada journal, List<Jornada> jornadas) {
            //se calcula el domingo al 100%
            // los primeros dos domingos del mes no se le paga.
            // a los horarios que descansan 1 dia completo
            journal.factor100 = 0;
            //calculo al 35% cambia al 100%
            journal.factor = 100;
            journal.fr_sueldo = Convert.ToDouble(journal.sueldo_hora);
            journal.MontoHeDiario = Math.Round((journal.fr_sueldo * journal.Horas_Extras), 2, MidpointRounding.AwayFromZero);
            journal.horas100 = Math.Round(journal.Horas_Jornada, 2, MidpointRounding.AwayFromZero);
            journal.Montoal100 = Math.Round((Convert.ToDouble(journal.sueldo_hora) * journal.Horas_Jornada), 2, MidpointRounding.AwayFromZero);
            // actualizar los datos la lista original.
            jornadas = jornadas.Where(x => x.IdUser.Equals(journal.IdUser) && x.Fecha.Equals(journal.Fecha))
                .Select(x => {
                    x.Montoal100 = journal.Montoal100;
                    x.factor = journal.factor;
                    return x;
                })
                .ToList();
        }

        public string CalculateDayFree(string horario) {

            //Clasificar los horarios por descanso full / descanso medio dia.
            string cadenaFull = "";
            string cadenaMid = "1/2";
            Boolean dayfree = false;
            string tipo = "";

            //buscar los que descansan full-day.
            dayfree = horario == null ? false : horario.ToUpper().Contains(cadenaFull);
            if (dayfree)
            {
                tipo = "full day.";
            }

            //buscar los que descansan medio-dia.
            dayfree = horario == null ? false : horario.ToUpper().Contains(cadenaMid);
            if (dayfree)
            {
                tipo = "medio dia.";
            }

            // otros horarios.
            if (horario == "")
            {
                horario = "sin asignar";
            }

            return tipo;
        }

        public string ConverirDiaSemanaEspañol(string param1) 
        {
            if (param1 == "Monday") param1 = "Lunes";
            if (param1 == "Tuesday") param1 = "Martes";
            if (param1 == "Wednesday") param1 = "Miercoles";
            if (param1 == "Thursday") param1 = "Jueves";
            if (param1 == "Friday") param1 = "Viernes";
            if (param1 == "Saturday") param1 = "Sabado";
            if (param1 == "Sunday") param1 = "Domingo";
            return param1;
        }

        public int[] StartEndDayShift(int idShift, int indexday, DateTime fechaPonche) 
        {
            DataTable dt = new DataTable();
            int[] StartEndConfigHourDay = new int[4];

            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,
                CommandText = "SELECT t2inhour,t2inminute,t2outhour,t2outminute FROM ShiftDetail WHERE ShiftId=@p1 and DayId=@p2"
            };
            SqlParameter p1 = new("@p1", idShift);
            SqlParameter p2 = new("@p2", indexday);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            comando.ExecuteNonQuery();

            SqlDataAdapter da = new SqlDataAdapter(comando);
            da.Fill(dt);

            //position 0 = in hour.
            //position 1 = in minute.
            //position 2 = out hour.
            //position 3 = out minute.

            if (dt.Rows.Count > 0) 
            {
                var hour_in = Convert.ToInt32(dt.Rows[0]["t2inhour"]);
                var minute_in = Convert.ToInt32(dt.Rows[0]["t2inminute"]);
                var hour_out = Convert.ToInt32(dt.Rows[0]["t2outhour"]);
                var minute_out = Convert.ToInt32(dt.Rows[0]["t2outminute"]);

                StartEndConfigHourDay[0] = hour_in;
                StartEndConfigHourDay[1] = minute_in;
                StartEndConfigHourDay[2] = hour_out;
                StartEndConfigHourDay[3] = minute_out;
            }

            
            return StartEndConfigHourDay;
           
        }
      
        public Boolean DayFreeWeek(int Idshift, int Indexday) 
        {
           
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text,
                CommandText = "SELECT type FROM ShiftDetail WHERE ShiftId=@p1 and DayId=@p2"
            };
            SqlParameter p1 = new("@p1", Idshift);
            SqlParameter p2 = new("@p2", Indexday);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2); 
            int result_sql = Convert.ToInt16(comando.ExecuteScalar());
            //resultado 1 es dia libre - 2 dia de trabajo.
            if (result_sql == 1)
            {
                //dia libre trabajado
                return true;
            }
            else 
            {
                return false;
            }
        }
        public Boolean VerificarDiaFeriado(DateTime fecha, List<Feriado> feriados) 
        {
            Boolean result = false;

            foreach (var item in feriados) 
            {
                if (fecha >= item.DateStart && fecha <= item.DateEnd)
                {
                    result = true;
                }
            }
            return result;
        }
        public List<CampoHorasExtras> CalculoEscalasDeHorarios(List<CampoHorasExtras> data, List<Feriado> feriados)
        {

            var ferdos = feriados;
            // Horas Extras por Escalas
            foreach (var item in data)
            {       
                int userid = Convert.ToInt16(item.UserId);
                int shiftf = ObtenerHorarioEmpleado(userid);
                int indexday = Convert.ToInt16(item.Fecha_Marcaje.DayOfWeek - 1);
                if (indexday == -1) indexday = 6;
                string dia = item.Fecha_Marcaje.ToString("dddd");
                //Parametros de los horarios
                ShiftAssingEmployeeRow hfp = ObtenerParametrosHorarios(shiftf, indexday);
                //todos los calculos se hacen si el empleado tiene asignado un horario
                if (Convert.ToInt16(item.Hora_Entrada) != 0 && Convert.ToInt16(item.Hora_Salida) != 0) 
                {
                    //Escala1
                    DateTime escala1_date1 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal1_Start), 0, 0);
                    DateTime escala1_date2 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal1_End), 0, 0);
                    int escala1_factor = Convert.ToInt16(hfp.Cal1_Factor);
                    TimeSpan escala1_horas = escala1_date2 - escala1_date1;
                    item.Escala1_titulo = hfp.Cal1_Start + "-" + hfp.Cal1_End + "-" + hfp.Cal1_Factor + "%";
                    //Escala2
                    DateTime escala2_date1 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal2_Start), 0, 0);
                    DateTime escala2_date2 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal2_End), 0, 0);
                    int escala2_factor = Convert.ToInt16(hfp.Cal2_Factor);
                    TimeSpan escala2_horas = escala2_date2 - escala2_date1;
                    item.Escala2_titulo = hfp.Cal2_Start + "-" + hfp.Cal2_End + "-" + hfp.Cal2_Factor + "%";
                    //Escala3
                    DateTime escala3_date1 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal3_Start), 0, 0);
                    DateTime escala3_date2 = new(item.Fecha_Marcaje.Year,
                    item.Fecha_Marcaje.Month, item.Fecha_Marcaje.Day, Convert.ToInt16(hfp.Cal3_End), 0, 0);
                    int escala3_factor = Convert.ToInt16(hfp.Cal3_Factor);
                    TimeSpan escala3_horas = escala3_date2 - escala3_date1;
                    item.Escala3_titulo = hfp.Cal3_Start + "-" + hfp.Cal3_End + "-" + hfp.Cal3_Factor + "%";
                    //verificar si hay horas extras.
                    if (item.Mark4_Dt > escala1_date1) 
                    {
                        TimeSpan het = (DateTime)item.Mark4_Dt - escala1_date1;
                        Boolean run;
                        // Condiciones del primer nivel.
                        if (het >= escala1_horas)
                        {
                            item.horas_escala1 =  escala1_horas.TotalHours;
                            item.pesos_escala1 = Math.Round((((item.Salario * escala1_factor) / 100) * item.horas_escala1), 2, MidpointRounding.AwayFromZero);
                            run = true;
                        }
                        else 
                        {
                            item.horas_escala1 =  Math.Round((het.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                            item.pesos_escala1 = Math.Round((((item.Salario * escala1_factor)/100) * item.horas_escala1), 2, MidpointRounding.AwayFromZero);
                            run = false;
                        }
                        // Condiciones de Segundo Nivel.
                        if (run)
                        {
                            TimeSpan het1 = (DateTime)item.Mark4_Dt - escala1_date2;
                            if (het1 >= escala2_horas)
                            {
                                item.horas_escala2 = escala2_horas.TotalHours;
                                item.pesos_escala2 = Math.Round((((item.Salario * escala2_factor) / 100) * item.horas_escala2), 2, MidpointRounding.AwayFromZero);
                                run = true;
                            }
                            else
                            {
                                item.horas_escala2 = Math.Round((het1.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                                item.pesos_escala2 = Math.Round((((item.Salario * escala2_factor) / 100) * item.horas_escala2), 2, MidpointRounding.AwayFromZero);
                                run = false;
                            }
                        }
                        // Condiciones de Tercer Nivel.
                        if (run)
                        {
                            TimeSpan het2 = (DateTime)item.Mark4_Dt - escala3_date1;
                            if (het2 >= escala3_horas)
                            {
                                item.horas_escala3 = escala3_horas.TotalHours;
                                item.pesos_escala3 = Math.Round((((item.Salario * escala3_factor) / 100) * item.horas_escala3), 2, MidpointRounding.AwayFromZero);
                                
                            }
                            else
                            {
                                item.horas_escala3 = Math.Round((het2.TotalMinutes / 60), 2, MidpointRounding.AwayFromZero);
                                item.pesos_escala3 = Math.Round((((item.Salario * escala3_factor) / 100) * item.horas_escala3), 2, MidpointRounding.AwayFromZero);
                                run = false;
                            }
                        }
                    }
                } 
            }
            //devuelvo la misma lista pero actualizada.
            return data;
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
        public List<Feriado> GetDataFeriados() 
        {
            
            DataTable dt1 = new();
            DataTable dt2 = new();



            //comando sql para traer los empleados del bioadmin.
            dt1.Clear();
            dt2.Clear();

            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text
            };

            //query1
            comando.CommandText = comando.CommandText = "select IdException,BeginingDate,EndingDate,Description,Comment,IdDepartment,IdUser,PaymentType,PaymentFactor,Recurring from Exception";
            micomm.Open();
            comando.ExecuteNonQuery();
            SqlDataAdapter da1 = new()
            {
                SelectCommand = comando
            };
            da1.Fill(dt1);
            //query2
            comando.CommandText = comando.CommandText = "select IdException,BeginingDate,EndingDate,Description,Comment,IdDepartment,CAST(IdUser AS char) as iduser,PaymentType,PaymentFactor,Recurring from Exception";
            comando.ExecuteNonQuery();
            SqlDataAdapter da2 = new()
            {
                SelectCommand = comando
            };
            da2.Fill(dt2);
            List<Feriado> lista1 = new();
            List<Feriado> lista2 = new();

            lista1 = dt1.AsEnumerable().Select(x => new Feriado
            {
                Id = x.Field<int>("IdException"),
                Description = x.Field<string>("Description")!,
                DateStart = x.Field<DateTime>("BeginingDate"),
                DateEnd = x.Field<DateTime>("EndingDate"),
                Type = x.Field<int>("PaymentType"),
                Factor = x.Field<int>("PaymentFactor"),
                Depart = x.Field<int>("IdDepartment"),
                Employee =  x.Field<string>("IdUser"),
                comment = x.Field<string>("Comment"),
                Recurrente = x.Field<Boolean>("Recurring")
            }).ToList();

            lista2 = dt2.AsEnumerable().Select(x => new Feriado
            {
                Id = x.Field<int>("IdException"),
                Description = x.Field<string>("Description")!,
                DateStart = x.Field<DateTime>("BeginingDate"),
                DateEnd = x.Field<DateTime>("EndingDate"),
                Type = x.Field<int>("PaymentType"),
                Factor = x.Field<int>("PaymentFactor"),
                Depart = x.Field<int>("IdDepartment"),
                Employee = x.Field<string>("IdUser"),
                comment = x.Field<string>("Comment"),
                Recurrente = x.Field<Boolean>("Recurring")
            }).ToList();

            return lista1.Concat(lista2).ToList();



        }
    }
}
