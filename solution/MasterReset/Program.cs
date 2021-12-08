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
            if (Result.ResultStatus) { Console.WriteLine("New Password: 000000"); }
            else { Console.WriteLine("Operation Failed!"); }
        }
    }
}
