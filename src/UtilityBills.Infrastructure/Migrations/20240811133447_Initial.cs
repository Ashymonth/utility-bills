using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBills.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilityPaymentPlatforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    PlatformType = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityPaymentPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilityPaymentPlatformCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UtilityPaymentPlatformId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityPaymentPlatformCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilityPaymentPlatformCredentials_UtilityPaymentPlatforms_UtilityPaymentPlatformId",
                        column: x => x.UtilityPaymentPlatformId,
                        principalTable: "UtilityPaymentPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityPaymentPlatformCredentials_UtilityPaymentPlatformId",
                table: "UtilityPaymentPlatformCredentials",
                column: "UtilityPaymentPlatformId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UtilityPaymentPlatformCredentials");

            migrationBuilder.DropTable(
                name: "UtilityPaymentPlatforms");
        }
    }
}
