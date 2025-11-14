using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aesclea_Back_End_.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Users");
        }
    }
}
