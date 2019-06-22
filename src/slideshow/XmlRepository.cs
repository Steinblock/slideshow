using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Ninject;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace slideshow
{
    // 
    public class XmlRepository : IXmlRepository
    {
        private readonly Func<ICacheEntryRepository> factory;

        public XmlRepository(Func<ICacheEntryRepository> factory)
        {
            this.factory = factory;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            var repo = factory();
            var items = repo.GetAllCacheEntries().Where(x => x.Key.EndsWith(".xml"));
            foreach (var item in items)
            {
                var reader = new StringReader(item.Value);
                yield return XElement.Load(reader);
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!IsSafeFilename(friendlyName))
            {
                string text = Guid.NewGuid().ToString();
                friendlyName = text;
            }
            StoreElementCore(element, friendlyName);
        }

        private void StoreElementCore(XElement element, string filename)
        {
            var repo = factory();
            var entry = repo.GetCacheEntry(filename) ?? repo.CreateCacheEntry(filename + ".xml");
            entry.Value = element.ToString();
            repo.Save();
        }

        private static bool IsSafeFilename(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                return filename.All(delegate (char c)
                {
                    if (c != '-' && c != '_' && ('0' > c || c > '9') && ('A' > c || c > 'Z'))
                    {
                        if ('a' <= c)
                        {
                            return c <= 'z';
                        }
                        return false;
                    }
                    return true;
                });
            }
            return false;
        }
    }
    public static class DataProtectionBuilderExtensions
    {
        public static IDataProtectionBuilder PersistKeysToDb(this IDataProtectionBuilder builder, Func<ICacheEntryRepository> factory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            ServiceCollectionServiceExtensions.AddSingleton<IConfigureOptions<KeyManagementOptions>>(builder.Services, (Func<IServiceProvider, IConfigureOptions<KeyManagementOptions>>)delegate (IServiceProvider services)
            {
                return new ConfigureOptions<KeyManagementOptions>((Action<KeyManagementOptions>)delegate (KeyManagementOptions options)
                {
                    options.XmlRepository = new XmlRepository(factory);
                });
            });
            return builder;
        }
    }

}
