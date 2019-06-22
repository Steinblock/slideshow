using System;
using System.Collections.Generic;
using System.Text;

namespace slideshow.core
{
    public interface IFeatureToggleProvider
    {
        bool IsEnabled(string feature);
        // TODO: IsEnabled(string feature, bool defaultValue)
    }
}
