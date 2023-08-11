namespace DataFlowRRHH.Models
{
    public class ShifthAssingEmployeeDetails
    {
        public int IdUser { get; set; } = 0;
        public int UserShiftId { get; set; } = 0;
        public int ShiftId { get; set; } = 0;
        public string Description { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal HourSalary { get; set; } = 0;
    }
}
