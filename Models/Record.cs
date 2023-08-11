using System.ComponentModel.DataAnnotations.Schema;

namespace DataFlowRRHH.Models;

public partial class Record
{
    public int IdUser { get; set; }

    public DateTime RecordTime { get; set; }

    public int MachineNumber { get; set; }

    public int RecordType { get; set; }

    public int? VerifyMode { get; set; }

    public int Workcode { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int ModifiedBy { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    [ForeignKey("MachineNumber")]
    public virtual Device IdDeviceNavigation { get; set; } = null!;

}
