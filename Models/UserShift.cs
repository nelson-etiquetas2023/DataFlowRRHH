using System;
using System.Collections.Generic;

namespace DataFlowRRHH.Models;

public partial class UserShift
{
    public int UserShiftId { get; set; }

    public int? IdUser { get; set; }

    public int? ShiftId { get; set; }

    public DateTime? BeginDate { get; set; }

    public DateTime? EndDate { get; set; }
}
