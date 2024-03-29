﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ServerBox.Controllers
{
    public class ConfigController : Base
    {
        public ConfigController(IHttpContextAccessor HttpContext, DbContentCore DbContent) : base(HttpContext, DbContent) { }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/Config")]
        public IActionResult CheckConfig(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.ConfigLogic.CheckConfig(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改系统配置
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="ConfigKey"></param>
        /// <param name="ConfigDesc"></param>
        /// <param name="ConfigType"></param>
        /// <param name="ConfigValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Modify/Config")]
        public IActionResult ModifyConfig(string Token, int TokenType, int ID, string ConfigKey, string ConfigDesc, int ConfigType, string ConfigValue)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.ConfigEntity Data = new();
            Data.ConfigKey = ConfigKey;
            Data.ConfigDesc = ConfigDesc;
            Data.ConfigType = ConfigType;
            Data.ConfigValue = ConfigValue;

            var Result = this.ConfigLogic.ModifyConfig(Token, TokenType, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 获取磁盘空间信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Get/HardDisk/Space/Info")]
        public IActionResult GetHardDiskSpaceInfo(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.ConfigLogic.GetHardDiskSpaceInfo(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 获取验签码
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Get/Hardware/Code")]
        public IActionResult GetHardwareCode(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.ConfigLogic.GetHardwareCode(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 产品激活
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="EncryptedCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Product/Activation")]
        public IActionResult ProductActivation(string Token, int TokenType, string EncryptedCode)
        {
            Token = Token == null ? "" : Token.Trim();
            EncryptedCode = EncryptedCode == null ? "" : EncryptedCode.Trim();
            var Result = this.ConfigLogic.ProductActivation(Token, TokenType, EncryptedCode);
            return Json(Result);
        }

        /// <summary>
        /// 系统账号数量统计
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Account/Number/Statistics")]
        public IActionResult AccountNumberStatistics(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.ConfigLogic.AccountNumberStatistics(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/Sys/Log")]
        public IActionResult CheckSysLog(string Token, int TokenType, int TimeStamp)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.ConfigLogic.CheckSysLog(Token, TokenType, TimeStamp);
            return Json(Result);
        }

    }
}
