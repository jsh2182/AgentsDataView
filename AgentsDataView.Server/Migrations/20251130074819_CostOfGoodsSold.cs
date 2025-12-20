using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class CostOfGoodsSold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostOfGoodsSold",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostOfGoodsSold",
                schema: "dbo",
                table: "InvoiceDetails");
        }
    }
}
