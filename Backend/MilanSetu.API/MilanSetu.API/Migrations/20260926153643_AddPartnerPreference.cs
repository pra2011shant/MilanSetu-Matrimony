using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilanSetu.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnerPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    MinHeight = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxHeight = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Community = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MotherTongue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Education = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MinAnnualIncome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreferredLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Diet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Drink = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Smoke = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPreferences_UserId",
                table: "PartnerPreferences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerPreferences");
        }
    }
}
