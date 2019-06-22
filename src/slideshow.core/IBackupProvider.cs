using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.core
{
    public interface IBackupProvider
    {
        string Backup();
        void Restore(string json);
    }
}
