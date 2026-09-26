using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilanSetu.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhysicalStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ProfileManagedBy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PartnerExpectations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HighestEducation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollegeOrUniversity = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployedIn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    WorkLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnnualIncome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FamilyType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FamilyValues = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FatherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotherOccupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumberOfBrothers = table.Column<int>(type: "int", nullable: false),
                    NumberOfSisters = table.Column<int>(type: "int", nullable: false),
                    FamilyCity = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Diet = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Drink = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Smoke = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Hobbies = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SubCasteOrGothra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManglikStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Rashi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nakshatra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NativePlace = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WillingToRelocate = table.Column<bool>(type: "bit", nullable: false),
                    PhotoGalleryJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCompletionPercentage = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
