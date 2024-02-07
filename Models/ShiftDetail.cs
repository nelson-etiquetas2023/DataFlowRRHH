namespace DataFlowRRHH.Models;

public partial class ShiftDetail
{
    public int ShiftId { get; set; }

    public int DayId { get; set; }

    public string? Description { get; set; }

    public int Type { get; set; }

    public int T1attTime { get; set; }

    public bool T1overTime1 { get; set; }

    public int T1overTime1Minutes { get; set; }

    public int T1overTime1Factor { get; set; }

    public bool T1overTime2 { get; set; }

    public int T1overTime2Minutes { get; set; }

    public int T1overTime2Factor { get; set; }

    public bool T1overTime3 { get; set; }

    public int T1overTime3Minutes { get; set; }

    public int T1overTime3Factor { get; set; }

    public bool T1overTime4 { get; set; }

    public int T1overTime4Minutes { get; set; }

    public int T1overTime4Factor { get; set; }

    public bool T1overTime5 { get; set; }

    public int T1overTime5Minutes { get; set; }

    public int T1overTime5Factor { get; set; }

    public bool T1accumulateOverTime { get; set; }

    public bool T1validateMinOverTime { get; set; }

    public int T1minOverTime { get; set; }

    public bool T2beginOverTime { get; set; }

    public int T2beginOverTimeHour { get; set; }

    public int T2beginOverTimeMinute { get; set; }

    public int T2beginOverTimeFactor { get; set; }

    public bool T2validateMinBeginOverTime { get; set; }

    public int T2minBeginOverTime { get; set; }

    public int T2inHour { get; set; } = 0;

    public int T2inMinute { get; set; }

    public int T2outHour { get; set; }

    public int T2outMinute { get; set; }

    public bool T2endOverTime1 { get; set; }

    public int T2overTime1BeginHour { get; set; }

    public int T2overTime1BeginMinute { get; set; }

    public int T2overTime1EndHour { get; set; }

    public int T2overTime1EndMinute { get; set; }

    public int T2overTime1Factor { get; set; }

    public bool T2endOverTime2 { get; set; }

    public int T2overTime2BeginHour { get; set; }

    public int T2overTime2BeginMinute { get; set; }

    public int T2overTime2EndHour { get; set; }

    public int T2overTime2EndMinute { get; set; }

    public int T2overTime2Factor { get; set; }

    public bool T2endOverTime3 { get; set; }

    public int T2overTime3BeginHour { get; set; }

    public int T2overTime3BeginMinute { get; set; }

    public int T2overTime3EndHour { get; set; }

    public int T2overTime3EndMinute { get; set; }

    public int T2overTime3Factor { get; set; }

    public bool T2endOverTime4 { get; set; }

    public int T2overTime4BeginHour { get; set; }

    public int T2overTime4BeginMinute { get; set; }

    public int T2overTime4EndHour { get; set; }

    public int T2overTime4EndMinute { get; set; }

    public int T2overTime4Factor { get; set; }

    public bool T2endOverTime5 { get; set; }

    public int T2overTime5BeginHour { get; set; }

    public int T2overTime5BeginMinute { get; set; }

    public int T2overTime5EndHour { get; set; }

    public int T2overTime5EndMinute { get; set; }

    public int T2overTime5Factor { get; set; }

    public bool T2validateMinOverTime { get; set; }

    public int T2minOverTime { get; set; }

    public int RestType { get; set; }

    public int Rt1minute { get; set; }

    public int Rt1max { get; set; }

    public int Rt21beginHour { get; set; }

    public int Rt21beginMinute { get; set; }

    public int Rt21endHour { get; set; }

    public int Rt21endMinute { get; set; }

    public bool Rt22 { get; set; }

    public int Rt22beginHour { get; set; }

    public int Rt22beginMinute { get; set; }

    public int Rt22endHour { get; set; }

    public int Rt22endMinute { get; set; }

    public bool Rt23 { get; set; }

    public int Rt23beginHour { get; set; }

    public int Rt23beginMinute { get; set; }

    public int Rt23endHour { get; set; }

    public int Rt23endMinute { get; set; }

    public bool Rt2overTime { get; set; }

    public int Rt2overTimeFactor { get; set; }

    public bool Rt2validateMinOverTime { get; set; }

    public int Rt2minOverTime { get; set; }

    public int AutoRestMinute { get; set; }

    public bool? LeastTimeAutoAssigned { get; set; }

    public bool? PayExtraTimeOnBegin { get; set; }

    public bool? PayExtraTimeOnEnd { get; set; }

    public int PayFactExtraTimeOnBegin { get; set; }

    public int PayFactExtraTimeOnEnd { get; set; }

    public bool? ValidateExtraTimeOnBegin { get; set; }

    public bool? ValidateExtraTimeOnEnd { get; set; }

    public int? MinExtraTimeOnBegin { get; set; }

    public int? MinExtraTimeOnEnd { get; set; }
}
