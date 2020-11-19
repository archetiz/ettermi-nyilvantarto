using Microsoft.EntityFrameworkCore.Migrations;

namespace ettermi_nyilvantarto.Dbl.Migrations
{
    public partial class Categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Tables",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "MenuItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MenuItemCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MenuItemCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Előétel" },
                    { 2, "Főétel" },
                    { 3, "Köret" },
                    { 4, "Desszert" },
                    { 5, "Ital" }
                });

            migrationBuilder.InsertData(
                table: "TableCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Terasz" },
                    { 2, "Családi" },
                    { 3, "Rendezvények" },
                    { 4, "Kicsi" },
                    { 6, "Normál" },
                    { 7, "Nagy" },
                    { 8, "Négy lába van, de nem szék" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_CategoryId",
                table: "Tables",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuItemCategories_CategoryId",
                table: "MenuItems",
                column: "CategoryId",
                principalTable: "MenuItemCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableCategories_CategoryId",
                table: "Tables",
                column: "CategoryId",
                principalTable: "TableCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuItemCategories_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableCategories_CategoryId",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "MenuItemCategories");

            migrationBuilder.DropTable(
                name: "TableCategories");

            migrationBuilder.DropIndex(
                name: "IX_Tables_CategoryId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "MenuItems");
        }
    }
}
