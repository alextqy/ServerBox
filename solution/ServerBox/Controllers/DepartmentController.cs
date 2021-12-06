using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ServerBox.Controllers
{
    public class DepartmentController : Base
    {
        public DepartmentController(IHttpContextAccessor HttpContext, DbContentCore DbContent) : base(HttpContext, DbContent) { }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DepartmentName"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Department")]
        public IActionResult CreateDepartment(string Token, int TokenType, string DepartmentName, int ParentID)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DepartmentEntity Data = new();
            Data.DepartmentName = DepartmentName == null ? "" : DepartmentName.Trim();
            Data.ParentID = ParentID;
            var Result = this.DepartmentLogic.CreateDepartment(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Department")]
        public IActionResult DeleteDepartment(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.DepartmentLogic.DeleteDepartment(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 禁用或启用部门
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Toggle/Department")]
        public IActionResult ToggleDepartment(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.DepartmentLogic.ToggleDepartment(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="DepartmentName"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Modify/Department")]
        public IActionResult ModifyDepartment(string Token, int TokenType, int ID, string DepartmentName, int ParentID)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DepartmentEntity Data = new();
            Data.DepartmentName = DepartmentName == null ? "" : DepartmentName.Trim();
            Data.ParentID = ParentID;
            var Result = this.DepartmentLogic.ModifyDepartment(Token, TokenType, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 部门详情
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Department/Info")]
        public IActionResult DepartmentInfo(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.DepartmentLogic.DepartmentInfo(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 部门列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DepartmentName"></param>
        /// <param name="ParentID"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Department")]
        public IActionResult SelectDepartment(string Token, int TokenType, string DepartmentName, int ParentID, int State)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DepartmentSelectParamEntity Data = new();
            Data.DepartmentName = DepartmentName == null ? "" : DepartmentName.Trim();
            Data.ParentID = ParentID;
            Data.State = State;
            var Result = this.DepartmentLogic.SelectDepartment(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 添加部门扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DepartmentID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Department/Extra")]
        public IActionResult CreateDepartmentExtraModel(string Token, int TokenType, int DepartmentID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DepartmentExtraEntity Data = new();
            Data.DepartmentID = DepartmentID;
            Data.ExtraDesc = ExtraDesc == null ? "" : ExtraDesc.Trim();
            Data.ExtraType = ExtraType;
            Data.ExtraValue = ExtraValue == null ? "" : ExtraValue.Trim();
            var Result = this.DepartmentLogic.CreateDepartmentExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除部门扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Department/Extra")]
        public IActionResult DeleteDepartmentExtra(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.DepartmentLogic.DeleteDepartmentExtra(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 部门扩展信息列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DepartmentID"></param>
        /// <param name="ExtraType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Department/Extra")]
        public IActionResult SelectDepartmentExtra(string Token, int TokenType, int DepartmentID, int ExtraType)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DepartmentExtraSelectParamEntity Data = new();
            Data.DepartmentID = DepartmentID;
            Data.ExtraType = ExtraType;
            var Result = this.DepartmentLogic.SelectDepartmentExtra(Token, TokenType, Data);
            return Json(Result);
        }
    }
}
