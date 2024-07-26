using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFlowRRHH.Migrations
{
    /// <inheritdoc />
    public partial class addFeriados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    IdDepartment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdParent = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    SupervisorName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    SupervisorEmail = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Comment = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    DepartamentosSuperiores = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    DepartamentosInferiores = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaDepartment_PK", x => x.IdDepartment)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    IdDevice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineNumber = table.Column<int>(type: "int", nullable: false),
                    MachinePassword = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Comment = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ConnectionType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    IP = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PortNumber = table.Column<short>(type: "smallint", nullable: true),
                    SerialPort = table.Column<int>(type: "int", nullable: true),
                    BaudRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Connect = table.Column<bool>(type: "bit", nullable: false),
                    Synchronize = table.Column<bool>(type: "bit", nullable: false),
                    DownloadRecords = table.Column<bool>(type: "bit", nullable: false),
                    Attendance = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaDevice_PK", x => x.IdDevice)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Feriado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recurrente = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Factor = table.Column<int>(type: "int", nullable: false),
                    Depart = table.Column<int>(type: "int", nullable: false),
                    Employee = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feriado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Comment = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CuttingHour = table.Column<int>(type: "int", nullable: true),
                    CuttingMinute = table.Column<int>(type: "int", nullable: true),
                    Cycle = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaShift_PK", x => x.ShiftId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Gender = table.Column<short>(type: "smallint", nullable: true),
                    Title = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MobileNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ExternalReference = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IdDepartment = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Active = table.Column<short>(type: "smallint", nullable: false),
                    Picture = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    PictureOrientation = table.Column<short>(type: "smallint", nullable: true),
                    Privilege = table.Column<int>(type: "int", nullable: false),
                    HourSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PreferredIdLanguage = table.Column<short>(type: "smallint", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    ProximityCard = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LastRecord = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    AdministratorType = table.Column<int>(type: "int", nullable: true),
                    IdProfile = table.Column<int>(type: "int", nullable: true),
                    DevPassword = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UseShift = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<int>(type: "int", nullable: true),
                    SMSPhone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TemplateCode = table.Column<int>(type: "int", nullable: true),
                    ApplyExceptionPermition = table.Column<bool>(type: "bit", nullable: true),
                    ExceptionPermitionBegin = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExceptionPermitionEnd = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaUser_PK", x => x.IdUser)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_User_Department_IdDepartment",
                        column: x => x.IdDepartment,
                        principalTable: "Department",
                        principalColumn: "IdDepartment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftDetail",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    DayId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    T1AttTime = table.Column<int>(type: "int", nullable: false),
                    T1OverTime1 = table.Column<bool>(type: "bit", nullable: false),
                    T1OverTime1Minutes = table.Column<int>(type: "int", nullable: false),
                    T1OverTime1Factor = table.Column<int>(type: "int", nullable: false),
                    T1OverTime2 = table.Column<bool>(type: "bit", nullable: false),
                    T1OverTime2Minutes = table.Column<int>(type: "int", nullable: false),
                    T1OverTime2Factor = table.Column<int>(type: "int", nullable: false),
                    T1OverTime3 = table.Column<bool>(type: "bit", nullable: false),
                    T1OverTime3Minutes = table.Column<int>(type: "int", nullable: false),
                    T1OverTime3Factor = table.Column<int>(type: "int", nullable: false),
                    T1OverTime4 = table.Column<bool>(type: "bit", nullable: false),
                    T1OverTime4Minutes = table.Column<int>(type: "int", nullable: false),
                    T1OverTime4Factor = table.Column<int>(type: "int", nullable: false),
                    T1OverTime5 = table.Column<bool>(type: "bit", nullable: false),
                    T1OverTime5Minutes = table.Column<int>(type: "int", nullable: false),
                    T1OverTime5Factor = table.Column<int>(type: "int", nullable: false),
                    T1AccumulateOverTime = table.Column<bool>(type: "bit", nullable: false),
                    T1ValidateMinOverTime = table.Column<bool>(type: "bit", nullable: false),
                    T1MinOverTime = table.Column<int>(type: "int", nullable: false),
                    T2BeginOverTime = table.Column<bool>(type: "bit", nullable: false),
                    T2BeginOverTimeHour = table.Column<int>(type: "int", nullable: false),
                    T2BeginOverTimeMinute = table.Column<int>(type: "int", nullable: false),
                    T2BeginOverTimeFactor = table.Column<int>(type: "int", nullable: false),
                    T2ValidateMinBeginOverTime = table.Column<bool>(type: "bit", nullable: false),
                    T2MinBeginOverTime = table.Column<int>(type: "int", nullable: false),
                    T2InHour = table.Column<int>(type: "int", nullable: false),
                    T2InMinute = table.Column<int>(type: "int", nullable: false),
                    T2OutHour = table.Column<int>(type: "int", nullable: false),
                    T2OutMinute = table.Column<int>(type: "int", nullable: false),
                    T2EndOverTime1 = table.Column<bool>(type: "bit", nullable: false),
                    T2OverTime1BeginHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime1BeginMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime1EndHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime1EndMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime1Factor = table.Column<int>(type: "int", nullable: false),
                    T2EndOverTime2 = table.Column<bool>(type: "bit", nullable: false),
                    T2OverTime2BeginHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime2BeginMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime2EndHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime2EndMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime2Factor = table.Column<int>(type: "int", nullable: false),
                    T2EndOverTime3 = table.Column<bool>(type: "bit", nullable: false),
                    T2OverTime3BeginHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime3BeginMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime3EndHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime3EndMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime3Factor = table.Column<int>(type: "int", nullable: false),
                    T2EndOverTime4 = table.Column<bool>(type: "bit", nullable: false),
                    T2OverTime4BeginHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime4BeginMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime4EndHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime4EndMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime4Factor = table.Column<int>(type: "int", nullable: false),
                    T2EndOverTime5 = table.Column<bool>(type: "bit", nullable: false),
                    T2OverTime5BeginHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime5BeginMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime5EndHour = table.Column<int>(type: "int", nullable: false),
                    T2OverTime5EndMinute = table.Column<int>(type: "int", nullable: false),
                    T2OverTime5Factor = table.Column<int>(type: "int", nullable: false),
                    T2ValidateMinOverTime = table.Column<bool>(type: "bit", nullable: false),
                    T2MinOverTime = table.Column<int>(type: "int", nullable: false),
                    RestType = table.Column<int>(type: "int", nullable: false),
                    RT1Minute = table.Column<int>(type: "int", nullable: false),
                    RT1Max = table.Column<int>(type: "int", nullable: false),
                    RT21BeginHour = table.Column<int>(type: "int", nullable: false),
                    RT21BeginMinute = table.Column<int>(type: "int", nullable: false),
                    RT21EndHour = table.Column<int>(type: "int", nullable: false),
                    RT21EndMinute = table.Column<int>(type: "int", nullable: false),
                    RT22 = table.Column<bool>(type: "bit", nullable: false),
                    RT22BeginHour = table.Column<int>(type: "int", nullable: false),
                    RT22BeginMinute = table.Column<int>(type: "int", nullable: false),
                    RT22EndHour = table.Column<int>(type: "int", nullable: false),
                    RT22EndMinute = table.Column<int>(type: "int", nullable: false),
                    RT23 = table.Column<bool>(type: "bit", nullable: false),
                    RT23BeginHour = table.Column<int>(type: "int", nullable: false),
                    RT23BeginMinute = table.Column<int>(type: "int", nullable: false),
                    RT23EndHour = table.Column<int>(type: "int", nullable: false),
                    RT23EndMinute = table.Column<int>(type: "int", nullable: false),
                    RT2OverTime = table.Column<bool>(type: "bit", nullable: false),
                    RT2OverTimeFactor = table.Column<int>(type: "int", nullable: false),
                    RT2ValidateMinOverTime = table.Column<bool>(type: "bit", nullable: false),
                    RT2MinOverTime = table.Column<int>(type: "int", nullable: false),
                    AutoRestMinute = table.Column<int>(type: "int", nullable: false),
                    LeastTimeAutoAssigned = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    PayExtraTimeOnBegin = table.Column<bool>(type: "bit", nullable: true),
                    PayExtraTimeOnEnd = table.Column<bool>(type: "bit", nullable: true),
                    PayFactExtraTimeOnBegin = table.Column<int>(type: "int", nullable: false),
                    PayFactExtraTimeOnEnd = table.Column<int>(type: "int", nullable: false),
                    ValidateExtraTimeOnBegin = table.Column<bool>(type: "bit", nullable: true),
                    ValidateExtraTimeOnEnd = table.Column<bool>(type: "bit", nullable: true),
                    MinExtraTimeOnBegin = table.Column<int>(type: "int", nullable: true),
                    MinExtraTimeOnEnd = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaShiftDetail_PK", x => new { x.ShiftId, x.DayId })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ShiftDetail_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shift",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    RecordTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RecordType = table.Column<int>(type: "int", nullable: false),
                    MachineNumber = table.Column<int>(type: "int", nullable: false),
                    VerifyMode = table.Column<int>(type: "int", nullable: true),
                    Workcode = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaRecord_PK", x => new { x.IdUser, x.RecordTime, x.RecordType })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Record_Device_MachineNumber",
                        column: x => x.MachineNumber,
                        principalTable: "Device",
                        principalColumn: "IdDevice",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Record_FK00",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "UserShift",
                columns: table => new
                {
                    UserShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: true),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaUserShift_PK", x => x.UserShiftId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_UserShift_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shift",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserShift_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Record_MachineNumber",
                table: "Record",
                column: "MachineNumber");

            migrationBuilder.CreateIndex(
                name: "RecordIdUser",
                table: "Record",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "UserRecord",
                table: "Record",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "DayId",
                table: "ShiftDetail",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdDepartment",
                table: "User",
                column: "IdDepartment");

            migrationBuilder.CreateIndex(
                name: "UserCreatedBy",
                table: "User",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "UserModifiedBy",
                table: "User",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserShift_IdUser",
                table: "UserShift",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "ShiftId",
                table: "UserShift",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "UserShiftId",
                table: "UserShift",
                column: "UserShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feriado");

            migrationBuilder.DropTable(
                name: "Record");

            migrationBuilder.DropTable(
                name: "ShiftDetail");

            migrationBuilder.DropTable(
                name: "UserShift");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
