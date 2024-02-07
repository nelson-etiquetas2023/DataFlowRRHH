namespace DataFlowRRHH.Models
{
    public class CamposRegistros
    {
        public int IdUser { get; set; } = 0;
        public string NameUser { get; set; } = "";
        public DateTime RecordTime { get; set; }
        public int IdDepartment { get; set; }
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public int IdDevice { get; set; } = 0;
        public int IdShift { get; set; } = 0;
        public int Start_journal_hour { get; set; } = 0;
        public int End_journal_hour { get; set; } = 0;
        public int Start_journal_minutes { get; set; } = 0;
        public int End_journal_minutes { get; set; } = 0;
        public int End_journal { get; set; } = 0;
        public string DeviceName { get; set; } = "";
        public string Type_Shift { get; set; } = "";
        public int Indexday { get; set; } = 0;
        public decimal salario_hora { get; set; }
    }
}
