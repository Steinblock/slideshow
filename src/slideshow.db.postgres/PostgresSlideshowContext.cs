using Microsoft.EntityFrameworkCore;
using System;

namespace slideshow.db
{
    // dotnet ef migrations add InitialCreate --project src/slideshow.db.postgres --startup-project src/slideshow --context PostgresSlideshowContext
    // dotnet ef database update --project src/slideshow.db.postgres --startup-project src/slideshow --context PostgresSlideshowContext

    public class PostgresSlideshowContext : SlideshowContext
    {
        public PostgresSlideshowContext(DbContextOptions options) : base(options)
        {
        }

        public static string GetConnectionString()
        {

            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "slideshow";
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "";

            // TODO: Enable Integrated Security=True; if password is empty (requires db setup)
            // https://www.cafe-encounter.net/p2034/postgres-using-integrated-security-on-windows-on-localhost
            return $"Host={host};Database={database};Username={username};Password={password}";

        }
    }
}
