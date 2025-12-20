using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInInvDtl_IsDraf_Signed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_IsDraft",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "SignedForDelete",
                schema: "dbo",
                table: "InvoiceDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SignedForDelete",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_IsDraft",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "IsDraft");
        }
    }
}
