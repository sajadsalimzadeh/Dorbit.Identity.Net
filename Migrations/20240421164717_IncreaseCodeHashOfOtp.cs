using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorbit.Identity.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseCodeHashOfOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                schema: "identity",
                table: "Otp",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Otp_CodeHash",
                schema: "identity",
                table: "Otp",
                column: "CodeHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Otp_CodeHash",
                schema: "identity",
                table: "Otp");

            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                schema: "identity",
                table: "Otp",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);
        }
    }
}
