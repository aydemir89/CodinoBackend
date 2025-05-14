using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Codino_UserCredential.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserTypeId = table.Column<int>(type: "integer", nullable: false),
                    UserStatusId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    TCKN = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    LastPassword = table.Column<string>(type: "text", nullable: false),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CellPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    AllowSms = table.Column<bool>(type: "boolean", nullable: false),
                    AllowEmail = table.Column<bool>(type: "boolean", nullable: false),
                    AllowKvkk = table.Column<bool>(type: "boolean", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hasRoboticModels = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarImage = table.Column<string>(type: "text", nullable: true),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginRequest",
                schema: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreaDate = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Salt = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Cellphone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Otp = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LoginStatus = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginRequest", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WorldMap",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldMap", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Biome",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WorldMapId = table.Column<int>(type: "integer", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WorldMapid = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biome", x => x.id);
                    table.ForeignKey(
                        name: "FK_Biome_WorldMap_WorldMapId",
                        column: x => x.WorldMapId,
                        principalSchema: "content",
                        principalTable: "WorldMap",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Biome_WorldMap_WorldMapid",
                        column: x => x.WorldMapid,
                        principalSchema: "content",
                        principalTable: "WorldMap",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Toy",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BiomeId = table.Column<int>(type: "integer", nullable: false),
                    IconImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Biomeid = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toy", x => x.id);
                    table.ForeignKey(
                        name: "FK_Toy_Biome_BiomeId",
                        column: x => x.BiomeId,
                        principalSchema: "content",
                        principalTable: "Biome",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Toy_Biome_Biomeid",
                        column: x => x.Biomeid,
                        principalSchema: "content",
                        principalTable: "Biome",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ToyId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExpectedPattern = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Toyid = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.id);
                    table.ForeignKey(
                        name: "FK_Task_Toy_ToyId",
                        column: x => x.ToyId,
                        principalSchema: "content",
                        principalTable: "Toy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Toy_Toyid",
                        column: x => x.Toyid,
                        principalSchema: "content",
                        principalTable: "Toy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskSubmission",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedCode = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Userid = table.Column<int>(type: "integer", nullable: false),
                    ProgrammingTaskid = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreaUserId = table.Column<int>(type: "integer", nullable: false),
                    creaDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubmission", x => x.id);
                    table.ForeignKey(
                        name: "FK_TaskSubmission_Task_ProgrammingTaskid",
                        column: x => x.ProgrammingTaskid,
                        principalSchema: "content",
                        principalTable: "Task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSubmission_Task_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "content",
                        principalTable: "Task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskSubmission_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskSubmission_User_Userid",
                        column: x => x.Userid,
                        principalSchema: "user",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Biome_WorldMapId",
                schema: "content",
                table: "Biome",
                column: "WorldMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Biome_WorldMapid",
                schema: "content",
                table: "Biome",
                column: "WorldMapid");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ToyId",
                schema: "content",
                table: "Task",
                column: "ToyId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Toyid",
                schema: "content",
                table: "Task",
                column: "Toyid");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmission_ProgrammingTaskid",
                schema: "content",
                table: "TaskSubmission",
                column: "ProgrammingTaskid");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmission_TaskId",
                schema: "content",
                table: "TaskSubmission",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmission_UserId",
                schema: "content",
                table: "TaskSubmission",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmission_Userid",
                schema: "content",
                table: "TaskSubmission",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Toy_BiomeId",
                schema: "content",
                table: "Toy",
                column: "BiomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Toy_Biomeid",
                schema: "content",
                table: "Toy",
                column: "Biomeid");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "user",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskSubmission",
                schema: "content");

            migrationBuilder.DropTable(
                name: "UserLoginRequest",
                schema: "user");

            migrationBuilder.DropTable(
                name: "Task",
                schema: "content");

            migrationBuilder.DropTable(
                name: "User",
                schema: "user");

            migrationBuilder.DropTable(
                name: "Toy",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Biome",
                schema: "content");

            migrationBuilder.DropTable(
                name: "WorldMap",
                schema: "content");
        }
    }
}
