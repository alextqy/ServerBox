using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ServerBox.Controllers
{
    public class Base : Controller
    {
        public string IP { set; get; }
        public Models.DbContentCore DbContentCore { set; get; }

        public UserLogic UserLogic;
        public DepartmentLogic DepartmentLogic;
        public FileLogic FileLogic;
        public ConfigLogic ConfigLogic;

        public Base(IHttpContextAccessor HttpContext, DbContentCore DbContent)
        {
            this.IP = HttpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            this.DbContentCore = DbContent;

            this.UserLogic = new UserLogic(this.IP, this.DbContentCore);
            this.DepartmentLogic = new DepartmentLogic(this.IP, this.DbContentCore);
            this.FileLogic = new FileLogic(this.IP, this.DbContentCore);
            this.ConfigLogic = new ConfigLogic(this.IP, this.DbContentCore);
        }
    }
}
