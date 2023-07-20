using System;
using System.Collections.Generic;
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

    public virtual DbSet<User> Users { get; set; }

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
