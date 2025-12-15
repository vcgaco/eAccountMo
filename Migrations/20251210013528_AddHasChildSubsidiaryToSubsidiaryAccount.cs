using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class AddHasChildSubsidiaryToSubsidiaryAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasChildSubsidiary",
                table: "SubsidiaryAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasChildSubsidiary",
                table: "SubsidiaryAccounts");
        }
    }
}
