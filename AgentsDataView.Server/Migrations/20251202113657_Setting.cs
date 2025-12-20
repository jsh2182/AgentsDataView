using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class Setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Companies_ProvinceId",
                schema: "dbo",
                table: "Companies",
                newName: "IX_Company_ProvinceId");

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Settings",
                columns: new[] { "Id", "SettingName", "SettingValue" },
                values: new object[] { 1, "LastUpdateDate", "" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code_Name",
                schema: "dbo",
                table: "Products",
                columns: new[] { "Code", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Type",
                schema: "dbo",
                table: "Invoices",
                column: "InvoiceType");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_IsDraft",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "IsDraft");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_Name",
                schema: "dbo",
                table: "Settings",
                column: "SettingName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Products_Code_Name",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_Type",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_IsDraft",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.RenameIndex(
                name: "IX_Company_ProvinceId",
                schema: "dbo",
                table: "Companies",
                newName: "IX_Companies_ProvinceId");
        }
    }
}
