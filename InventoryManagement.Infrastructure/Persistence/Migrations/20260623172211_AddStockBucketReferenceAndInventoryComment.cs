using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBucketReferenceAndInventoryComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "StockBuckets",
                type: "TEXT",
                maxLength: 21,
                nullable: true);

            migrationBuilder.Sql(
                """
                WITH RankedBuckets AS (
                    SELECT "Id", ROW_NUMBER() OVER (ORDER BY "CreatedAt", "Id") AS "Sequence"
                    FROM "StockBuckets"
                )
                UPDATE "StockBuckets"
                SET "Reference" = 'ref-lot-' || printf('%013d', (
                    SELECT "Sequence"
                    FROM RankedBuckets
                    WHERE RankedBuckets."Id" = "StockBuckets"."Id"
                ));
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "StockBuckets",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 21,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InventoryComment",
                table: "ArticleMovements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_Reference",
                table: "StockBuckets",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockBuckets_Reference",
                table: "StockBuckets");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "StockBuckets");

            migrationBuilder.DropColumn(
                name: "InventoryComment",
                table: "ArticleMovements");
        }
    }
}
