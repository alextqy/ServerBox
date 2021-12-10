using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service;
using System;
using System.IO;
using System.Text.Json;

namespace ConfigHelper
{
    public class AppSettingsHelper : Base
    {
        public AppSettingsHelper() { }

        /// <summary>
        /// 初始化状态
        /// </summary>
        /// <returns></returns>
        public static bool InitState() { return Tools.FileIsExists(Tools.RootPath() + "Init.conf"); }

        /// <summary>
        /// 初始化系统完成
        /// </summary>
        /// <returns></returns>
        public static bool SetInit()
        {
            var InitFile = Tools.RootPath() + "Init.conf";
            if (!Tools.FileIsExists(InitFile))
            {
                if (!Tools.CreateFile(InitFile))
                {
                    return false;
                }
            }
            if (!Tools.WriteFile(InitFile, Tools.TimeMS().ToString())) { return false; }
            return true;
        }

        /// <summary>
        /// 配置文件状态
        /// </summary>
        /// <returns></returns>
        public static bool AppSettingsState(string FilePath) { return Tools.FileIsExists(FilePath); }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSettings(string key)
        {
            var _Configuration = new ConfigurationBuilder().AddJsonFile(Tools.RootPath() + "appsettings.json", optional: true, reloadOnChange: true).Build();
            return _Configuration[key];

            //using StreamReader FilePath = new(Tools.RootPath() + "appsettings.json");
            //using JsonTextReader Reader = new(FilePath);
            //JObject JsonObject = (JObject)JToken.ReadFrom(Reader);
            //Reader.Close();
            //return (string)JsonObject[key];
        }

        /// <summary>
        /// 新建配置文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool InitSettings(string FilePath)
        {
            if (!Tools.FileIsExists(FilePath)) { return false; }

            AppSettingsObject SettingsObject = new();
            SettingsObject.URL = "http://*:6000";
            SettingsObject.UDPPort = 6002;
            SettingsObject.DataBase = "Data Source = ../DaoRoom.db;";
            SettingsObject.TokenPeriod = 8;
            JsonSerializerOptions Options = new() { WriteIndented = true, };
            Console.WriteLine("");
            return Tools.WriteFile(FilePath, System.Text.Json.JsonSerializer.Serialize(SettingsObject, Options));
        }

        public static bool WriteSettings(string k, string v)
        {
            var FilePath = Tools.RootPath() + "appsettings.json";
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

        /// <summary>
        /// 激活码
        /// </summary>
        /// <returns></returns>
        public static string ActivationCode()
        {
            var ActivationFile = Directory.GetParent("../") + "/ActivationFile.conf";
            if (!Tools.FileIsExists(ActivationFile)) { return ""; }
            var ActivationCode = Tools.ReadFile(ActivationFile);
            if (String.IsNullOrEmpty(ActivationCode))
            {
                Tools.DelFile(ActivationFile);
                return "";
            }
            else { return ActivationCode; }
        }

        /// <summary>
        /// 产品激活
        /// </summary>
        /// <param name="ActivationCode"></param>
        /// <returns></returns>
        public static bool Activation(string ActivationCode)
        {
            var ActivationFile = Directory.GetParent("../") + "/ActivationFile.conf";
            if (!Tools.CreateFile(ActivationFile)) { return false; }
            if (!Tools.WriteFile(ActivationFile, ActivationCode)) { return false; }
            return true;
        }

        /// <summary>
        /// 取消激活
        /// </summary>
        /// <returns></returns>
        public static bool Unactivation()
        {
            return Tools.DelFile(Directory.GetParent("../") + "/ActivationFile.sys");
        }
    }
}
