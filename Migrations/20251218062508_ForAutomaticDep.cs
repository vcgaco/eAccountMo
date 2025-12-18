using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class ForAutomaticDep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccumulatedDepreciation",
                table: "FixedAsset",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccumulatedDepreciation",
                table: "FixedAsset");
        }
    }
}
