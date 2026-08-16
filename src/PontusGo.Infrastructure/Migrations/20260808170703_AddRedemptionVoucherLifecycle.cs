using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontusGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRedemptionVoucherLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "Redemptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Redemptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Redemptions",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "ValidatedByAdminId",
                table: "Redemptions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Redemptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Redemptions"
                SET "VoucherCode" = 'PG-LEGACY-' || UPPER(SUBSTRING(REPLACE("Id"::text, '-', '') FROM 1 FOR 8)),
                    "ExpiresAt" = "CreatedAt" + INTERVAL '7 days',
                    "CollectedAt" = "CreatedAt";
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "Redemptions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VoucherCode",
                table: "Redemptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Redemptions_VoucherCode",
                table: "Redemptions",
                column: "VoucherCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Redemptions_VoucherCode",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "ValidatedByAdminId",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Redemptions");
        }
    }
}
