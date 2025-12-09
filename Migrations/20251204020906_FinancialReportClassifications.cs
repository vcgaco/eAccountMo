using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class FinancialReportClassifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialReportClassifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubSection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReportClassifications", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubsidiaryEntries_JevId",
                table: "SubsidiaryEntries",
                column: "JevId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubsidiaryEntries_Jevs_JevId",
                table: "SubsidiaryEntries",
                column: "JevId",
                principalTable: "Jevs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubsidiaryEntries_Jevs_JevId",
                table: "SubsidiaryEntries");

            migrationBuilder.DropTable(
                name: "FinancialReportClassifications");

            migrationBuilder.DropIndex(
                name: "IX_SubsidiaryEntries_JevId",
                table: "SubsidiaryEntries");
        }
    }
}
