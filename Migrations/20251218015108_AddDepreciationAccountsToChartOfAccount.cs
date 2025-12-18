using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class AddDepreciationAccountsToChartOfAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "ChartofAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccumulatedDepreciationAccountId",
                table: "ChartofAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepreciationExpenseAccountId",
                table: "ChartofAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartofAccounts_AccumulatedDepreciationAccountId",
                table: "ChartofAccounts",
                column: "AccumulatedDepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartofAccounts_DepreciationExpenseAccountId",
                table: "ChartofAccounts",
                column: "DepreciationExpenseAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartofAccounts_ChartofAccounts_AccumulatedDepreciationAccountId",
                table: "ChartofAccounts",
                column: "AccumulatedDepreciationAccountId",
                principalTable: "ChartofAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartofAccounts_ChartofAccounts_DepreciationExpenseAccountId",
                table: "ChartofAccounts",
                column: "DepreciationExpenseAccountId",
                principalTable: "ChartofAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartofAccounts_ChartofAccounts_AccumulatedDepreciationAccountId",
                table: "ChartofAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartofAccounts_ChartofAccounts_DepreciationExpenseAccountId",
                table: "ChartofAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartofAccounts_AccumulatedDepreciationAccountId",
                table: "ChartofAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartofAccounts_DepreciationExpenseAccountId",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "AccumulatedDepreciationAccountId",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "DepreciationExpenseAccountId",
                table: "ChartofAccounts");
        }
    }
}
