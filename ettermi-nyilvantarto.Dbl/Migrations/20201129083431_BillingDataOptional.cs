using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class BillingDataOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "BillingDataId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices",
                column: "BillingDataId",
                principalTable: "BillingData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "BillingDataId",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices",
                column: "BillingDataId",
                principalTable: "BillingData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
