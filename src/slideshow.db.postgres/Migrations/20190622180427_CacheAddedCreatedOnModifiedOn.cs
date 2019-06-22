using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace slideshow.db.postgres.Migrations
{
    public partial class CacheAddedCreatedOnModifiedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CacheEntries",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedOn",
                table: "CacheEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CacheEntries");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "CacheEntries");
        }
    }
}
