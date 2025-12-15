using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAccount.Migrations
{
    /// <inheritdoc />
    public partial class AddFixAssetsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FixedAsset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubsidiaryAccountId = table.Column<int>(type: "int", nullable: false),
                    ChildCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsefulLife = table.Column<int>(type: "int", nullable: true),
                    AcquisitionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepreciationEffectivity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScrapValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedAsset_SubsidiaryAccounts_SubsidiaryAccountId",
                        column: x => x.SubsidiaryAccountId,
                        principalTable: "SubsidiaryAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FixedAsset_SubsidiaryAccountId",
                table: "FixedAsset",
                column: "SubsidiaryAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FixedAsset");
        }
    }
}
