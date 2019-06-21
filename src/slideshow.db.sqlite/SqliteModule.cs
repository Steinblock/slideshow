using Microsoft.EntityFrameworkCore;
using Ninject.Modules;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace slideshow.db.sqlite
{

    public class SqliteModule : NinjectModule
    {

        public SqliteModule()
        {

        }

        public override void Load()
        {
            var dbname = "slideshow.db";
            var path = Path.GetFullPath(dbname);
            var optionsBuilder = new DbContextOptionsBuilder<SqliteSlideshowContext>();
            optionsBuilder.UseSqlite($"Data Source=\"{path}\"");
            var options = optionsBuilder.Options;

            this.Bind<SlideshowContext>().To<SqliteSlideshowContext>().WithConstructorArgument("options", options);
        }

    }

}
