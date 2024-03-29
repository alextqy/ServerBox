﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ServerBox.Controllers
{
    public class FileController : Base
    {
        public FileController(IHttpContextAccessor HttpContext, DbContentCore DbContent) : base(HttpContext, DbContent) { }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirName"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Dir")]
        public IActionResult CreateDir(string Token, int TokenType, string DirName, int ParentID)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DirEntity Data = new();
            Data.DirName = DirName == null ? "" : DirName.Trim();
            Data.ParentID = ParentID;
            var Result = this.FileLogic.CreateDir(Token, TokenType, Data);

            return Json(Result);
        }

        /// <summary>
        /// 文件夹详情
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Dir/Info")]
        public IActionResult DirInfo(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DirInfo(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Dir")]
        public IActionResult DeleteDir(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DeleteDir(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改文件夹信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="DirName"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Modify/Dir")]
        public IActionResult ModifyDir(string Token, int TokenType, int ID, string DirName, int ParentID)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DirEntity Data = new();
            Data.DirName = DirName == null ? "" : DirName.Trim();
            Data.ParentID = ParentID;
            var Result = this.FileLogic.ModifyDir(Token, TokenType, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 遍历子文件夹
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirName"></param>
        /// <param name="ParentID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Dir")]
        public IActionResult SelectDir(string Token, int TokenType, string DirName, int ParentID, int UserID)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.DirSelectParamEntity Data = new();
            Data.DirName = DirName == null ? "" : DirName.Trim();
            Data.ParentID = ParentID;
            Data.UserID = UserID;
            var Result = this.FileLogic.SelectDir(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 添加文件夹扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Dir/Extra")]
        public IActionResult CreateDirExtra(string Token, int TokenType, int DirID, string ExtraDesc, int ExtraType, string ExtraValue)
        {

            Entity.DirExtraEntity Data = new();
            Data.DirID = DirID;
            Data.ExtraDesc = ExtraDesc == null ? "" : ExtraDesc.Trim();
            Data.ExtraType = ExtraType;
            Data.ExtraValue = ExtraValue == null ? "" : ExtraValue.Trim();
            var Result = this.FileLogic.CreateDirExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除文件夹扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/Dir/Extra")]
        public IActionResult DeleteDirExtra(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DeleteDirExtra(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历文件夹扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/Dir/Extra")]
        public IActionResult SelectDirExtra(string Token, int TokenType, int DirID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Entity.DirExtraSelectParamEntity Data = new();
            Data.DirID = DirID;
            Data.ExtraDesc = ExtraDesc == null ? "" : ExtraDesc.Trim();
            Data.ExtraType = ExtraType;
            Data.ExtraValue = ExtraValue == null ? "" : ExtraValue.Trim();
            var Result = this.FileLogic.SelectDirExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 文件列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirID"></param>
        /// <param name="State"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/List")]
        public IActionResult FileList(string Token, int TokenType, int DirID, int State = 0, int UserID = 0)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileList(Token, TokenType, DirID, State, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileName"></param>
        /// <param name="FileType"></param>
        /// <param name="FileSize"></param>
        /// <param name="UploadPath"></param>
        /// <param name="DirID"></param>
        /// <param name="MD5"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/File")]
        public IActionResult CreateFile(string Token, int TokenType, string FileName, string FileType, long FileSize, string UploadPath, int DirID, string MD5)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.FileEntity Data = new();
            Data.FileName = FileName == null ? "" : FileName.Trim();
            Data.FileType = FileType == null ? "" : FileType.Trim();
            Data.FileSize = FileSize.ToString();
            Data.UploadPath = UploadPath == null ? "" : UploadPath.Trim();
            Data.DirID = DirID;
            Data.MD5 = MD5 == null ? "" : MD5.Trim();
            var Result = this.FileLogic.CreateFile(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 上传文件实体
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="FileEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Upload/File/Entity")]
        public IActionResult UploadFileEntity(string Token, int TokenType, int ID, string FileSectionName, IFormFile FileEntity)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.UploadFileEntity(Token, TokenType, ID, FileSectionName, FileEntity);
            return Json(Result);
        }

        /// <summary>
        /// 下载文件实体
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="POS"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Download/File/Entity")]
        public IActionResult DownloadFileEntity(string Token, int TokenType, int ID, int POS)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DownloadFileEntity(Token, TokenType, ID, POS);
            return Json(Result);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/File")]
        public IActionResult DeleteFile(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DeleteFile(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 修改文件信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="FileName"></param>
        /// <param name="State"></param>
        /// <param name="FileSize"></param>
        /// <param name="BlockSize"></param>
        /// <param name="UploadBlockSize"></param>
        /// <param name="DirID"></param>
        /// <param name="MD5"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Modify/File")]
        public IActionResult ModifyFile(string Token, int TokenType, int ID, string FileName, int State, long FileSize, int BlockSize, int UploadBlockSize, int DirID, string MD5)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.FileEntity Data = new();
            Data.FileName = FileName == null ? "" : FileName.Trim();
            Data.State = State;
            Data.FileSize = FileSize.ToString();
            Data.BlockSize = BlockSize;
            Data.UploadBlockSize = UploadBlockSize;
            Data.DirID = DirID;
            Data.MD5 = MD5 == null ? "" : MD5.Trim();
            var Result = this.FileLogic.ModifyFile(Token, TokenType, ID, Data);
            return Json(Result);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="DirID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Move/File")]
        public IActionResult MoveFile(string Token, int TokenType, int ID, int DirID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.MoveFile(Token, TokenType, ID, DirID);
            return Json(Result);
        }

        /// <summary>
        /// 查看文件详情
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Check/File")]
        public IActionResult CheckFile(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.CheckFile(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 添加文件扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/File/Extra")]
        public IActionResult CreateFileExtra(string Token, int TokenType, int FileID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.FileExtraEntity Data = new();
            Data.FileID = FileID;
            Data.ExtraDesc = ExtraDesc == null ? "" : ExtraDesc.Trim();
            Data.ExtraType = ExtraType;
            Data.ExtraValue = ExtraValue == null ? "" : ExtraValue.Trim();
            var Result = this.FileLogic.CreateFileExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 删除文件扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Delete/File/Extra")]
        public IActionResult DeleteFileExtra(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DeleteFileExtra(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 遍历文件扩展信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <param name="ExtraDesc"></param>
        /// <param name="ExtraType"></param>
        /// <param name="ExtraValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Select/File/Extra")]
        public IActionResult SelectFileExtra(string Token, int TokenType, int FileID, string ExtraDesc, int ExtraType, string ExtraValue)
        {
            Token = Token == null ? "" : Token.Trim();
            Entity.FileExtraSelectParamEntity Data = new();
            Data.FileID = FileID;
            Data.ExtraDesc = ExtraDesc.Trim();
            Data.ExtraType = ExtraType;
            Data.ExtraValue = ExtraValue.Trim();
            var Result = this.FileLogic.SelectFileExtra(Token, TokenType, Data);
            return Json(Result);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="DirID"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Copy/File")]
        public IActionResult CopyFile(string Token, int TokenType, int DirID, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.CopyFile(Token, TokenType, DirID, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 文件锁定开关
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Lock/Switch")]
        public IActionResult FileLockSwitch(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileLockSwitch(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 已锁定的文件列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Lock/List")]
        public IActionResult FileLockList(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileLockList(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 文件同步前置操作
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [Route("/File/Entity/Sync/Prefix")]
        public IActionResult FileEntitySyncPrefix(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileEntitySyncPrefix(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 文件同步
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <param name="FileSectionName"></param>
        /// <param name="FileEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Entity/Sync")]
        public IActionResult FileEntitySync(string Token, int TokenType, int FileID, string FileSectionName, IFormFile FileEntity)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileEntitySync(Token, TokenType, FileID, FileSectionName, FileEntity);
            return Json(Result);
        }

        /// <summary>
        /// 同步后置操作
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Entity/Sync/Defer")]
        public IActionResult FileEntitySyncDefer(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileEntitySyncDefer(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 同步失败后的操作
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Entity/Sync/Fail")]
        public IActionResult FileEntitySyncFail(string Token, int TokenType, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileEntitySyncFail(Token, TokenType, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 发送文件给用户
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="FileID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Send/File/To/User")]
        public IActionResult SendFileToUser(string Token, int TokenType, int FileID, int UserID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.SendFileToUser(Token, TokenType, FileID, UserID);
            return Json(Result);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="TagName"></param>
        /// <param name="TagMemo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Tag")]
        public IActionResult CreateTag(string Token, int TokenType, string TagName, string TagMemo)
        {
            Token = Token == null ? "" : Token.Trim();
            TagName = TagName == null ? "" : TagName.Trim();
            TagMemo = TagMemo == null ? "" : TagMemo.Trim();
            var Result = this.FileLogic.CreateTag(Token, TokenType, TagName, TagMemo);
            return Json(Result);
        }

        /// <summary>
        /// 修改标签
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="TagName"></param>
        /// <param name="TagMemo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Modify/Tag")]
        public IActionResult ModifyTag(string Token, int TokenType, int ID, string TagName, string TagMemo)
        {
            Token = Token == null ? "" : Token.Trim();
            TagName = TagName == null ? "" : TagName.Trim();
            TagMemo = TagMemo == null ? "" : TagMemo.Trim();
            var Result = this.FileLogic.ModifyTag(Token, TokenType, ID, TagName, TagMemo);
            return Json(Result);
        }

        /// <summary>
        /// 标签信息
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Tag/Info")]
        public IActionResult TagInfo(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.TagInfo(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 标签列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Tag/List")]
        public IActionResult TagList(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.TagList(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 标签重命名
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <param name="TagName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Tag/Rename")]
        public IActionResult TagRename(string Token, int TokenType, int ID, string TagName)
        {
            Token = Token == null ? "" : Token.Trim();
            TagName = TagName == null ? "" : TagName.Trim();
            var Result = this.FileLogic.TagRename(Token, TokenType, ID, TagName);
            return Json(Result);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Del/Tag")]
        public IActionResult DelTag(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DelTag(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 添加文件标签
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="TagID"></param>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/File/Tag")]
        public IActionResult CreateFileTag(string Token, int TokenType, int TagID, int FileID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.CreateFileTag(Token, TokenType, TagID, FileID);
            return Json(Result);
        }

        /// <summary>
        /// 文件标签列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="TagID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/File/Tag/List")]
        public IActionResult FileTagList(string Token, int TokenType, int TagID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.FileTagList(Token, TokenType, TagID);
            return Json(Result);
        }

        /// <summary>
        /// 删除文件标签
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Del/File/Tag")]
        public IActionResult DelFileTag(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DelFileTag(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 添加离线任务
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="URL"></param>
        /// <param name="TaskMemo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Create/Offline/Task")]
        public IActionResult CreateOfflineTask(string Token, int TokenType, string URL, string TaskMemo = "")
        {
            Token = Token == null ? "" : Token.Trim();
            URL = URL == null ? "" : URL.Trim();
            TaskMemo = TaskMemo == null ? "" : TaskMemo.Trim();
            var Result = this.FileLogic.CreateOfflineTask(Token, TokenType, URL, TaskMemo);
            return Json(Result);
        }

        /// <summary>
        /// 离线任务列表
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Offline/Task/List")]
        public IActionResult OfflineTaskList(string Token, int TokenType)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.OfflineTaskList(Token, TokenType);
            return Json(Result);
        }

        /// <summary>
        /// 删除离线任务
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Del/Offline/Task")]
        public IActionResult DelOfflineTask(string Token, int TokenType, int ID)
        {
            Token = Token == null ? "" : Token.Trim();
            var Result = this.FileLogic.DelOfflineTask(Token, TokenType, ID);
            return Json(Result);
        }

        /// <summary>
        /// 设置离线任务状态 1未处理 2处理中 3处理完成 4任务失败
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Set/Offline/Task/State")]
        public IActionResult SetOfflineTaskState(int ID, int State)
        {
            var Result = this.FileLogic.SetOfflineTaskState(ID, State);
            return Json(Result);
        }
    }
}
