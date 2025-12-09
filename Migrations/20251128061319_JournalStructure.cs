using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class JournalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FundId",
                table: "Jevs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ChartofAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasSubsidiary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartofAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubsidiaryAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    SubsidiaryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubsidiaryCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubsidiaryAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubsidiaryAccounts_ChartofAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartofAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JevEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JevId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    SubsidiaryId = table.Column<int>(type: "int", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JevEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JevEntries_ChartofAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartofAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JevEntries_Jevs_JevId",
                        column: x => x.JevId,
                        principalTable: "Jevs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JevEntries_SubsidiaryAccounts_SubsidiaryId",
                        column: x => x.SubsidiaryId,
                        principalTable: "SubsidiaryAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JevEntries_AccountId",
                table: "JevEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JevEntries_JevId",
                table: "JevEntries",
                column: "JevId");

            migrationBuilder.CreateIndex(
                name: "IX_JevEntries_SubsidiaryId",
                table: "JevEntries",
                column: "SubsidiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubsidiaryAccounts_AccountId",
                table: "SubsidiaryAccounts",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JevEntries");

            migrationBuilder.DropTable(
                name: "SubsidiaryAccounts");

            migrationBuilder.DropTable(
                name: "ChartofAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "FundId",
                table: "Jevs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
