using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Entity
{
    public class PublicEntity : Base { }

    public class CommonResultEntity : Base
    {
        [JsonPropertyName("ResultStatus")]
        public bool ResultStatus { get; set; }

        [JsonPropertyName("StatusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("Memo")]
        public string Memo { get; set; }

        [JsonPropertyName("ID")]
        public int ID { get; set; }

        [JsonPropertyName("Data")]
        public dynamic Data { get; set; }

        public CommonResultEntity()
        {
            this.ResultStatus = false;
            this.StatusCode = 200;
            this.Memo = "";
            this.ID = 0;
            this.Data = null;
        }
    }

    public class LoginResultEntity : Base
    {
        [JsonPropertyName("ResultStatus")]
        public bool ResultStatus { get; set; }

        [JsonPropertyName("StatusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("Memo")]
        public string Memo { get; set; }

        [JsonPropertyName("Token")]
        public string Token { get; set; }

        public LoginResultEntity()
        {
            this.ResultStatus = false;
            this.StatusCode = 200;
            this.Memo = "";
            this.Token = "";
        }
    }

    public class DownloadFileEntity : Base
    {
        [JsonPropertyName("FileEntityName")]
        public string FileEntityName { get; set; }

        [JsonPropertyName("Data")]
        public string Data { get; set; }

        public DownloadFileEntity()
        {
            this.FileEntityName = "";
            this.Data = "";
        }
    }

    public class DepartmentExtraSelectParamEntity : Base
    {
        public int DepartmentID { get; set; }
        public int ExtraType { get; set; }

        public DepartmentExtraSelectParamEntity()
        {
            this.DepartmentID = 0;
            this.ExtraType = 0;
        }
    }

    public class DepartmentFileSelectParamEntity : Base
    {
        public int DepartmentID { get; set; }
        public int FileID { get; set; }
        public int UserID { get; set; }

        public DepartmentFileSelectParamEntity()
        {
            this.DepartmentID = 0;
            this.FileID = 0;
            this.UserID = 0;
        }
    }

    public class DepartmentSelectParamEntity
    {
        public string DepartmentName { get; set; }
        public int ParentID { get; set; }
        public int State { get; set; }

        public DepartmentSelectParamEntity()
        {
            this.DepartmentName = "";
            this.ParentID = 0;
            this.State = 0;
        }
    }

    public class DirExtraSelectParamEntity
    {
        public int DirID { get; set; }
        public string ExtraDesc { get; set; }
        public int ExtraType { get; set; }
        public string ExtraValue { get; set; }

        public DirExtraSelectParamEntity()
        {
            this.DirID = 0;
            this.ExtraDesc = "";
            this.ExtraType = 0;
            this.ExtraValue = "";
        }
    }

    public class DirSelectParamEntity
    {
        public string DirName { get; set; }
        public int ParentID { get; set; }
        public int UserID { get; set; }

        public DirSelectParamEntity()
        {
            this.DirName = "";
            this.ParentID = 0;
            this.UserID = 0;
        }
    }

    public class FileExtraSelectParamEntity
    {
        public int FileID { get; set; }
        public string ExtraDesc { get; set; }
        public int ExtraType { get; set; }
        public string ExtraValue { get; set; }

        public FileExtraSelectParamEntity()
        {
            this.FileID = 0;
            this.ExtraDesc = "";
            this.ExtraType = 0;
            this.ExtraValue = "";
        }
    }

    public class FileSelectParamEntity
    {
        public string FileName { get; set; }
        public int ParentID { get; set; }
        public int UserID { get; set; }
        public int Createtime { get; set; }
        public string FileType { get; set; }
        public int State { get; set; }
        public long FileSize { get; set; }
        public int BlockSize { get; set; }
        public int UploadBlockSize { get; set; }
        public string ServerStoragePath { get; set; }
        public string UploadPath { get; set; }
        public int DirID { get; set; }
        public string MD5 { get; set; }

        public FileSelectParamEntity()
        {
            this.FileName = "";
            this.ParentID = 0;
            this.UserID = 0;
            this.Createtime = 0;
            this.FileType = "";
            this.State = 0;
            this.FileSize = 0;
            this.BlockSize = 0;
            this.UploadBlockSize = 0;
            this.ServerStoragePath = "";
            this.UploadPath = "";
            this.DirID = 0;
            this.MD5 = "";
        }
    }

    public class FileTagSelectParamEntity
    {
        public int FileID { get; set; }
        public int TagID { get; set; }

        public FileTagSelectParamEntity()
        {
            this.FileID = 0;
            this.TagID = 0;
        }
    }

    public class MessageSelectParamEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public int State { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }

        public MessageSelectParamEntity()
        {
            this.Title = "";
            this.Content = "";
            this.SenderID = 0;
            this.ReceiverID = 0;
            this.State = 0;
            this.StartPoint = 0;
            this.EndPoint = 0;
        }
    }

    public class OuterTokenSelectParamEntity
    {
        public int UserID { get; set; }
        public string OuterToken { get; set; }
        public string TokenDesc { get; set; }

        public OuterTokenSelectParamEntity()
        {
            this.UserID = 0;
            this.OuterToken = "";
            this.TokenDesc = "";
        }
    }

    public class TagSelectParamEntity
    {
        public int UserID { get; set; }

        public TagSelectParamEntity()
        {
            this.UserID = 0;
        }
    }

    public class UserExtraSelectParamEntity
    {
        public int UserID { get; set; }
        public string ExtraDesc { get; set; }
        public int ExtraType { get; set; }
        public string ExtraValue { get; set; }

        public UserExtraSelectParamEntity()
        {
            this.UserID = 0;
            this.ExtraDesc = "";
            this.ExtraType = 0;
            this.ExtraValue = "";
        }
    }

    public class UserSelectParamEntity
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
        public int Admin { get; set; }
        public int Master { get; set; }
        public int DepartmentID { get; set; }

        public UserSelectParamEntity()
        {
            this.Account = "";
            this.Name = "";
            this.State = 0;
            this.Admin = 0;
            this.Master = 0;
            this.DepartmentID = 0;
        }
    }

    public class CommonImportUserEntity
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public CommonImportUserEntity()
        {
            Account = "";
            Name = "";
            Password = "";
        }
    }
}
