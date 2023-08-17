namespace DataFlowRRHH.Models
{
    public class CampoHorasExtras
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Departamento { get; set; } = "";
        public DateTime Fecha_Marcaje { get; set; }
        public string Horario_Asignado { get; set; } = "";
        public string Hora_Entrada { get; set; } = "";
        public string Hora_Salida { get; set; } = "";
        public string M1 { get; set; } = "";
        public string M2 { get; set; } = "";
        public string M3 { get; set; } = "";
        public string M4 { get; set; } = "";
        public double Horas_trabajadas { get; set; }
        public double Horas_Extras { get; set; }
        public double Salario { get; set; }
        public int Marcas { get; set; }
    }
}
