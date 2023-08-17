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
        public DateTime? Mark1_Dt { get; set; }
        public DateTime? Mark2_Dt { get; set; }
        public DateTime? Mark3_Dt { get; set; }
        public DateTime? Mark4_Dt { get; set; }
        public int Ponches { get; set; }
        public double Horas_Jornada { get; set; }
        public string Type_shift { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public int Start_journal { get; set; } = 0;
        public int End_journal { get; set; } = 0;
        public double Horas_Extras { get; set; }
        public double Salario { get; set; }
    }
}
