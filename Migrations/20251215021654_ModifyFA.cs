using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyDescription",
                table: "FixedAsset");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualDepreciation",
                table: "FixedAsset",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyDepreciation",
                table: "FixedAsset",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualDepreciation",
                table: "FixedAsset");

            migrationBuilder.DropColumn(
                name: "MonthlyDepreciation",
                table: "FixedAsset");

            migrationBuilder.AddColumn<string>(
                name: "PropertyDescription",
                table: "FixedAsset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
