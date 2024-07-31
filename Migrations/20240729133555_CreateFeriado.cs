using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFlowRRHH.Migrations
{
    /// <inheritdoc />
    public partial class CreateFeriado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Feriado",
                table: "Feriado");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Feriado");

            migrationBuilder.DropColumn(
                name: "Depart",
                table: "Feriado");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "Feriado");

            migrationBuilder.RenameTable(
                name: "Feriado",
                newName: "Exception");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Exception",
                newName: "PaymentFactor");

            migrationBuilder.RenameColumn(
                name: "Recurrente",
                table: "Exception",
                newName: "Recurring");

            migrationBuilder.RenameColumn(
                name: "Factor",
                table: "Exception",
                newName: "IdException");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "Exception",
                newName: "EndingDate");

            migrationBuilder.RenameColumn(
                name: "DateEnd",
                table: "Exception",
                newName: "BeginingDate");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exception",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdException",
                table: "Exception",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "aaaaaExceptionStr_PK",
                table: "Exception",
                column: "IdException");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "aaaaaExceptionStr_PK",
                table: "Exception");

            migrationBuilder.RenameTable(
                name: "Exception",
                newName: "Feriado");

            migrationBuilder.RenameColumn(
                name: "Recurring",
                table: "Feriado",
                newName: "Recurrente");

            migrationBuilder.RenameColumn(
                name: "PaymentFactor",
                table: "Feriado",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "EndingDate",
                table: "Feriado",
                newName: "DateStart");

            migrationBuilder.RenameColumn(
                name: "BeginingDate",
                table: "Feriado",
                newName: "DateEnd");

            migrationBuilder.RenameColumn(
                name: "IdException",
                table: "Feriado",
                newName: "Factor");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Feriado",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<int>(
                name: "Factor",
                table: "Feriado",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Feriado",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Depart",
                table: "Feriado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Employee",
                table: "Feriado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feriado",
                table: "Feriado",
                column: "Id");
        }
    }
}
