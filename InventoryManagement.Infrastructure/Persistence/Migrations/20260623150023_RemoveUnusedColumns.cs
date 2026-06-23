using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockBuckets_ExpirationDate",
                table: "StockBuckets");

            migrationBuilder.DropIndex(
                name: "IX_StockBuckets_PackagingLevel",
                table: "StockBuckets");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "ArticleMovements");

            migrationBuilder.AddColumn<string>(
                name: "BucketType",
                table: "StockBuckets",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PackagingLevel",
                table: "ArticleMovements",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketType",
                table: "StockBuckets");

            migrationBuilder.AlterColumn<string>(
                name: "PackagingLevel",
                table: "ArticleMovements",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpirationDate",
                table: "ArticleMovements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_ExpirationDate",
                table: "StockBuckets",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_PackagingLevel",
                table: "StockBuckets",
                column: "PackagingLevel");
        }
    }
}
