using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrimeraPrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalBalance",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "Seller",
                table: "Auctions",
                newName: "SellerId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Version",
                table: "Wallets",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "Auctions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Auctions",
                newName: "Seller");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "Wallets",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBalance",
                table: "Wallets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
