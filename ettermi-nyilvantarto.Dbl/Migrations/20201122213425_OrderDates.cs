using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class OrderDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "OrderSessions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAt",
                table: "OrderSessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAt",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "OrderSessions");

            migrationBuilder.DropColumn(
                name: "OpenedAt",
                table: "OrderSessions");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OpenedAt",
                table: "Orders");
        }
    }
}
