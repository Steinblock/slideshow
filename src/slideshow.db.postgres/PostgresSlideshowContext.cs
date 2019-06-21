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

            // gitlab only exposes the process environment variable
            // DATABASE_URL=postgres://user:pass@host:5432/db
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new ArgumentNullException("DATABASE_URL is not defined");

            // https://stackoverflow.com/a/45916910/98491
            if (Uri.TryCreate(databaseUrl, UriKind.Absolute, out Uri url))
            {
                // TODO: Enable Integrated Security=True; if password is empty (requires db setup)
                // https://www.cafe-encounter.net/p2034/postgres-using-integrated-security-on-windows-on-localhost
                return $"Host={url.Host};Username={url.UserInfo.Split(':')[0]};Password={url.UserInfo.Split(':')[1]};Database={url.LocalPath.Substring(1)};Pooling=true;";
            }

            throw new FormatException("DATABASE_URL is not well formatted");


        }
    }
}
