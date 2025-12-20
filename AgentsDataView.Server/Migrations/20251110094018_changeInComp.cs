using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeInComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDate",
                schema: "dbo",
                table: "Companies",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "decimal(22,4)",
                precision: 22,
                scale: 4,
                nullable: false,
                computedColumnSql: "[Quantity] * [UnitPrice]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,4)",
                oldPrecision: 22,
                oldScale: 4,
                oldComputedColumnSql: "[ProductCount] * [UnitPrice]",
                oldStored: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NetPrice",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "decimal(22,4)",
                precision: 22,
                scale: 4,
                nullable: false,
                computedColumnSql: "[Quantity] * [UnitPrice] - [DiscountValue] + [TaxValue]",
                oldClrType: typeof(decimal),
                oldType: "decimal(22,4)",
                oldPrecision: 22,
                oldScale: 4,
                oldComputedColumnSql: "[ProductCount] * [UnitPrice] - [Discount] + [TaxValue]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "decimal(22,4)",
                precision: 22,
                scale: 4,
                nullable: false,
                computedColumnSql: "[ProductCount] * [UnitPrice]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,4)",
                oldPrecision: 22,
                oldScale: 4,
                oldComputedColumnSql: "[Quantity] * [UnitPrice]",
                oldStored: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NetPrice",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "decimal(22,4)",
                precision: 22,
                scale: 4,
                nullable: false,
                computedColumnSql: "[ProductCount] * [UnitPrice] - [Discount] + [TaxValue]",
                oldClrType: typeof(decimal),
                oldType: "decimal(22,4)",
                oldPrecision: 22,
                oldScale: 4,
                oldComputedColumnSql: "[Quantity] * [UnitPrice] - [DiscountValue] + [TaxValue]");
        }
    }
}
