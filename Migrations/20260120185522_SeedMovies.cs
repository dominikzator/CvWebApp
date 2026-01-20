using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CvWebApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "Title", "Url" },
                values: new object[,]
                {
                    { 1, "OnModelCreatingDescription1", "OnModelCreatingTitle1", "OnModelCreatingUrl1" },
                    { 2, "OnModelCreatingDescription2", "OnModelCreatingTitle2", "OnModelCreatingUrl2" },
                    { 3, "OnModelCreatingDescription3", "OnModelCreatingTitle3", "OnModelCreatingUrl3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
