using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ConfigHelper
{
    public class AppSettingsHelper : Base
    {
        public AppSettingsHelper() { }

        public static bool AppSettingsState()
        {
            return File.Exists(Directory.GetParent("../") + "/" + "appsettings.json");
        }

        public static string GetSettings(string key)
        {
            //var _Configuration = new ConfigurationBuilder().AddJsonFile(Directory.GetParent("../") + "/" + "appsettings.json", optional: true, reloadOnChange: true).Build();
            //return _Configuration[key];

            using StreamReader FilePath = new(Directory.GetParent("../") + "/" + "appsettings.json");
            using JsonTextReader Reader = new(FilePath);
            JObject JsonObject = (JObject)JToken.ReadFrom(Reader);
            Reader.Close();
            return (string)JsonObject[key];
        }

        public static bool WriteSettings(string k, string v)
        {
            var FilePath = Directory.GetParent("../") + "/" + "appsettings.json";
            JObject JsonObject;
            StreamReader JsonFile = new(FilePath);
            JsonTextReader Reader = new(JsonFile);
            JsonObject = (JObject)JToken.ReadFrom(Reader);
            JsonObject[k] = v;
            try
            {
                using StreamWriter Writer = new(FilePath, false);
                JsonTextWriter Jsonwriter = new(Writer);
                JsonObject.WriteTo(Jsonwriter);
                Writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
    }
}
