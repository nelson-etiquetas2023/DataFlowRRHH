using System.ComponentModel.DataAnnotations.Schema;

namespace DataFlowRRHH.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? IdentificationNumber { get; set; }

    public string Name { get; set; } = null!;

    public short? Gender { get; set; }

    public string? Title { get; set; }

    public DateTime? Birthday { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobileNumber { get; set; }

    public string? Address { get; set; }

    public string ExternalReference { get; set; } = null!;

    public int IdDepartment { get; set; }

    public string? Position { get; set; }

    public short Active { get; set; }

    public string? Picture { get; set; }

    public short? PictureOrientation { get; set; }

    public int Privilege { get; set; }

    public decimal HourSalary { get; set; }

    public string? Password { get; set; }

    public short PreferredIdLanguage { get; set; }

    public string? Email { get; set; }

    public string? Comment { get; set; }

    public string? ProximityCard { get; set; }

    public DateTime? LastRecord { get; set; }

    public DateTime? LastLogin { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDatetime { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDatetime { get; set; }

    public int? AdministratorType { get; set; }

    public int? IdProfile { get; set; }

    public string? DevPassword { get; set; }

    public bool UseShift { get; set; }

    public int? SendSms { get; set; }

    public string? Smsphone { get; set; }

    public int? TemplateCode { get; set; }

    public bool? ApplyExceptionPermition { get; set; }

    public DateTime? ExceptionPermitionBegin { get; set; }

    public DateTime? ExceptionPermitionEnd { get; set; }

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    [ForeignKey("IdDepartment")]
    public virtual Department IdDepartmentNavigation { get; set; } = null!;

    [ForeignKey("IdUser")]
    public virtual ICollection<UserShift> UserShiftNavigation { get; set; } = new List<UserShift>();


}
