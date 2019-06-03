using slideshow.data;
using slideshow.data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.db
{

    public abstract class SlideshowContext : DbContext
    {

        public DbSet<Section> Sections { get; set; }
        public DbSet<Slide> Slides { get; set; }

        public SlideshowContext(DbContextOptions options) : base(options)
        {
          
        }

    }
}
