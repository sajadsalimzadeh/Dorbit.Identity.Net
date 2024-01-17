using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorbit.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "Accesses",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accesses_Accesses_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "identity",
                        principalTable: "Accesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Otp",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    TryRemain = table.Column<byte>(type: "smallint", nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Salt = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Cellphone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CellphoneValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EmailValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AuthenticatorKey = table.Column<byte[]>(type: "bytea", nullable: true),
                    AuthenticatorValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsTwoFactorAuthenticationEnable = table.Column<bool>(type: "boolean", nullable: false),
                    NeedResetPassword = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    InActiveMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ActiveTokenCount = table.Column<short>(type: "smallint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Accesses = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Privileges_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Os = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Platform = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Application = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Accesses = table.Column<string>(type: "text", nullable: true),
                    ExpireTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_ParentId",
                schema: "identity",
                table: "Accesses",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Privileges_UserId",
                schema: "identity",
                table: "Privileges",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                schema: "identity",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "identity",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesses",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Otp",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Privileges",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");
        }
    }
}
