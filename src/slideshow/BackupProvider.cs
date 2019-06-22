using Newtonsoft.Json;
using slideshow.core;
using slideshow.core.Repository;
using slideshow.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow
{
    public class BackupProvider : IBackupProvider
    {
        private readonly ISectionRepository repo;

        public BackupProvider(ISectionRepository repo)
        {
            this.repo = repo;
        }

        public string Backup()
        {
            var sections = repo.GetAllSections();
            foreach (var section in sections)
            {
                section.Slides = repo.GetSlides(section).ToList();
            }
            var json = JsonConvert.SerializeObject(sections, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            });
            return json;
        }

        public void Restore(string json)
        {

            var sections = JsonConvert.DeserializeObject<IEnumerable<Section>>(json);

            foreach (var section in repo.GetAllSections())
            {
                repo.DeleteSection(section);
            }

            repo.Save();


            foreach (var section in sections)
            {
                repo.AddSection(section);
            }

            repo.Save();

        }
    }
}
