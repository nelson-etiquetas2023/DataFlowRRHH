namespace DataFlowRRHH.Models
{
    public class FieldsReportsHorasExtras
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string ShiftId { get; set; } = null!;
        public string ShiftName { get; set; } = null!;
        public int ShiftType { get; set; }
        public int JournalStart { get; set; }
        public int JournalFinish { get; set; }
        public int StartHourN1 { get; set; }
        public int FinishHourN1 { get; set; }
        public int FactorN1 { get; set; }
        public int StartHourN2 { get; set; }
        public int FinishHourN2 { get; set; }
        public int FactorN2 { get; set; }
        public int StartHourN3 { get; set; }
        public int FinishHourN3 { get; set; }
        public int FactorN3 { get; set; }
        public int StartHourN4 { get; set; }
        public int FinishHourN4 { get; set; }
        public int FactorN4 { get; set; }
        public int StartHourN5 { get; set; }
        public int FinishHourN5 { get; set; }
        public int FactorN5 { get; set; }
    }
}
