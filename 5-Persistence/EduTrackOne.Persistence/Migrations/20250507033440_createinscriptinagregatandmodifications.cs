using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTrackOne.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class createinscriptinagregatandmodifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClasseId",
                table: "Inscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EleveId",
                table: "Inscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptions_ClasseId",
                table: "Inscriptions",
                column: "ClasseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptions_EleveId",
                table: "Inscriptions",
                column: "EleveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_Classes_ClasseId",
                table: "Inscriptions",
                column: "ClasseId",
                principalTable: "Classes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_Eleves_EleveId",
                table: "Inscriptions",
                column: "EleveId",
                principalTable: "Eleves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_Classes_ClasseId",
                table: "Inscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_Eleves_EleveId",
                table: "Inscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Inscriptions_ClasseId",
                table: "Inscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Inscriptions_EleveId",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "ClasseId",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "EleveId",
                table: "Inscriptions");
        }
    }
}
