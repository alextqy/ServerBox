using Microsoft.EntityFrameworkCore.Storage;
using Models;
using Npoi.Mapper;
using Service;
using System;
using System.Collections.Generic;

namespace Logic
{
    public class Base
    {
        public string IP { set; get; }
        public DbContentCore DbContent { set; get; }
        public Tools Tools { set; get; }
        public Entity.CommonResultEntity Result { set; get; }
        public Entity.CommonListResultEntity ResultList { set; get; }

        public Models.ConfigModel ConfigModel { set; get; }
        public Models.DepartmentExtraModel DepartmentExtraModel { set; get; }
        public Models.DepartmentFileModel DepartmentFileModel { set; get; }
        public Models.DepartmentModel DepartmentModel { set; get; }
        public Models.DirExtraModel DirExtraModel { set; get; }
        public Models.DirModel DirModel { set; get; }
        public Models.FileExtraModel FileExtraModel { set; get; }
        public Models.FileModel FileModel { set; get; }
        public Models.FileTagModel FileTagModel { set; get; }
        public Models.MessageModel MessageModel { set; get; }
        public Models.OuterTokenModel OuterTokenModel { set; get; }
        public Models.TagModel TagModel { set; get; }
        public Models.TokenModel TokenModel { set; get; }
        public Models.UserExtraModel UserExtraModel { set; get; }
        public Models.UserModel UserModel { set; get; }

        public Base() { this.Tools = new Service.Tools(); }

        public Base(string IP, DbContentCore DbContent)
        {
            this.IP = IP;
            this.DbContent = DbContent;
            this.Tools = new Service.Tools();
            this.Result = new Entity.CommonResultEntity();
            this.ResultList = new Entity.CommonListResultEntity();

            this.ConfigModel = new Models.ConfigModel(this.DbContent);
            this.DepartmentExtraModel = new Models.DepartmentExtraModel(this.DbContent);
            this.DepartmentFileModel = new Models.DepartmentFileModel(this.DbContent);
            this.DepartmentModel = new Models.DepartmentModel(this.DbContent);
            this.DirExtraModel = new Models.DirExtraModel(this.DbContent);
            this.DirModel = new Models.DirModel(this.DbContent);
            this.FileExtraModel = new Models.FileExtraModel(this.DbContent);
            this.FileModel = new Models.FileModel(this.DbContent);
            this.FileTagModel = new Models.FileTagModel(this.DbContent);
            this.MessageModel = new Models.MessageModel(this.DbContent);
            this.OuterTokenModel = new Models.OuterTokenModel(this.DbContent);
            this.TagModel = new Models.TagModel(this.DbContent);
            this.TokenModel = new Models.TokenModel(this.DbContent);
            this.UserExtraModel = new Models.UserExtraModel(this.DbContent);
            this.UserModel = new Models.UserModel(this.DbContent);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return this.DbContent.Database.BeginTransaction();
        }

        public Entity.DirEntity RootPath(int UserID)
        {
            Entity.DirEntity DirInfo = new();
            var UserInfo = this.UserModel.Find(UserID);
            if (UserInfo.ID != 0)
            {
                DirInfo = this.DirModel.RootDir(UserID);
            }
            return DirInfo;
        }

        // 账号统计
        public bool AccountStat(int UserIncrement)
        {
            var CountUser = this.UserModel.CountUser() + UserIncrement; // 验证用户数
            var ActivationCode = ConfigHelper.AppSettingsHelper.GetSettings("ActivationCode");
            if (ActivationCode != "")
            {
                // 获取操作系统类型和机器码
                var OSType = Tools.OSType();
                string Motherboard;
                if (OSType == "Linux")
                {
                    Motherboard = Tools.SysShell("dmidecode", "-s system-uuid").Trim();
                }
                else if (OSType == "Windows")
                {
                    Motherboard = Tools.SysShell("wmic", "csproduct get UUID").Replace("UUID", "").Trim();
                }
                else
                {
                    Motherboard = "";
                }

                if (Motherboard != "")
                {
                    var DeCode = Tools.AES_Decrypt(ActivationCode, 3); // 解密
                    var DeCodeArr = Tools.Explode("_", DeCode);
                    var HardwareCode = DeCodeArr[1];
                    if (HardwareCode != Motherboard) // 验证机器码
                    {
                        ConfigHelper.AppSettingsHelper.WriteSettings("ActivationCode", ""); // 清空当前激活码
                        return false;
                    }
                    var UserLimit = Tools.StrToInt32(DeCodeArr[2]) + 5;
                    if (CountUser >= UserLimit)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (CountUser > 5)
                {
                    return false;
                }
            }
            return true;
        }

        public int TokenVerify(string Token, int TokenType)
        {
            if (Token == null)
            {
                return 0;
            }
            if (Token == "")
            {
                return 0;
            }
            else if (TokenType <= 0)
            {
                return 0;
            }
            else
            {
                try
                {
                    var TokenInfo = this.TokenModel.FindByToken(Token, TokenType);
                    if (TokenInfo.ID > 0)
                    {
                        var LifeCycle = Tools.Time() - TokenInfo.Createtime;
                        if ((LifeCycle / 3600) >= Convert.ToInt32(ConfigHelper.AppSettingsHelper.GetSettings("TokenPeriod")))
                        {
                            this.TokenModel.Delete(TokenInfo.ID);
                            DbContent.SaveChanges();
                            return 0;
                        }
                        else
                        {
                            return TokenInfo.UserID;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return 0;
                }
            }
        }

        // 个人权限(逗号分隔) 1新建 2读取 3修改 4删除 5下载 6上传 7复制 8移动 9分享
        public bool PermissionVerify(int UserID, int PermissionType)
        {
            if (UserID <= 0)
            {
                return false;
            }
            else
            {
                var Info = this.UserModel.Find(UserID);
                if (Info.ID == 0)
                {
                    return false;
                }
                else
                {
                    if (Info.Permission.Contains(PermissionType.ToString()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool MasterVerify(int UserID)
        {
            if (UserID <= 0)
            {
                return false;
            }
            else
            {
                var Info = this.UserModel.Find(UserID);
                if (Info.Master != 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool AdminVerify(int UserID)
        {
            if (UserID <= 0)
            {
                return false;
            }
            else
            {
                var Info = this.UserModel.Find(UserID);
                if (Info.Admin != 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void WTL(string IP, string Content, int ActionType)
        {
            // Models.Entity.LogModel Data = new();
            // Data.IP = IP;
            // Data.ActionType = ActionType;
            // Data.ActionTime = Convert.ToInt32(Tools.Time());
            // Data.ActionDesc = Content;
            // this.Insert(Data);
        }
    }

    public class UserExcelHandler
    {
        string FilePath { get; set; }
        int StartRow { get; set; }

        public UserExcelHandler(string FilePath, int StartRow = 0)
        {
            this.FilePath = FilePath;
            this.StartRow = StartRow;
        }

        public List<Entity.CommonImportUserEntity> Reader()
        {
            var Handler = new Mapper(this.FilePath) { FirstRowIndex = StartRow };
            var UserList = new List<Entity.CommonImportUserEntity>();
            var ObjectRows = Handler.Take<dynamic>("sheet1");
            foreach (var Rows in ObjectRows)
            {
                string Row = Rows.Value.ToString().Replace("{", "").Replace("}", "").Replace(" ", "").Replace("=", " ").Trim();
                var RowList = Row.Split(",");
                var Obj = new Entity.CommonImportUserEntity
                {
                    Account = RowList[0].Split(" ")[1],
                    Name = RowList[1].Split(" ")[1],
                    Password = RowList[2].Split(" ")[1],
                };
                UserList.Add(Obj);
            }
            return UserList;
        }
    }
}
