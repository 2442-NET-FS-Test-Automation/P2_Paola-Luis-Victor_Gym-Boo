using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Boo.Data.Migrations
{
    /// <inheritdoc />
    public partial class Discliplneclassadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisciplineId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DisciplineId",
                table: "Classes",
                column: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Disciplines_DisciplineId",
                table: "Classes",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Disciplines_DisciplineId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropIndex(
                name: "IX_Classes_DisciplineId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DisciplineId",
                table: "Classes");
        }
    }
}
