using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ConfigHelper
{
    public class AppSettingsHelper : Base
    {
        public AppSettingsHelper() { }

        public static string GetSettings(string key)
        {
            var _Configuration = new ConfigurationBuilder().AddJsonFile(Directory.GetParent("../") + "/" + "appsettings.json", optional: true, reloadOnChange: true).Build();
            return _Configuration[key];
        }

        public static bool WriteSettings(string k, string v)
        {
            var FilePath = Directory.GetParent("../") + "/" + "appsettings.json";
            JObject JsonObject;
            StreamReader JsonFile = new(FilePath);
            JsonTextReader Reader = new(JsonFile);
            JsonObject = (JObject)JToken.ReadFrom(Reader);
            JsonObject[k] = v;
            var Writer = new StreamWriter(FilePath);
            JsonTextWriter Jsonwriter = new(Writer);
            JsonObject.WriteTo(Jsonwriter);
            return false;
        }
    }
}
