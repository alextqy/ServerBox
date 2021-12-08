using Microsoft.EntityFrameworkCore;
using Models;

namespace ConfigHelper
{
    public class DBHelper
    {
        public DbContentCore EnvironmentDbContent { set; get; }

        public DBHelper()
        {
            var OptionsBuilder = new DbContextOptionsBuilder<DbContentCore>();
            OptionsBuilder.UseSqlite(AppSettingsHelper.GetSettings("DataBase"));
            this.EnvironmentDbContent = new DbContentCore(OptionsBuilder.Options);
        }
    }
}
