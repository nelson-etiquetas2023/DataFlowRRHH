namespace DataFlowRRHH.Models
{
    public class Feriado
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? Description { get; set; }
        public Boolean Recurrente { get; set; }
        public int Type { get; set; }
        public int Factor { get; set; }
        public int Depart { get; set; }
        public string? Employee { get; set; } 
        public string? comment { get; set; }
    }
}
