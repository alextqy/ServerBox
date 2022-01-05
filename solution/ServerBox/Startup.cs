using CrondTask;
using Init;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace ServerBox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var Init = new SysInit();
            if (!Init.CheckConfigFile()) { Tools.WarningConsole("Press enter to exit."); Console.ReadLine(); Environment.Exit(0); }
            Tools.CorrectConsole("========== Ver 0.0.1 alpha ==========");
            Tools.CorrectConsole("Bit Box is working!");
            CrondTool.RunTask();
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<Models.DbContentCore>(OptionsAction => OptionsAction.UseSqlite(ConfigHelper.AppSettingsHelper.GetSettings("DataBase")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
