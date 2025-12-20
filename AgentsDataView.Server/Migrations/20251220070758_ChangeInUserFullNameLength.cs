using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInUserFullNameLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SystemUser_CreatedById",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Companies_CompanyId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SystemUser_CreatedById",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_SystemUser_UserId",
                schema: "dbo",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_SystemUser_UserId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserFullName",
                schema: "dbo",
                table: "SystemUser",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoiceId",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                schema: "dbo",
                table: "Invoices",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SystemUser_CreatedById",
                schema: "dbo",
                table: "Invoices",
                column: "CreatedById",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Companies_CompanyId",
                schema: "dbo",
                table: "Products",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SystemUser_CreatedById",
                schema: "dbo",
                table: "Products",
                column: "CreatedById",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_SystemUser_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "dbo",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_SystemUser_UserId",
                schema: "dbo",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SystemUser_CreatedById",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Companies_CompanyId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SystemUser_CreatedById",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_SystemUser_UserId",
                schema: "dbo",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_SystemUser_UserId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserFullName",
                schema: "dbo",
                table: "SystemUser",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(350)",
                oldMaxLength: 350);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoiceId",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                schema: "dbo",
                table: "Invoices",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SystemUser_CreatedById",
                schema: "dbo",
                table: "Invoices",
                column: "CreatedById",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Companies_CompanyId",
                schema: "dbo",
                table: "Products",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SystemUser_CreatedById",
                schema: "dbo",
                table: "Products",
                column: "CreatedById",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_SystemUser_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "dbo",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_SystemUser_UserId",
                schema: "dbo",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
