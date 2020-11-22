using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class BillingCustomerUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingDataId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BillingData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    TaxNumber = table.Column<string>(maxLength: 13, nullable: true),
                    Address = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BillingDataId",
                table: "Invoices",
                column: "BillingDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices",
                column: "BillingDataId",
                principalTable: "BillingData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_BillingData_BillingDataId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "BillingData");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BillingDataId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BillingDataId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");
        }
    }
}
