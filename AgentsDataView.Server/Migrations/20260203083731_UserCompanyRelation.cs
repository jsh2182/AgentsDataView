using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentsDataView.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserCompanyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemUser_Companies_CompanyId",
                schema: "dbo",
                table: "SystemUser");

            migrationBuilder.RenameIndex(
                name: "IX_SystemUser_CompanyID",
                schema: "dbo",
                table: "SystemUser",
                newName: "IX_SystemUser_CompanyId");

            migrationBuilder.AddColumn<bool>(
                name: "IsApiUser",
                schema: "dbo",
                table: "SystemUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CompanyUserRelations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyUserRelations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyUserRelations_SystemUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsApiUser",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRelations_CompanyId",
                schema: "dbo",
                table: "CompanyUserRelations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRelations_User_Comp",
                schema: "dbo",
                table: "CompanyUserRelations",
                columns: new[] { "UserId", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SystemUser_Companies_CompanyId",
                schema: "dbo",
                table: "SystemUser",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.Sql("INSERT INTO dbo.CompanyUserRelations(UserId, CompanyId) SELECT Id, CompanyId FROM dbo.SystemUser WHERE CompanyId IS NOT NULL");

            migrationBuilder.Sql("UPDATE dbo.SystemUser SET IsApiUser = 1 WHERE UserName LIKE 'Comp_%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemUser_Companies_CompanyId",
                schema: "dbo",
                table: "SystemUser");

            migrationBuilder.DropTable(
                name: "CompanyUserRelations",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "IsApiUser",
                schema: "dbo",
                table: "SystemUser");

            migrationBuilder.RenameIndex(
                name: "IX_SystemUser_CompanyId",
                schema: "dbo",
                table: "SystemUser",
                newName: "IX_SystemUser_CompanyID");

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
    }
}
