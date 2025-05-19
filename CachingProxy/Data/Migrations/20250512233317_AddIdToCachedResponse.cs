using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CachingProxy.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToCachedResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CachedResponses",
                table: "CachedResponses");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CachedResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CachedResponses",
                table: "CachedResponses",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CachedResponses",
                table: "CachedResponses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CachedResponses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CachedResponses",
                table: "CachedResponses",
                column: "CacheKey");
        }
    }
}
