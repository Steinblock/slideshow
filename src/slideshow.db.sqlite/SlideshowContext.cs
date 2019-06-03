using System;
using Microsoft.EntityFrameworkCore;

namespace slideshow.db
{
    // dotnet ef migrations add InitialCreate --project src/slideshow.db.sqlite --startup-project src/slideshow
    // dotnet ef database update --project src/slideshow.db.sqlite --startup-project src/slideshow

    public class SqliteSlideshowContext : SlideshowContext
    {
        public SqliteSlideshowContext(DbContextOptions options) : base(options)
        {
        }
    }
}
