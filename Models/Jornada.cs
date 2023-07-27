namespace DataFlowRRHH.Models
{
    public class Jornada
    {
        public int IdUser { get; set; }
        public string Empleado { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string Mark1 { get; set; } = null!;
        public string Mark2 { get; set; } = null!;
        public string Mark3 { get; set; } = null!;
        public string Mark4 { get; set; } = null!;
        public int Ponches { get; set; }
        public TimeSpan Horas_Jornada { get; set; } 


    }
}
