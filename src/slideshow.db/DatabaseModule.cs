using Microsoft.EntityFrameworkCore;
using Ninject.Modules;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace slideshow.db.sqlite
{

    public class DatabaseModule : NinjectModule
    {

        public override void Load()
        {
            this.Bind<ISectionRepository>().To<SectionRepository>();
            this.Bind<ISlideRepository>().To<SlideRepository>();
            this.Bind<ICacheEntryRepository>().To<CacheEntryRepository>();

        }

    }

}
