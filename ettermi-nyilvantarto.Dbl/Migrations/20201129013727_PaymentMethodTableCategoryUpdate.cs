using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class PaymentMethodTableCategoryUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TableCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Egyszemélyes");

            migrationBuilder.InsertData(
                table: "TableCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Kicsi" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TableCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Invoices");

            migrationBuilder.UpdateData(
                table: "TableCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Kicsi");
        }
    }
}
