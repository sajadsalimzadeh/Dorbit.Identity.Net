using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorbit.Identity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCellphoneAndEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Cellphone",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Cellphone",
                schema: "identity",
                table: "Users",
                column: "Cellphone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}
