using ConfigHelper;
using Service;
using System;
using System.Text.Json;

namespace ConfInit
{
    public class Program
    {
        static void Main(string[] args)
        {
            InitSettings(Tools.RootPath() + "appsettings.json");
        }

        /// <summary>
        /// 新建配置文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static void InitSettings(string FilePath)
        {
            Tools.DelFile(FilePath);
            if (!Tools.CreateFile(FilePath))
            {
                Console.WriteLine("Failed to create configuration file !!!");
                return;
            }

            AppSettingsObject SettingsObject = new();
            SettingsObject.URL = "http://*:6000";
            SettingsObject.UDPPort = 6002;
            SettingsObject.DataBase = "Data Source = ../DaoRoom.db;";
            SettingsObject.TokenPeriod = 8;
            JsonSerializerOptions Options = new() { WriteIndented = true, };
            Tools.WriteFile(FilePath, JsonSerializer.Serialize(SettingsObject, Options));
            Console.WriteLine("The configuration file is initialized.");
        }
    }
}
