using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInInvoice_Prices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                schema: "dbo",
                table: "Invoices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraCosts",
                schema: "dbo",
                table: "Invoices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceNetPrice",
                schema: "dbo",
                table: "Invoices",
                type: "decimal(24,4)",
                precision: 24,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "Invoices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExtraCosts",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNetPrice",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "Invoices");
        }
    }
}
