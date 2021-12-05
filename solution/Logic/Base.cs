using Microsoft.EntityFrameworkCore.Storage;
using Models;
using Service;
using System;

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
    }
}
