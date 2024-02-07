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
        public DateTime? Mark1_Dt { get; set; }
        public DateTime? Mark2_Dt { get; set; }
        public DateTime? Mark3_Dt { get; set; }
        public DateTime? Mark4_Dt { get; set; }
        public double Horas_trabajadas { get; set; }
        public double Horas_Extras { get; set; }
        public double Salario { get; set; }
        public int Marcas { get; set; }
        public string Escala1_titulo { get; set; } = "";
        public string Escala2_titulo { get; set; } = "";
        public string Escala3_titulo { get; set; } = "";
        public double horas_escala1 { get; set; }
        public double pesos_escala1 { get; set; }
        public double horas_escala2 { get; set; }
        public double pesos_escala2 { get; set; }
        public double horas_escala3 { get; set; }
        public double pesos_escala3 { get; set; }
        public Boolean tardanza_entrada_jornada { get; set; }
        public Boolean tardanza_almuerzo_jornada { get; set; }
        public double lapso_tardanza_entrada { get; set; } = 0;
        public double lapso_tardanza_almuerzo { get; set; } = 0;

    }
}
