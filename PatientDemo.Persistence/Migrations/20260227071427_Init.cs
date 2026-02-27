using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientDemo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PATIENTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    GENDER = table.Column<string>(type: "varchar(10)", nullable: false, defaultValue: "Unknown"),
                    BIRTH_DATE = table.Column<DateTime>(type: "date", nullable: false),
                    ACTIVE = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENTS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HUMAN_NAMES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    USE = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "official"),
                    FAMILY = table.Column<string>(type: "varchar(100)", nullable: false),
                    GIVEN = table.Column<string[]>(type: "varchar(30)[]", nullable: false, defaultValue: new string[0]),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HUMAN_NAMES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HUMAN_NAMES_PATIENTS_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PATIENTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HUMAN_NAMES_ID",
                table: "HUMAN_NAMES",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HUMAN_NAMES_PatientId",
                table: "HUMAN_NAMES",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PATIENTS_BIRTH_DATE",
                table: "PATIENTS",
                column: "BIRTH_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENTS_ID",
                table: "PATIENTS",
                column: "ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HUMAN_NAMES");

            migrationBuilder.DropTable(
                name: "PATIENTS");
        }
    }
}
