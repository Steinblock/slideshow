using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace slideshow.db.postgres.Migrations
{
    public partial class AddCacheEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CacheEntries",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    AbsoluteExpiration = table.Column<DateTimeOffset>(nullable: true),
                    AbsoluteExpirationRelativeToNow = table.Column<TimeSpan>(nullable: true),
                    SlidingExpiration = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CacheEntries", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CacheEntries");
        }
    }
}
