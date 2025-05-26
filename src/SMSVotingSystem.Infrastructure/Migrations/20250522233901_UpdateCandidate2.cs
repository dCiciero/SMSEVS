using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMSVotingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCandidate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candidates_ShortCode",
                table: "Candidates");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Candidates",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_ShortCode",
                table: "Candidates",
                column: "ShortCode",
                unique: true,
                filter: "[ShortCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candidates_ShortCode",
                table: "Candidates");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Candidates",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_ShortCode",
                table: "Candidates",
                column: "ShortCode",
                unique: true);
        }
    }
}
