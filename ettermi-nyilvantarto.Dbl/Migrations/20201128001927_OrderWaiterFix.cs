using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class OrderWaiterFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_WaiterId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WaiterId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WaiterId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WaiterUserId",
                table: "Orders",
                column: "WaiterUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_WaiterUserId",
                table: "Orders",
                column: "WaiterUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_WaiterUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WaiterUserId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "WaiterId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WaiterId",
                table: "Orders",
                column: "WaiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_WaiterId",
                table: "Orders",
                column: "WaiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
