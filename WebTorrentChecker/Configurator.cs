using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTorrentChecker
{
    class Configurator
    {
        public static string PATH_LASTS = "last.path";
        public static string PATH_NEWS = "news.path";
        public static string PATH_PROVIDERS = "providers.path";

        private Dictionary<string, string> properties;

        private static Configurator instance;

        private Configurator()
        {
            properties = new Dictionary<string, string>();
            properties.Add(PATH_LASTS, "./last.txt");
            properties.Add(PATH_NEWS, "./news.txt");
            properties.Add(PATH_PROVIDERS, "./providers.txt");
        }

        public static Configurator getInstance()
        {
            if(instance == null)
            {
                instance = new Configurator();
            }
            return instance;
        }

        public string GetPropertyValue(string propertyKey)
        {
            string propVal = null;
            if (properties.ContainsKey(propertyKey))
            {
                propVal = properties[propertyKey];
            }
            return propVal;
        }
    }

}
