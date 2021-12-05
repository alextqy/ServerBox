using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ServerBox.Controllers
{
    public class Base : Controller
    {
        public string IP { set; get; }
        public Models.DbContentCore DbContentCore { set; get; }

        public Logic.UserLogic UserLogic;
        public Logic.DepartmentLogic DepartmentLogic;
        public Logic.FileLogic FileLogic;
        public Logic.ConfigLogic ConfigLogic;

        public Base(IHttpContextAccessor HttpContext, DbContentCore DbContent)
        {
            this.IP = HttpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            this.DbContentCore = DbContent;


        }
    }
}
