using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class Relation_User_Comp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "SystemUser",
                newName: "CompanyId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                schema: "dbo",
                table: "Companies",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemUser_Companies_CompanyId",
                schema: "dbo",
                table: "SystemUser",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemUser_Companies_CompanyId",
                schema: "dbo",
                table: "SystemUser");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "dbo",
                table: "SystemUser",
                newName: "CompanyID");
        }
    }
}
