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

            migrationBuilder.CreateSequence<int>(
                name: "seq_user_code",
                schema: "identity");

            migrationBuilder.CreateTable(
                name: "Otp",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    TryRemain = table.Column<byte>(type: "smallint", nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Accesses = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifierId = table.Column<string>(type: "text", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<string>(type: "text", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('identity.seq_user_code')"),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Salt = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Cellphone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CellphoneValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EmailValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AuthenticatorKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AuthenticatorValidateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Thumbnail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NeedResetPassword = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Profile = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    MaxTokenCount = table.Column<short>(type: "smallint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifierId = table.Column<string>(type: "text", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<string>(type: "text", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Token",
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
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPrivilege",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    To = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    Accesses = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifierId = table.Column<string>(type: "text", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<string>(type: "text", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivilege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrivilege_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Otp_CodeHash",
                schema: "identity",
                table: "Otp",
                column: "CodeHash");

            migrationBuilder.CreateIndex(
                name: "IX_Token_UserId",
                schema: "identity",
                table: "Token",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                schema: "identity",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivilege_UserId",
                schema: "identity",
                table: "UserPrivilege",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Otp",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Token",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserPrivilege",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "User",
                schema: "identity");

            migrationBuilder.DropSequence(
                name: "seq_user_code",
                schema: "identity");
        }
    }
}
