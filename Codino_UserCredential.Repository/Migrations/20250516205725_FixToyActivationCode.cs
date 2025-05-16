using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Codino_UserCredential.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixToyActivationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToyActivationCode",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ToyId = table.Column<int>(type: "integer", nullable: false),
                    IsActivated = table.Column<bool>(type: "boolean", nullable: false),
                    ActivatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ActivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToyActivationCode", x => x.id);
                    table.ForeignKey(
                        name: "FK_ToyActivationCode_Toy_ToyId",
                        column: x => x.ToyId,
                        principalSchema: "content",
                        principalTable: "Toy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToyActivationCode_User_ActivatedByUserId",
                        column: x => x.ActivatedByUserId,
                        principalSchema: "user",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToyActivationCode_ActivatedByUserId",
                schema: "content",
                table: "ToyActivationCode",
                column: "ActivatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ToyActivationCode_ToyId",
                schema: "content",
                table: "ToyActivationCode",
                column: "ToyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToyActivationCode",
                schema: "content");
        }
    }
}
