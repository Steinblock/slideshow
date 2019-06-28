using Newtonsoft.Json.Linq;
using slideshow.core;
using System;
using System.Collections.Generic;
using System.Net;

namespace slideshow
{
    public class UnleashFeatureToggleProvider : IFeatureToggleProvider
    {
        //private readonly DefaultUnleash unleash;
        private readonly string appName;
        private readonly string instanceId;
        private readonly string apiUrl;
        private DateTime lastUpdate = DateTime.MinValue;
        private readonly IDictionary<string, bool> features = new Dictionary<string, bool>();

        public UnleashFeatureToggleProvider(string apiUrl, string instanceId)
        {
            var environment = "production";
            if (Environment.GetEnvironmentVariable("KUBERNETES_PORT") != null)
            {
                // TODO: pass enviroment via envvar to container
                environment = Environment.GetEnvironmentVariable("HOSTNAME").Split('-')[0];
            };
            this.appName = environment;
            this.instanceId = "ybCTn4f-1Qun4oz4sHcz";
            this.apiUrl = "https://gitlab.com/api/v4/feature_flags/unleash/12678898/features";

        }

        public bool IsEnabled(string feature)
        {
            // TODO: Wert laden und im Cache speichern
            // nach Zeitraum X invalidieren

            if (lastUpdate < DateTime.Now.AddMinutes(-1))
            {
                GetFeatures();
            }

            lock (features)
            {
                return features.ContainsKey(feature) && features[feature];
            }
   
        }

        private void GetFeatures()
        {
            lock (features)
            {
                if (lastUpdate < DateTime.Now.AddMinutes(-1))
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.Headers.Add("UNLEASH-APPNAME", appName);
                            client.Headers.Add("UNLEASH-INSTANCEID", instanceId);

                            var result = client.DownloadString(this.apiUrl);
                            dynamic json = JObject.Parse(result);

                            features.Clear();

                            foreach (var feature in json.features)
                            {
                                features.Add((string)feature.name, (bool)feature.enabled);
                            }

                        }
                    }
                    catch (Exception)
                    {
                        // TODO: logging
                    }
        
                    lastUpdate = DateTime.Now;

                }
            }
        }
    }
}
