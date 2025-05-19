using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceRadar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToOfferUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Offers_Url",
                table: "Offers",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_Url",
                table: "Offers");
        }
    }
}
