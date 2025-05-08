using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTrackOne.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Rowversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Inscriptions",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Eleves",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Classes",
                type: "rowversion",
                rowVersion: true,
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Eleves");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Classes");
        }
    }
}
