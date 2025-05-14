using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codino_UserCredential.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixBiomeTableForeinKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Biome_WorldMap_WorldMapId",
                schema: "content",
                table: "Biome");

            migrationBuilder.DropForeignKey(
                name: "FK_Biome_WorldMap_WorldMapid",
                schema: "content",
                table: "Biome");

            migrationBuilder.DropIndex(
                name: "IX_Biome_WorldMapId",
                schema: "content",
                table: "Biome");

            migrationBuilder.DropColumn(
                name: "WorldMapId",
                schema: "content",
                table: "Biome");

            migrationBuilder.AddColumn<int>(
                name: "WorldMap",
                schema: "content",
                table: "Biome",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Biome_WorldMap_WorldMapid",
                schema: "content",
                table: "Biome",
                column: "WorldMapid",
                principalSchema: "content",
                principalTable: "WorldMap",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Biome_WorldMap_WorldMapid",
                schema: "content",
                table: "Biome");

            migrationBuilder.DropColumn(
                name: "WorldMap",
                schema: "content",
                table: "Biome");

            migrationBuilder.AddColumn<int>(
                name: "WorldMapId",
                schema: "content",
                table: "Biome",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Biome_WorldMapId",
                schema: "content",
                table: "Biome",
                column: "WorldMapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Biome_WorldMap_WorldMapId",
                schema: "content",
                table: "Biome",
                column: "WorldMapId",
                principalSchema: "content",
                principalTable: "WorldMap",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Biome_WorldMap_WorldMapid",
                schema: "content",
                table: "Biome",
                column: "WorldMapid",
                principalSchema: "content",
                principalTable: "WorldMap",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
