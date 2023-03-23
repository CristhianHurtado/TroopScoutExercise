using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test2.Data.ScoutTroopMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementSummaryVM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    TroopName = table.Column<string>(type: "TEXT", nullable: true),
                    NumberOfYears = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalHL = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageLS = table.Column<double>(type: "REAL", nullable: false),
                    MaxEO = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementSummaryVM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Troops",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TroopName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TroopNumber = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    TroopBudget = table.Column<double>(type: "REAL", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Troops", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Scouts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DOB = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FeePaid = table.Column<double>(type: "REAL", nullable: false),
                    TroopID = table.Column<int>(type: "INTEGER", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scouts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Scouts_Troops_TroopID",
                        column: x => x.TroopID,
                        principalTable: "Troops",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    HL = table.Column<int>(type: "INTEGER", nullable: false),
                    BV = table.Column<int>(type: "INTEGER", nullable: false),
                    LS = table.Column<int>(type: "INTEGER", nullable: false),
                    CE = table.Column<int>(type: "INTEGER", nullable: false),
                    EO = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoutID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Achievements_Scouts_ScoutID",
                        column: x => x.ScoutID,
                        principalTable: "Scouts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scoutPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ScoutID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scoutPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_scoutPhotos_Scouts_ScoutID",
                        column: x => x.ScoutID,
                        principalTable: "Scouts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scoutThumbnails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ScoutID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scoutThumbnails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_scoutThumbnails_Scouts_ScoutID",
                        column: x => x.ScoutID,
                        principalTable: "Scouts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_ScoutID",
                table: "Achievements",
                column: "ScoutID");

            migrationBuilder.CreateIndex(
                name: "IX_scoutPhotos_ScoutID",
                table: "scoutPhotos",
                column: "ScoutID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scouts_FirstName_LastName_DOB",
                table: "Scouts",
                columns: new[] { "FirstName", "LastName", "DOB" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scouts_TroopID",
                table: "Scouts",
                column: "TroopID");

            migrationBuilder.CreateIndex(
                name: "IX_scoutThumbnails_ScoutID",
                table: "scoutThumbnails",
                column: "ScoutID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Troops_TroopNumber",
                table: "Troops",
                column: "TroopNumber",
                unique: true);

            ExtraMigration.Steps(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "AchievementSummaryVM");

            migrationBuilder.DropTable(
                name: "scoutPhotos");

            migrationBuilder.DropTable(
                name: "scoutThumbnails");

            migrationBuilder.DropTable(
                name: "Scouts");

            migrationBuilder.DropTable(
                name: "Troops");
        }
    }
}
