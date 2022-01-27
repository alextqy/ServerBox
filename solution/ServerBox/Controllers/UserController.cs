using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace ServerBox.Controllers
{
    public class UserController : Base
    {
        public UserController(IHttpContextAccessor HttpContext, DbContentCore DbContent) : base(HttpContext, DbContent) { }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Password"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Sign/In")]
        public IActionResult SignIn(string Account, string Password, int TokenType)
        {
            Account = Account == null ? "" : Account.Trim().ToLower();
            Password = Password == null ? "" : Password.Trim();
            var Result = this.UserLogic.SignIn(Account, Password, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Sign/Out")]
        public IActionResult SignOut(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.SignOut(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// Token状态获取
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Token/Running/State")]
        public IActionResult TokenRunningState(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.TokenRunningState(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 查询个人信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/Self")]
        public IActionResult CheckSelf(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.CheckSelf(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Password"></param>
        /// <param name="Avatar"></param>
        /// <param name="Wallpaper"></param>
        /// <param name="Admin"></param>
        /// <param name="Status"></param>
        /// <param name="Permission"></param>
        /// <param name="DepartmentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/User/Modify")]
        public IActionResult UserModify(
            string Token,
            int TokenType,
            int ID,
            String Name,
            string Password,
            string Avatar,
            string Wallpaper,
            int Admin,
            int Status,
            string Permission,
            int Master,
            int DepartmentID
            )
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.UserEntity Data = new();
            Data.Name = Name == null ? "" : Name.Trim();
            Data.Password = Password == null ? "" : Password.Trim();
            Data.Avatar = Avatar == null ? "" : Avatar.Trim();
            Data.Wallpaper = Wallpaper == null ? "" : Wallpaper.Trim();
            Data.Admin = Admin;
            Data.Status = Status;
            Data.Permission = Permission == null ? "" : Permission.Trim();
            Data.Master = Master;
            Data.DepartmentID = DepartmentID;
            var Result = this.UserLogic.UserModify(Token, TokenType, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 超级管理员身份验证
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Is/Master")]
        public IActionResult IsMaster(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.IsMaster(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="Account"></param>
        /// <param name="Name"></param>
        /// <param name="Password"></param>
        /// <param name="Avatar"></param>
        /// <param name="Wallpaper"></param>
        /// <param name="Admin"></param>
        /// <param name="Status"></param>
        /// <param name="Permission"></param>
        /// <param name="Master"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/User")]
        public IActionResult CreateUser(
            string Token,
            int TokenType,
            string Account,
            string Name,
            string Password,
            string Avatar,
            string Wallpaper,
            int Admin,
            int Status,
            string Permission,
            int Master,
            int DepartmentID
            )
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.UserEntity Data = new();
            Data.Account = Account == null ? "" : Account.Trim().ToLower();
            Data.Name = Name == null ? "" : Name.Trim();
            Data.Password = Password == null ? "" : Password.Trim();
            Data.Avatar = Avatar == null ? "" : Avatar.Trim();
            Data.Wallpaper = Wallpaper == null ? "" : Wallpaper.Trim();
            Data.Admin = Admin;
            Data.Status = Status;
            Data.Permission = Permission == null ? "" : Permission.Trim();
            Data.Master = Master;
            Data.DepartmentID = DepartmentID;
            var Result = this.UserLogic.CreateUser(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 移除用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Remove/User")]
        public IActionResult RemoveUser(string Token, int TokenType, int UserID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.RemoveUser(Token, TokenType, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/User/Info")]
        public IActionResult UserInfo(string Token, int TokenType, int UserID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.UserInfo(Token, TokenType, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="Account"></param>
        /// <param name="Name"></param>
        /// <param name="State"></param>
        /// <param name="Admin"></param>
        /// <param name="Master"></param>
        /// <param name="DepartmentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/User")]
        public IActionResult SelectUser(string Token, int TokenType, string Account, string Name, int State, int Admin, int Master, int DepartmentID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.UserSelectParamEntity Data = new();
            if (!String.IsNullOrEmpty(Account))
            {
                Data.Account = Account.Trim();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                Data.Name = Name.Trim();
            }
            Data.State = State;
            Data.Admin = Admin;
            Data.Master = Master;
            Data.DepartmentID = DepartmentID;
            var Result = this.UserLogic.SelectUser(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 添加用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="UserID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/User/Extra")]
        public IActionResult CreateUserExtra(string Token, int TokenType, int UserID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.UserExtraEntity Data = new();
            Data.UserID = UserID;
            if (!String.IsNullOrEmpty(ExtraDesc))
            {
                Data.ExtraDesc = ExtraDesc.Trim();
            }
            Data.ExtraType = ExtraType;
            if (!String.IsNullOrEmpty(ExtraValue))
            {
                Data.ExtraValue = ExtraValue.Trim();
            }

            var Result = this.UserLogic.CreateUserExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/User/Extra")]
        public IActionResult DeleteUserExtra(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.DeleteUserExtra(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="UserID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/User/Extra")]
        public IActionResult SelectUserExtra(string Token, int TokenType, int UserID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.UserExtraSelectParamEntity Data = new();
            Data.UserID = UserID;
            if (!String.IsNullOrEmpty(ExtraDesc))
            {
                Data.ExtraDesc = ExtraDesc.Trim();
            }
            Data.ExtraType = ExtraType;
            if (!String.IsNullOrEmpty(ExtraValue))
            {
                Data.ExtraValue = ExtraValue.Trim();
            }
            var Result = this.UserLogic.SelectUserExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 系统日志列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="YMD"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Log")]
        public IActionResult SelectLog(string Token, int TokenType, int YMD = 0)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.SelectLog(Token, TokenType, YMD);
            return Json(Result);
        }

        /// <summary>
        /// 删除系统日志
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Clear/Log")]
        public IActionResult ClearLog(string Token, int TokenType, int YMD)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.ClearLog(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 添加 outer token
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="OuterToken"></param>
        /// <param name="TokenDesc"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Outer/Token")]
        public IActionResult CreateOuterToken(string Token, int TokenType, string OuterToken, string TokenDesc)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.OuterTokenEntity Data = new();
            Data.OuterToken = OuterToken == null ? "" : OuterToken.Trim();
            Data.TokenDesc = TokenDesc == null ? "" : TokenDesc.Trim();
            var Result = this.UserLogic.CreateOuterToken(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 获取outer token信息
        /// </summary>
        /// <param name="OuterToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/Outer/Token")]
        public IActionResult CheckOuterToken(String OuterToken)
        {
            OuterToken = OuterToken == null ? "" : OuterToken.Trim();
            var Result = this.UserLogic.CheckOuterToken(OuterToken);
            return Json(Result);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <param name="ReceiverID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Message")]
        public IActionResult CreateMessage(string Token, int TokenType, string Title, string Content, int ReceiverID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.MessageEntity Data = new();
            Data.Title = Title == null ? "" : Title.Trim();
            Data.Content = Content == null ? "" : Content.Trim();
            Data.ReceiverID = ReceiverID;
            var Result = this.UserLogic.CreateMessage(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 查看信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/Message")]
        public IActionResult CheckMessage(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.CheckMessage(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="MessageType">1 接收到的信息 2 发送的信息</param>
        /// <param name="UserID"></param>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Message/List")]
        public IActionResult MessageList(string Token, int TokenType, int MessageType, int UserID, int State, int StartPoint = 0, int EndPoint = 0)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.MessageList(Token, TokenType, MessageType, UserID, State, StartPoint, EndPoint);
            return Json(Result);
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Message")]
        public IActionResult DeleteMessage(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.DeleteMessage(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改信息状态
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Set/Message")]
        public IActionResult SetMessage(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.SetMessage(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 分享文件到部门
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Share/Files/To/Department")]
        public IActionResult ShareFilesToDepartment(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.ShareFilesToDepartment(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 删除分享的文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Department/File")]
        public IActionResult DeleteDepartmentFile(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.UserLogic.DeleteDepartmentFile(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历部门文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DepartmentID"></param>
        /// <param name="FileID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Department/File")]
        public IActionResult SelectDepartmentFile(string Token, int TokenType, int DepartmentID, int FileID, int UserID)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            Entity.DepartmentFileSelectParamEntity Data = new();
            Data.DepartmentID = DepartmentID;
            Data.FileID = FileID;
            Data.UserID = UserID;
            var Result = this.UserLogic.SelectDepartmentFile(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 下载人员导入Demo
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Download/Demo")]
        public IActionResult DownloadDemo(string Token, int TokenType, string LangType)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var LangTypeData = LangType == null ? "" : LangType.Trim();
            var Result = this.FileLogic.DownloadDemo(Token, TokenType, LangTypeData);
            return Json(Result);
        }

        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Import/User")]
        public IActionResult ImportUser(string Token, int TokenType, IFormFile FileEntity)
        {
            Token = Token == null ? "" : Token.Trim().ToLower();
            var Result = this.FileLogic.ImportUser(Token, TokenType, FileEntity);
            return Json(Result);
        }

        /// <summary>
        /// 重置超级管理员信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/Reset/Master")]
        public IActionResult ResetMaster()
        {
            var Result = this.UserLogic.ResetMaster();
            return Json(Result);
        }
    }
}
