using System;
using System.Collections.Generic;

namespace DataFlowRRHH.Models;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string? Description { get; set; }

    public string? Comment { get; set; }

    public int? CuttingHour { get; set; }

    public int? CuttingMinute { get; set; }

    public int? Cycle { get; set; }
}
