using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceRadar.Infrastructure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsParserImplemented",
                table: "Stores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StoreCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreCategories_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("2509a6ea-b884-4f49-8b44-af7da9b7812c"),
                column: "IsParserImplemented",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("8d779e8f-f6c5-46da-9eeb-5f749c7988ba"),
                column: "IsParserImplemented",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("99b698b7-ae46-4a2c-af66-d7155497caff"),
                column: "IsParserImplemented",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategories_CategoryId",
                table: "StoreCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategories_StoreId",
                table: "StoreCategories",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreCategories");

            migrationBuilder.DropColumn(
                name: "IsParserImplemented",
                table: "Stores");
        }
    }
}
