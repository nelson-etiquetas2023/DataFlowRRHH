using Microsoft.EntityFrameworkCore;

namespace DataFlowRRHH.Models;

public partial class BdbioAdminSqlContext : DbContext
{

    public BdbioAdminSqlContext(DbContextOptions<BdbioAdminSqlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserShift> UserShifts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment)
                .HasName("aaaaaDepartment_PK")
                .IsClustered(false);

            entity.ToTable("Department");

            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DepartamentosInferiores).IsUnicode(false);
            entity.Property(e => e.DepartamentosSuperiores).IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SupervisorEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SupervisorName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.IdDevice)
                .HasName("aaaaaDevice_PK")
                .IsClustered(false);

            entity.ToTable("Device");

            entity.Property(e => e.BaudRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ConnectionType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Ip)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IP");
            entity.Property(e => e.MachinePassword)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.RecordTime, e.RecordType })
                .HasName("aaaaaRecord_PK")
                .IsClustered(false);

            entity.ToTable("Record");

            entity.HasIndex(e => e.IdUser, "RecordIdUser");

            entity.HasIndex(e => e.IdUser, "UserRecord");

            entity.Property(e => e.RecordTime).HasColumnType("datetime");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Records)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Record_FK00");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId)
                .HasName("aaaaaShift_PK")
                .IsClustered(false);

            entity.ToTable("Shift");

            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.HasKey(e => new { e.ShiftId, e.DayId })
                .HasName("aaaaaShiftDetail_PK")
                .IsClustered(false);

            entity.ToTable("ShiftDetail");

            entity.HasIndex(e => e.DayId, "DayId");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LeastTimeAutoAssigned).HasDefaultValueSql("((0))");
            entity.Property(e => e.Rt1max).HasColumnName("RT1Max");
            entity.Property(e => e.Rt1minute).HasColumnName("RT1Minute");
            entity.Property(e => e.Rt21beginHour).HasColumnName("RT21BeginHour");
            entity.Property(e => e.Rt21beginMinute).HasColumnName("RT21BeginMinute");
            entity.Property(e => e.Rt21endHour).HasColumnName("RT21EndHour");
            entity.Property(e => e.Rt21endMinute).HasColumnName("RT21EndMinute");
            entity.Property(e => e.Rt22).HasColumnName("RT22");
            entity.Property(e => e.Rt22beginHour).HasColumnName("RT22BeginHour");
            entity.Property(e => e.Rt22beginMinute).HasColumnName("RT22BeginMinute");
            entity.Property(e => e.Rt22endHour).HasColumnName("RT22EndHour");
            entity.Property(e => e.Rt22endMinute).HasColumnName("RT22EndMinute");
            entity.Property(e => e.Rt23).HasColumnName("RT23");
            entity.Property(e => e.Rt23beginHour).HasColumnName("RT23BeginHour");
            entity.Property(e => e.Rt23beginMinute).HasColumnName("RT23BeginMinute");
            entity.Property(e => e.Rt23endHour).HasColumnName("RT23EndHour");
            entity.Property(e => e.Rt23endMinute).HasColumnName("RT23EndMinute");
            entity.Property(e => e.Rt2minOverTime).HasColumnName("RT2MinOverTime");
            entity.Property(e => e.Rt2overTime).HasColumnName("RT2OverTime");
            entity.Property(e => e.Rt2overTimeFactor).HasColumnName("RT2OverTimeFactor");
            entity.Property(e => e.Rt2validateMinOverTime).HasColumnName("RT2ValidateMinOverTime");
            entity.Property(e => e.T1accumulateOverTime).HasColumnName("T1AccumulateOverTime");
            entity.Property(e => e.T1attTime).HasColumnName("T1AttTime");
            entity.Property(e => e.T1minOverTime).HasColumnName("T1MinOverTime");
            entity.Property(e => e.T1overTime1).HasColumnName("T1OverTime1");
            entity.Property(e => e.T1overTime1Factor).HasColumnName("T1OverTime1Factor");
            entity.Property(e => e.T1overTime1Minutes).HasColumnName("T1OverTime1Minutes");
            entity.Property(e => e.T1overTime2).HasColumnName("T1OverTime2");
            entity.Property(e => e.T1overTime2Factor).HasColumnName("T1OverTime2Factor");
            entity.Property(e => e.T1overTime2Minutes).HasColumnName("T1OverTime2Minutes");
            entity.Property(e => e.T1overTime3).HasColumnName("T1OverTime3");
            entity.Property(e => e.T1overTime3Factor).HasColumnName("T1OverTime3Factor");
            entity.Property(e => e.T1overTime3Minutes).HasColumnName("T1OverTime3Minutes");
            entity.Property(e => e.T1overTime4).HasColumnName("T1OverTime4");
            entity.Property(e => e.T1overTime4Factor).HasColumnName("T1OverTime4Factor");
            entity.Property(e => e.T1overTime4Minutes).HasColumnName("T1OverTime4Minutes");
            entity.Property(e => e.T1overTime5).HasColumnName("T1OverTime5");
            entity.Property(e => e.T1overTime5Factor).HasColumnName("T1OverTime5Factor");
            entity.Property(e => e.T1overTime5Minutes).HasColumnName("T1OverTime5Minutes");
            entity.Property(e => e.T1validateMinOverTime).HasColumnName("T1ValidateMinOverTime");
            entity.Property(e => e.T2beginOverTime).HasColumnName("T2BeginOverTime");
            entity.Property(e => e.T2beginOverTimeFactor).HasColumnName("T2BeginOverTimeFactor");
            entity.Property(e => e.T2beginOverTimeHour).HasColumnName("T2BeginOverTimeHour");
            entity.Property(e => e.T2beginOverTimeMinute).HasColumnName("T2BeginOverTimeMinute");
            entity.Property(e => e.T2endOverTime1).HasColumnName("T2EndOverTime1");
            entity.Property(e => e.T2endOverTime2).HasColumnName("T2EndOverTime2");
            entity.Property(e => e.T2endOverTime3).HasColumnName("T2EndOverTime3");
            entity.Property(e => e.T2endOverTime4).HasColumnName("T2EndOverTime4");
            entity.Property(e => e.T2endOverTime5).HasColumnName("T2EndOverTime5");
            entity.Property(e => e.T2inHour).HasColumnName("T2InHour");
            entity.Property(e => e.T2inMinute).HasColumnName("T2InMinute");
            entity.Property(e => e.T2minBeginOverTime).HasColumnName("T2MinBeginOverTime");
            entity.Property(e => e.T2minOverTime).HasColumnName("T2MinOverTime");
            entity.Property(e => e.T2outHour).HasColumnName("T2OutHour");
            entity.Property(e => e.T2outMinute).HasColumnName("T2OutMinute");
            entity.Property(e => e.T2overTime1BeginHour).HasColumnName("T2OverTime1BeginHour");
            entity.Property(e => e.T2overTime1BeginMinute).HasColumnName("T2OverTime1BeginMinute");
            entity.Property(e => e.T2overTime1EndHour).HasColumnName("T2OverTime1EndHour");
            entity.Property(e => e.T2overTime1EndMinute).HasColumnName("T2OverTime1EndMinute");
            entity.Property(e => e.T2overTime1Factor).HasColumnName("T2OverTime1Factor");
            entity.Property(e => e.T2overTime2BeginHour).HasColumnName("T2OverTime2BeginHour");
            entity.Property(e => e.T2overTime2BeginMinute).HasColumnName("T2OverTime2BeginMinute");
            entity.Property(e => e.T2overTime2EndHour).HasColumnName("T2OverTime2EndHour");
            entity.Property(e => e.T2overTime2EndMinute).HasColumnName("T2OverTime2EndMinute");
            entity.Property(e => e.T2overTime2Factor).HasColumnName("T2OverTime2Factor");
            entity.Property(e => e.T2overTime3BeginHour).HasColumnName("T2OverTime3BeginHour");
            entity.Property(e => e.T2overTime3BeginMinute).HasColumnName("T2OverTime3BeginMinute");
            entity.Property(e => e.T2overTime3EndHour).HasColumnName("T2OverTime3EndHour");
            entity.Property(e => e.T2overTime3EndMinute).HasColumnName("T2OverTime3EndMinute");
            entity.Property(e => e.T2overTime3Factor).HasColumnName("T2OverTime3Factor");
            entity.Property(e => e.T2overTime4BeginHour).HasColumnName("T2OverTime4BeginHour");
            entity.Property(e => e.T2overTime4BeginMinute).HasColumnName("T2OverTime4BeginMinute");
            entity.Property(e => e.T2overTime4EndHour).HasColumnName("T2OverTime4EndHour");
            entity.Property(e => e.T2overTime4EndMinute).HasColumnName("T2OverTime4EndMinute");
            entity.Property(e => e.T2overTime4Factor).HasColumnName("T2OverTime4Factor");
            entity.Property(e => e.T2overTime5BeginHour).HasColumnName("T2OverTime5BeginHour");
            entity.Property(e => e.T2overTime5BeginMinute).HasColumnName("T2OverTime5BeginMinute");
            entity.Property(e => e.T2overTime5EndHour).HasColumnName("T2OverTime5EndHour");
            entity.Property(e => e.T2overTime5EndMinute).HasColumnName("T2OverTime5EndMinute");
            entity.Property(e => e.T2overTime5Factor).HasColumnName("T2OverTime5Factor");
            entity.Property(e => e.T2validateMinBeginOverTime).HasColumnName("T2ValidateMinBeginOverTime");
            entity.Property(e => e.T2validateMinOverTime).HasColumnName("T2ValidateMinOverTime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser)
                .HasName("aaaaaUser_PK")
                .IsClustered(false);

            entity.ToTable("User");

            entity.HasIndex(e => e.CreatedBy, "UserCreatedBy");

            entity.HasIndex(e => e.ModifiedBy, "UserModifiedBy");

            entity.Property(e => e.IdUser).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Comment)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDatetime).HasColumnType("datetime");
            entity.Property(e => e.DevPassword)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ExceptionPermitionBegin).HasColumnType("datetime");
            entity.Property(e => e.ExceptionPermitionEnd).HasColumnType("datetime");
            entity.Property(e => e.ExternalReference)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HourSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdentificationNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LastRecord).HasColumnType("datetime");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDatetime).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
            entity.Property(e => e.Position)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ProximityCard)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SendSms).HasColumnName("SendSMS");
            entity.Property(e => e.Smsphone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMSPhone");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserShift>(entity =>
        {
            entity.HasKey(e => e.UserShiftId)
                .HasName("aaaaaUserShift_PK")
                .IsClustered(false);

            entity.ToTable("UserShift");

            entity.HasIndex(e => e.ShiftId, "ShiftId");

            entity.HasIndex(e => e.UserShiftId, "UserShiftId");

            entity.Property(e => e.BeginDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
