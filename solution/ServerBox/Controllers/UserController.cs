using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using websystem.Models;

namespace websystem.Controllers
{
    public class UserController : Base
    {
        public UserController(IHttpContextAccessor HttpContext, CoreDbContent DbContent) : base(HttpContext, DbContent) { }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Password"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.LoginResultModel), 200)]
        [HttpPost]
        [Route("/Sign/In")]
        public IActionResult SignIn(string Account, string Password, int Type)
        {
            Models.Worker.LoginParamModel Data = new();
            Data.Account = Account == null ? "" : Account.Trim().ToLower();
            Data.Password = Password == null ? "" : Password.Trim();
            Data.Type = Type;
            var Result = this.UserLogic.SignIn(Data);
            return Json(Result);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.LoginResultModel), 200)]
        [HttpPost]
        [Route("/Sign/Out")]
        public IActionResult SignOut(string Token, int Type)
        {
            Models.Worker.LogoutParamModel Data = new();
            if (Token != null)
            {
                Data.Token = Token.Trim();
            }
            Data.Type = Type;
            var Result = this.UserLogic.SignOut(Data);
            return Json(Result);
        }

        /// <summary>
        /// Token状态获取
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("Token/Running/State")]
        public IActionResult TokenRunningState(string Token, int Type)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.TokenRunningState(this.Param);
            return Json(Result);
        }

        /// <summary>
        /// 查询个人信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.UserInfoModel), 200)]
        [HttpPost]
        [Route("/Check/Self")]
        public IActionResult CheckSelf(string Token, int Type)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.CheckSelf(this.Param);
            return Json(Result);
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
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
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/User/Modify")]
        public IActionResult UserModify(
            string Token,
            int Type,
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
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.UserModifyParamModel Data = new();
            if (Name != null)
            {
                Data.Name = Name.Trim();
            }
            if (Password != null)
            {
                Data.Password = Password.Trim();
            }
            if (Avatar != null)
            {
                Data.Avatar = Avatar.Trim();
            }
            if (Wallpaper != null)
            {
                Data.Wallpaper = Wallpaper.Trim();
            }
            Data.Admin = Admin;
            Data.Status = Status;
            if (Permission != null)
            {
                Data.Permission = Permission.Trim();
            }
            Data.Master = Master;
            Data.DepartmentID = DepartmentID;
            var Result = this.UserLogic.UserModify(this.Param, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 超级管理员身份验证
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Is/Master")]
        public IActionResult IsMaster(string Token, int Type)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.IsMaster(this.Param);
            return Json(Result);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
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
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Create/User")]
        public IActionResult CreateUser(
            string Token,
            int Type,
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
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.CreateUserParamModel UserParam = new();
            UserParam.Account = Account == null ? "" : Account.Trim().ToLower();
            UserParam.Name = Name == null ? "" : Name.Trim();
            UserParam.Password = Password == null ? "" : Password.Trim();
            UserParam.Avatar = Avatar == null ? "" : Avatar.Trim();
            UserParam.Wallpaper = Wallpaper == null ? "" : Wallpaper.Trim();
            UserParam.Admin = Admin;
            UserParam.Status = Status;
            UserParam.Permission = Permission == null ? "" : Permission.Trim();
            UserParam.Master = Master;
            UserParam.DepartmentID = DepartmentID;

            var Result = this.UserLogic.CreateUser(this.Param, UserParam);
            return Json(Result);
        }

        /// <summary>
        /// 移除用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Remove/User")]
        public IActionResult RemoveUser(string Token, int Type, int UserID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.RemoveUser(this.Param, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/User/Info")]
        public IActionResult UserInfo(string Token, int Type, int UserID)
        {
            if (Token != null)
            {
                this.Param.Token = Token;
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.UserInfo(this.Param, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="Account"></param>
        /// <param name="Name"></param>
        /// <param name="State"></param>
        /// <param name="Admin"></param>
        /// <param name="Master"></param>
        /// <param name="DepartmentID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.UserSelectResultModel), 200)]
        [HttpPost]
        [Route("/Select/User")]
        public IActionResult SelectUser(string Token, int Type, string Account, string Name, int State, int Admin, int Master, int DepartmentID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.UserSelectParamModel Data = new();

            if (Account != null)
            {
                Data.Account = Account.Trim();
            }
            if (Name != null)
            {
                Data.Name = Name.Trim();
            }
            Data.State = State;
            Data.Admin = Admin;
            Data.Master = Master;
            Data.DepartmentID = DepartmentID;

            var Result = this.UserLogic.SelectUser(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 添加用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="UserID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Create/User/Extra")]
        public IActionResult CreateUserExtra(string Token, int Type, int UserID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.CreateUserExtraParamModel Data = new();
            Data.UserID = UserID;
            if (ExtraDesc != null)
            {
                Data.ExtraDesc = ExtraDesc.Trim();
            }
            Data.ExtraType = ExtraType;
            if (ExtraValue != null)
            {
                Data.ExtraValue = ExtraValue.Trim();
            }

            var Result = this.UserLogic.CreateUserExtra(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Delete/User/Extra")]
        public IActionResult DeleteUserExtra(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.DeleteUserExtra(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历用户扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="UserID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.UserExtraSelectResultModel), 200)]
        [HttpPost]
        [Route("/Select/User/Extra")]
        public IActionResult SelectUserExtra(string Token, int Type, int UserID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.UserExtraSelectParamModel Data = new();
            Data.UserID = UserID;
            if (ExtraDesc != null)
            {
                Data.ExtraDesc = ExtraDesc.Trim();
            }
            Data.ExtraType = ExtraType;
            if (ExtraValue != null)
            {
                Data.ExtraValue = ExtraValue.Trim();
            }
            var Result = this.UserLogic.SelectUserExtra(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 系统日志列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="IP"></param>
        /// <param name="ActionType"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.LogSelectResultModel), 200)]
        [HttpPost]
        [Route("/Select/Log")]
        public IActionResult SelectLog(string Token, int Type, string IP, int ActionType)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.LogSelectParamModel Data = new();
            if (IP != null)
            {
                Data.IP = IP.Trim();
            }
            Data.ActionType = ActionType;
            var Result = this.UserLogic.SelectLog(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除系统日志
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Delete/Log")]
        public IActionResult DeleteLog(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.DeleteLog(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 添加 outer token
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="OuterToken"></param>
        /// <param name="TokenDesc"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Create/Outer/Token")]
        public IActionResult CreateOuterToken(string Token, int Type, string OuterToken, string TokenDesc)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.CreateOuterTokenParamModel Data = new();
            if (OuterToken != null)
            {
                Data.OuterToken = OuterToken.Trim();
            }
            if (TokenDesc != null)
            {
                Data.TokenDesc = TokenDesc.Trim();
            }

            var Result = this.UserLogic.CreateOuterToken(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 获取outer token信息
        /// </summary>
        /// <param name="OuterToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Check/Outer/Token")]
        public IActionResult CheckOuterToken(String OuterToken)
        {
            var OuterTokenData = "";
            if (OuterToken != null)
            {
                OuterTokenData = OuterToken.Trim();
            }
            var Result = this.UserLogic.CheckOuterToken(OuterTokenData);
            return Json(Result);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <param name="ReceiverID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Create/Message")]
        public IActionResult CreateMessage(string Token, int Type, string Title, string Content, int ReceiverID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.CreateMessageParamModel Data = new();
            if (Title != null)
            {
                Data.Title = Title.Trim();
            }
            if (Content != null)
            {
                Data.Content = Content.Trim();
            }
            Data.ReceiverID = ReceiverID;
            var Result = this.UserLogic.CreateMessage(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 查看信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.MessageDataModel), 200)]
        [HttpPost]
        [Route("/Check/Message")]
        public IActionResult CheckMessage(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.CheckMessage(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="MessageType">1 接收到的信息 2 发送的信息</param>
        /// <param name="UserID"></param>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.MessageDataModel), 200)]
        [HttpPost]
        [Route("/Message/List")]
        public IActionResult MessageList(string Token, int Type, int MessageType, int UserID, int State, int StartPoint = 0, int EndPoint = 0)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.MessageList(this.Param, MessageType, UserID, State, StartPoint, EndPoint);
            return Json(Result);
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Delete/Message")]
        public IActionResult DeleteMessage(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.DeleteMessage(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改信息状态
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Set/Message")]
        public IActionResult SetMessage(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.SetMessage(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 分享文件到部门
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Share/Files/To/Department")]
        public IActionResult ShareFilesToDepartment(string Token, int Type, int FileID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.ShareFilesToDepartment(this.Param, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 删除分享的文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Delete/Department/File")]
        public IActionResult DeleteDepartmentFile(string Token, int Type, int ID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;
            var Result = this.UserLogic.DeleteDepartmentFile(this.Param, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历部门文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="DepartmentID"></param>
        /// <param name="FileID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.DepartmentFileSelectResultModel), 200)]
        [HttpPost]
        [Route("/Select/Department/File")]
        public IActionResult SelectDepartmentFile(string Token, int Type, int DepartmentID, int FileID, int UserID)
        {
            if (Token != null)
            {
                this.Param.Token = Token.Trim();
            }
            this.Param.Type = Type;

            Models.Worker.DepartmentFileSelectParamModel Data = new();
            Data.DepartmentID = DepartmentID;
            Data.FileID = FileID;
            Data.UserID = UserID;
            var Result = this.UserLogic.SelectDepartmentFile(this.Param, Data);
            return Json(Result);
        }

        /// <summary>
        /// 下载人员导入Demo
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.DownloadFileEntityModel), 200)]
        [HttpPost]
        [Route("/Download/Demo")]
        public IActionResult DownloadDemo(string Token, int Type, string LangType)
        {
            this.Param.Token = Token == null ? "" : Token.Trim();
            this.Param.Type = Type;
            var LangTypeData = LangType == null ? "" : LangType.Trim();
            var Result = this.FileLogic.DownloadDemo(this.Param, LangTypeData);
            return Json(Result);
        }

        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Type"></param>
        /// <param name="FileEntity"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Models.Worker.CommonResultModel), 200)]
        [HttpPost]
        [Route("/Import/User")]
        public IActionResult ImportUser(string Token, int Type, IFormFile FileEntity)
        {
            this.Param.Token = Token == null ? "" : Token.Trim();
            this.Param.Type = Type;
            var Result = this.FileLogic.ImportUser(this.Param, FileEntity);
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
