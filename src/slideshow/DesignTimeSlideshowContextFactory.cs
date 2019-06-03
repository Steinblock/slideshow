using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using slideshow.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow
{

    public class DesignTimeSlideshowContextFactory : IDesignTimeDbContextFactory<SqliteSlideshowContext>
    {
        public DbContextOptions<SqliteSlideshowContext> GetDefaultOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteSlideshowContext>();

            optionsBuilder.UseSqlite("Data Source=slideshow.db");

            return optionsBuilder.Options;

        }

        public SqliteSlideshowContext CreateDbContext(string[] args)
        {
            var options = GetDefaultOptions();
            return new SqliteSlideshowContext(options);
        }
    }

}
