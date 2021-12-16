using ConfigHelper;
using Logic;
using Service;
using System;

namespace MasterReset
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var DB = new DBHelper();
            UserLogic UserLogic = new(Tools.LocalIP(), DB.EnvironmentDbContent);
            var Result = UserLogic.ResetMaster();
            if (Result.State)
            {
                Tools.CorrectConsole("New Password: 000000");
                Console.ReadLine();
            }
            else
            {
                Tools.WarningConsole("Operation Failed!");
                Console.ReadLine();
            }
        }
    }
}
