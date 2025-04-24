using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTrackOne.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateValueObjectsAndEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Valeur",
                table: "Notes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "Valeur_EstAbsent",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valeur_EstAbsent",
                table: "Notes");

            migrationBuilder.AlterColumn<double>(
                name: "Valeur",
                table: "Notes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
