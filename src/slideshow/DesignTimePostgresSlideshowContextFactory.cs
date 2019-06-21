using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using slideshow.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow
{

    public class DesignTimePostgresSlideshowContextFactory : IDesignTimeDbContextFactory<PostgresSlideshowContext>
    {
        public DbContextOptions<PostgresSlideshowContext> GetDefaultOptions()
        {

            var connectionString = PostgresSlideshowContext.GetConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<PostgresSlideshowContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return optionsBuilder.Options;

        }

        public PostgresSlideshowContext CreateDbContext(string[] args)
        {
            var options = GetDefaultOptions();
            return new PostgresSlideshowContext(options);
        }
    }

}
