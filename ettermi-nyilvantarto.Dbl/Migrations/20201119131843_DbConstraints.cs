using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class DbConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherNumber",
                table: "Vouchers");

            migrationBuilder.CreateCheckConstraint(
                name: "CK_VoucherDates",
                table: "Vouchers",
                sql: "Timefrom < TimeTo");

            migrationBuilder.CreateCheckConstraint(
                name: "CK_ReservationDates",
                table: "Reservations",
                sql: "Timefrom < TimeTo");

            migrationBuilder.CreateCheckConstraint(
                name: "CK_FeedbackRating",
                table: "Feedback",
                sql: "Rating >= 0 AND Rating <= 5");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Vouchers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tables",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MenuItems",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_VoucherDates",
                table: "Vouchers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ReservationDates",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FeedbackRating",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Vouchers");

            migrationBuilder.AddColumn<int>(
                name: "VoucherNumber",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tables",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 15,
                oldNullable: true);
        }
    }
}
