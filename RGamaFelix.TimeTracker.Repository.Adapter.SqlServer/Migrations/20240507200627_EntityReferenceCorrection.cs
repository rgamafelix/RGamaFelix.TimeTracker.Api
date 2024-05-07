using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RGamaFelix.TimeTracker.Repository.Adapter.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class EntityReferenceCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_User_UserId1",
                table: "Session");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Session",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Session_UserId1",
                table: "Session",
                newName: "IX_Session_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_User_UserId",
                table: "Session",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_User_UserId",
                table: "Session");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Session",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Session_UserId",
                table: "Session",
                newName: "IX_Session_UserId1");

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entity = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_UserId",
                table: "Audit",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_User_UserId1",
                table: "Session",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
