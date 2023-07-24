using DataFlowRRHH.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;


namespace DataFlowRRHH.Service
{
    public class ServiceHorasExtras
    {
        public SqlConnection micomm;
        public ServiceHorasExtras()
        {
            micomm = new SqlConnection
            {
                ConnectionString = @"Data Source=SERVER-ETIQUETA;Initial Catalog=BDBioAdminSQL;User Id=Npino;Password=Jossycar5%;TrustServerCertificate=True;"
            };
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
        public DataTable HorariosAsignadosxEmpleados()
        {
            DataTable dt = new();
            //comando sql para traer los empleados del bioadmin.
            dt.Clear();
            SqlCommand comando = new()
            {
                Connection = micomm,
                CommandType = CommandType.Text
            };
            comando.CommandText = comando.CommandText = "SELECT a.[IdUser],b.[UserShiftId],b.[ShiftId],c.[Description]," +
            "a.[IdentificationNumber]," +
            "a.[Name],a.[Gender],a.[Title],[Birthday],[PhoneNumber],[MobileNumber],[Address]" +
            ",[ExternalReference],a.[IdDepartment],d.Description as Departamento,[Position],[Active],[Picture],[PictureOrientation]" +
            ",[Privilege],[HourSalary],[Password],[PreferredIdLanguage],[Email],a.[Comment],[ProximityCard]" +
            ",[LastRecord],[LastLogin],[CreatedBy],[CreatedDatetime],[ModifiedBy],[ModifiedDatetime]" +
            ",[AdministratorType],[IdProfile],[DevPassword],[UseShift],[SendSMS],[SMSPhone],[TemplateCode]" +
            ",[ApplyExceptionPermition],[ExceptionPermitionBegin],[ExceptionPermitionEnd] FROM [BDBioAdminSQL].[dbo].[User] a " +
            "LEFT JOIN [BDBioAdminSQL].[dbo].[UserShift] b ON a.IdUser = b.IdUser " +
            "LEFT JOIN [BDBioAdminSQL].[dbo].[Shift] c ON b.ShiftId = c.ShiftId " +
            "LEFT JOIN [BDBioAdminSQL].[dbo].[Department] d ON a.[IdDepartment] = d.[IdDepartment]";
            micomm.Open();
            comando.ExecuteNonQuery();
            SqlDataAdapter da = new()
            {
                SelectCommand = comando
            };
            da.Fill(dt);
            return dt;
        }
    }
}
