using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dorbit.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesses",
                schema: "identity");

            migrationBuilder.EnsureSchema(
                name: "public");

            // migrationBuilder.CreateSequence<int>(
            //     name: "seq_user_code",
            //     schema: "public");

            migrationBuilder.AlterColumn<int>(
                name: "Code",
                schema: "identity",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('public.seq_user_code')",
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                schema: "identity",
                table: "Users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                schema: "identity",
                table: "Users");

            // migrationBuilder.DropSequence(
            //     name: "seq_user_code",
            //     schema: "public");

            migrationBuilder.AlterColumn<int>(
                name: "Code",
                schema: "identity",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('public.seq_user_code')")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "Accesses",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeleterId = table.Column<string>(type: "text", nullable: true),
                    DeleterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifierId = table.Column<string>(type: "text", nullable: true),
                    ModifierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_ParentId",
                schema: "identity",
                table: "Accesses",
                column: "ParentId");
        }
    }
}
