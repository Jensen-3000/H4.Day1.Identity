using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace H4.Day1.Identity.Data.Migrations.TodoDb
{
    /// <inheritdoc />
    public partial class InitTodoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cpr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CprNr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cpr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todolist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todolist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todolist_Cpr",
                        column: x => x.UserId,
                        principalTable: "Cpr",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Todolist_UserId",
                table: "Todolist",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todolist");

            migrationBuilder.DropTable(
                name: "Cpr");
        }
    }
}
