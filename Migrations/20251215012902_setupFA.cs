using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class setupFA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FixedAssetId",
                table: "JevEntries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JevEntries_FixedAssetId",
                table: "JevEntries",
                column: "FixedAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_JevEntries_FixedAsset_FixedAssetId",
                table: "JevEntries",
                column: "FixedAssetId",
                principalTable: "FixedAsset",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JevEntries_FixedAsset_FixedAssetId",
                table: "JevEntries");

            migrationBuilder.DropIndex(
                name: "IX_JevEntries_FixedAssetId",
                table: "JevEntries");

            migrationBuilder.DropColumn(
                name: "FixedAssetId",
                table: "JevEntries");
        }
    }
}
