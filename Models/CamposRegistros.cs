using System.Globalization;

namespace DataFlowRRHH.Models
{
    public class CamposRegistros
    {
        public int IdUser { get; set; } = 0;
        public string NameUser { get; set; } = "";
        public DateTime RecordTime { get; set; }
        public int IdDepartment { get; set; }
        public string DepartmentName { get; set; } = "";
        public int IdDevice { get; set; } = 0;
        public string DeviceName { get; set; } = "";
        public string Type_Shift { get; set; } = "";

    }
}
