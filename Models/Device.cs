namespace DataFlowRRHH.Models;

public partial class Device
{
    public int IdDevice { get; set; }

    public int MachineNumber { get; set; }

    public string? MachinePassword { get; set; }

    public string Description { get; set; } = null!;

    public string? Comment { get; set; }

    public string ConnectionType { get; set; } = null!;

    public string? Ip { get; set; }

    public short? PortNumber { get; set; }

    public int? SerialPort { get; set; }

    public decimal? BaudRate { get; set; }

    public int Type { get; set; }

    public bool Connect { get; set; }

    public bool Synchronize { get; set; }

    public bool DownloadRecords { get; set; }

    public int? Attendance { get; set; }
}
