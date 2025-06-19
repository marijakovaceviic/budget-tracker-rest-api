using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finansije.Migrations
{
    /// <inheritdoc />
    public partial class addAddressInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressInfoId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AddressInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNumber = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AddressInfo",
                columns: new[] { "Id", "City", "Country", "HouseNumber", "PostalCode", "Street" },
                values: new object[] { 1, "Belgrade", "Serbia", 73, "104104", "Bulevar Kralja Aleksandra" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "AddressInfoId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressInfoId",
                table: "Users",
                column: "AddressInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AddressInfo_AddressInfoId",
                table: "Users",
                column: "AddressInfoId",
                principalTable: "AddressInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AddressInfo_AddressInfoId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AddressInfo");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressInfoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfoId",
                table: "Users");
        }
    }
}
