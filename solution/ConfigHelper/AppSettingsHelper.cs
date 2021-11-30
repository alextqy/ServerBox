using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConfigHelper
{
    public class AppSettingsHelper
    {
        public AppSettingsHelper() { }

        public static string GetSettings(string key)
        {
            var _Configuration = new ConfigurationBuilder().AddJsonFile(Directory.GetParent("../") + "/" + "appsettings.json", optional: true, reloadOnChange: true).Build();
            return _Configuration[key];
        }
    }
}
