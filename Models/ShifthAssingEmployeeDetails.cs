using Microsoft.AspNetCore.Identity;

namespace DataFlowRRHH.Models
{
    public class ShifthAssingEmployeeDetails
    {
        public string UserId { get; set; } = "";
        public string UserShifthId { get; set; } = "";
        public string ShiftId { get; set; } = "";
        public string Description { get; set; } = "";
        public double SalaryHour { get; set; } = 0;
    }
}
