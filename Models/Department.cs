namespace DataFlowRRHH.Models;

public partial class Department
{
    public int IdDepartment { get; set; }

    public int? IdParent { get; set; }

    public string Description { get; set; } = null!;

    public string? SupervisorName { get; set; }

    public string? SupervisorEmail { get; set; }

    public string? Comment { get; set; }

    public string? DepartamentosSuperiores { get; set; }

    public string? DepartamentosInferiores { get; set; }
}
