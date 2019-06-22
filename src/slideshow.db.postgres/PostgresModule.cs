using Microsoft.EntityFrameworkCore;
using Ninject.Modules;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace slideshow.db.postgres
{

    public class PostgresModule : NinjectModule
    {

        public PostgresModule()
        {

        }

        public override void Load()
        {

            var connectionString = PostgresSlideshowContext.GetConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<PostgresSlideshowContext>();
            optionsBuilder.UseNpgsql(connectionString);
            var options = optionsBuilder.Options;

            this.Bind<SlideshowContext>().To<PostgresSlideshowContext>().WithConstructorArgument("options", options);

        }

    }

}
