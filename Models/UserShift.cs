using System.ComponentModel.DataAnnotations.Schema;

namespace DataFlowRRHH.Models;

public partial class UserShift
{
    public int UserShiftId { get; set; }

    public int? IdUser { get; set; }

    public int? ShiftId { get; set; }

    public DateTime? BeginDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey("ShiftId")]
    public virtual Shift ShiftNavigation { get; set; } = null!;
}
