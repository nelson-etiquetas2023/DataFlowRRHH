namespace DataFlowRRHH.Models
{
    public class Feriado
    {
        public int IdException { get; set; }
        public DateTime BeginingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public string? Description { get; set; }
        public Boolean Recurring { get; set; }
        public int PaymentFactor { get; set; }
        public string? Comment { get; set; }
    }
}
