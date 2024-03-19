using Microsoft.EntityFrameworkCore.Metadata;

namespace DataFlowRRHH.Models
{
    public class Jornada
    {
        public int IdUser { get; set; }
        public string Empleado { get; set; } = null!;
        public string Dpto { get; set; } = "";
        public DateTime Fecha { get; set; }
        public DateTime Horario_salida { get; set; }
        public string Mark1 { get; set; } = null!;
        public string Mark2 { get; set; } = null!;
        public string Mark3 { get; set; } = null!;
        public string Mark4 { get; set; } = null!;
        public int Tardanza_Entrada { get; set; } = 0;
        public DateTime? Mark1_Dt { get; set; }
        public DateTime? Mark2_Dt { get; set; }
        public DateTime? Mark3_Dt { get; set; }
        public DateTime? Mark4_Dt { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public int Ponches { get; set; }
        public double Horas_Jornada { get; set; }
        public string Type_shift { get; set; } = "";
        public string Tipo_Descanso { get; set; } = "";
        public int IdShift { get; set; }
        public string ShiftName { get; set; } = "";
        public int Start_journal_hour { get; set; } = 0;
        public int End_journal_hour { get; set; } = 0;
        public int Start_journal_minutes { get; set; } = 0;
        public int End_journal_minutes { get; set; } = 0;
        public int End_journal { get; set; } = 0;
        public double Lapso_tardanza_entrada { get; set; } = 0;
        public double Lapso_tardanza_almuerzo { get; set; } = 0;
        public Boolean Feriado { get; set; }
        public Boolean DayFree { get; set; }
        public int IndexDay { get; set; } = 0;
        public double Horas_Extras { get; set; } = 0;
        public string he_string { get; set; } = ""; 
        public decimal sueldo_hora  { get; set; }
        public double factor { get; set; }
        public double fr_sueldo { get; set; }
        public double MontoHeDiario { get; set; }
        public double factor100 { get; set; }
        public double Montoal100 { get; set; }
        public string DiaSemana { get; set; } = "";
        public double horas100 { get; set; } = 0;
        public Boolean diaAusencia { get; set; }
    }
}
