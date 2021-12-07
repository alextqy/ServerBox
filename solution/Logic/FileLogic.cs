﻿using Microsoft.AspNetCore.Http;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic
{
    public class FileLogic : Base
    {
        public FileLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Entity.CommonResultEntity CreateDir(string Token, int TokenType, Entity.DirEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.DirName == "")
            {
                this.Result.Memo = "DirName error";
            }
            else if (!Tools.RegCheckPro(Data.DirName))
            {
                this.Result.Memo = "DirName format error";
            }
            else if (Data.ParentID < 0)
            {
                this.Result.Memo = "ParentID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 1))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    if (Data.ParentID == 0)
                    {
                        var UserData = this.UserModel.Find(UserID);
                        if (UserData.ID == 0)
                        {
                            this.Result.Memo = "User info error";
                            return this.Result;
                        }
                        else
                        {
                            var RootDirData = this.DirModel.RootDir(UserData.ID);
                            if (RootDirData.ID == 0)
                            {
                                this.Result.Memo = "Root dir info error";
                                return this.Result;
                            }
                            else
                            {
                                Data.ParentID = RootDirData.ID;
                            }
                        }
                    }

                    var Info = this.DirModel.IsExistDir(Data.DirName, Data.ParentID, UserID);
                    if (Info.ID != 0)
                    {
                        this.Result.Memo = "Data is exist";
                    }
                    else
                    {
                        Entity.DirEntity DirData = new();
                        DirData.DirName = Data.DirName;
                        DirData.ParentID = Data.ParentID;
                        DirData.UserID = UserID;
                        DirData.Createtime = Tools.Time32();
                        try
                        {
                            this.DirModel.Insert(DirData);
                            this.DbContent.SaveChanges();
                            this.Result.ID = DirData.ID;
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Create error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DirInfo(string Token, int TokenType, int ID)
        {
            if (Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    var DirInfo = this.DirModel.Find(ID);
                    if (DirInfo.ID == 0)
                    {
                        this.Result.Memo = "Data is not exist";
                    }
                    else
                    {
                        this.Result.ResultStatus = true;
                        this.Result.Memo = "Success";
                        this.Result.Data = DirInfo;
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteDir(string Token, int TokenType, int ID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID <= 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 4))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DirModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (Info.ParentID == 0)
                    {
                        this.Result.Memo = "Data error";
                    }
                    else
                    {
                        if (Info.UserID != UserID)
                        {
                            if (!this.MasterVerify(UserID))
                            {
                                this.Result.Memo = "Permission denied";
                                return this.Result;
                            }
                        }
                        else
                        {
                            var TA = this.BeginTransaction();
                            var Result = this.DeleteDirInRecursion(ID);
                            if (!Result)
                            {
                                TA.Rollback();
                                this.Result.Memo = "Delete error";
                            }
                            else
                            {
                                TA.Commit();
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public bool DeleteDirInRecursion(int ID)
        {
            // 获取目录下的所有文件列表 并设置为删除状态
            Entity.FileSelectParamEntity FileSelectParam = new();
            FileSelectParam.DirID = ID;
            var FileList = this.FileModel.Select(FileSelectParam);
            if (FileList.Count > 0)
            {
                try
                {
                    for (var i = 0; i < FileList.Count; i++)
                    {
                        FileList[i].DirID = 0;
                        FileList[i].State = 4;
                        this.FileModel.Modify(FileList[i].ID, FileList[i]);
                    }
                    this.DbContent.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            // 子文件夹
            Entity.DirSelectParamEntity SubData = new();
            SubData.ParentID = ID;
            var SubDirList = this.DirModel.Select(SubData);
            var SubState = true;
            if (SubDirList.Count > 0)
            {
                for (var i = 0; i < SubDirList.Count; i++)
                {
                    if (!DeleteDirInRecursion(SubDirList[i].ID))
                    {
                        SubState = false;
                        break;
                    }
                }
            }

            if (!SubState)
            {
                return false;
            }

            try
            {
                this.DirModel.Delete(ID);
                this.DbContent.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Entity.CommonResultEntity ModifyDir(string Token, int TokenType, int ID, Entity.DirEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (Data.DirName == "")
            {
                this.Result.Memo = "DirName error";
            }
            else if (!Tools.RegCheckPro(Data.DirName))
            {
                this.Result.Memo = "DirName format error";
            }
            else if (Data.ParentID <= 0)
            {
                this.Result.Memo = "ParentID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 3))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DirModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (Info.ParentID == 0)
                    {
                        this.Result.Memo = "Data error";
                    }
                    else
                    {
                        if (Info.ParentID > 0)
                        {
                            var ParentInfo = this.DirModel.Find(Info.ParentID);
                            if (ParentInfo.ID == 0)
                            {
                                this.Result.Memo = "Parent info not found";
                                return this.Result;
                            }

                            if (ParentInfo.UserID != Info.UserID)
                            {
                                this.Result.Memo = "Data error";
                                return this.Result;
                            }
                        }

                        if (Info.UserID != UserID)
                        {
                            if (!this.MasterVerify(UserID))
                            {
                                this.Result.Memo = "Permission denied";
                                return this.Result;
                            }
                        }

                        if (Info.DirName != Data.DirName)
                        {
                            var CheckDirInfo = this.DirModel.IsExistDir(Data.DirName, Info.ParentID, UserID);
                            if (CheckDirInfo.ID != 0)
                            {
                                this.Result.Memo = "Data is exist";
                                return this.Result;
                            }
                        }

                        try
                        {
                            Info.DirName = Data.DirName;
                            Info.ParentID = Data.ParentID;
                            this.DirModel.Modify(ID, Info);
                            this.DbContent.SaveChanges();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Modify error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonListResultEntity SelectDir(string Token, int TokenType, Entity.DirSelectParamEntity Data)
        {
            if (Token == "")
            {
                ResultList.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                ResultList.Memo = "TokenType error";
            }
            else if (Data.DirName != "" && !Tools.RegCheckPro(Data.DirName))
            {
                ResultList.Memo = "Dir name error";
            }
            else if (Data.ParentID < 0)
            {
                ResultList.Memo = "ParentID error";
            }
            else if (Data.UserID < 0)
            {
                ResultList.Memo = "UserID";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    ResultList.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 2))
                {
                    ResultList.Memo = "Permission denied";
                }
                else
                {
                    if (Data.UserID > 0 && Data.UserID != UserID)
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            ResultList.Memo = "Permission denied";
                            return ResultList;
                        }
                    }
                    if (Data.ParentID > 0)
                    {
                        var ParentDirInfo = this.DirModel.Find(Data.ParentID);
                        if (ParentDirInfo.UserID != UserID && !this.MasterVerify(UserID))
                        {
                            ResultList.Memo = "Permission denied";
                            return ResultList;
                        }
                    }
                    ResultList.Info = Data.ParentID;
                    if (Data.ParentID == 0)
                    {
                        var RootDir = this.RootPath(UserID);
                        Data.ParentID = RootDir.ID;
                        ResultList.Info = RootDir.ID;
                    }
                    ResultList.ResultStatus = true;
                    ResultList.Memo = "Success";
                    ResultList.DataList = this.DirModel.Select(Data);
                }
            }
            return this.ResultList;
        }

        public Entity.CommonResultEntity CreateDirExtra(string Token, int TokenType, Entity.DirExtraEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.DirID <= 0)
            {
                this.Result.Memo = "DirID error";
            }
            else if (Data.ExtraDesc == "")
            {
                this.Result.Memo = "ExtraDesc error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraDesc))
            {
                this.Result.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraType <= 0)
            {
                this.Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue == "")
            {
                this.Result.Memo = "ExtraValue error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraValue))
            {
                this.Result.Memo = "ExtraValue format error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 1))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var DirInfo = this.DirModel.Find(Data.DirID);
                    if (DirInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (DirInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        Entity.DirExtraEntity ExtraData = new();
                        ExtraData.DirID = Data.DirID;
                        ExtraData.ExtraDesc = Data.ExtraDesc;
                        ExtraData.ExtraType = Data.ExtraType;
                        ExtraData.ExtraValue = Data.ExtraValue;
                        try
                        {
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                            this.DirExtraModel.Insert(ExtraData);
                            this.DbContent.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Create error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteDirExtra(string Token, int TokenType, int ID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 4))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DirExtraModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        var DirInfo = this.DirModel.Find(Info.DirID);
                        if (DirInfo.ID != 0 && DirInfo.UserID != UserID)
                        {
                            if (!this.MasterVerify(UserID))
                            {
                                this.Result.Memo = "Permission denied";
                                return this.Result;
                            }
                        }

                        try
                        {
                            this.DirExtraModel.Delete(ID);
                            this.DbContent.SaveChanges();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Delete error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonListResultEntity SelectDirExtra(string Token, int TokenType, Entity.DirExtraSelectParamEntity Data)
        {
            if (Token == "")
            {
                ResultList.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                ResultList.Memo = "TokenType error";
            }
            else if (Data.DirID <= 0)
            {
                this.ResultList.Memo = "DirID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    ResultList.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 2))
                {
                    ResultList.Memo = "Permission denied";
                }
                else
                {
                    var DirInfo = this.DirModel.Find(Data.DirID);
                    if (DirInfo.ID == 0)
                    {
                        ResultList.Memo = "Data error";
                        return ResultList;
                    }
                    if (DirInfo.UserID != UserID && !this.MasterVerify(UserID))
                    {
                        ResultList.Memo = "Permission denied";
                        return ResultList;
                    }

                    ResultList.ResultStatus = true;
                    ResultList.Memo = "Success";
                    ResultList.DataList = this.DirExtraModel.Select(Data);
                }
            }
            return ResultList;
        }

        public Entity.CommonListResultEntity FileList(string Token, int TokenType, int DirID, int State, int UID)
        {
            if (Token == "")
            {
                ResultList.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                ResultList.Memo = "TokenType error";
            }
            else if (DirID < 0)
            {
                Result.Memo = "DirID error";
            }
            else if (State <= 0)
            {
                Result.Memo = "State error";
            }
            else if (UID < 0)
            {
                Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 2))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    if (UID > 0 && UserID != UID)
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            ResultList.Memo = "Permission denied";
                            return ResultList;
                        }
                    }
                    if (DirID > 0)
                    {
                        var DirInfo = this.DirModel.Find(DirID);
                        if (DirInfo.ID == 0)
                        {
                            ResultList.Memo = "Dir is not exist";
                            return ResultList;
                        }
                        if (DirInfo.UserID != UserID)
                        {
                            if (!this.MasterVerify(UserID))
                            {
                                ResultList.Memo = "Permission denied";
                                return ResultList;
                            }
                        }
                    }

                    try
                    {
                        Entity.FileSelectParamEntity Data = new();
                        Data.DirID = DirID;
                        Data.State = State;
                        Data.UserID = UID;
                        ResultList.DataList = this.FileModel.Select(Data);
                        ResultList.ResultStatus = true;
                        ResultList.Memo = "Success";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ResultList.Memo = "Error";
                    }
                }
            }

            return ResultList;
        }

        public Entity.CommonResultEntity CreateFile(string Token, int TokenType, Entity.FileEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.FileName == "")
            {
                this.Result.Memo = "FileName error";
            }
            //else if (!Tools.RegCheckPro(Data.FileName))
            //{
            //    this.Result.Memo = "FileName format error";
            //}
            //else if (Data.FileType == "")
            //{
            //    this.Result.Memo = "FileType error";
            //}
            else if (Data.FileType != "" && !this.ConfigModel.CheckConfigValue("FileType").Contains(Data.FileType.ToLower()))
            {
                this.Result.Memo = "FileType error";
            }
            else if (Convert.ToInt32(Data.FileSize) <= 0)
            {
                this.Result.Memo = "FileSize error";
            }
            else if (Data.UploadPath == "")
            {
                this.Result.Memo = "UploadPath error";
            }
            else if (Data.DirID < 0)
            {
                this.Result.Memo = "DirID error";
            }
            //else if (Data.MD5 == "")
            //{
            //    this.Result.Memo = "MD5 error";
            //}
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 6))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    if (Data.DirID > 0)
                    {
                        var DirData = this.DirModel.Find(Data.DirID);
                        if (DirData.ID == 0)
                        {
                            this.Result.Memo = "Dir not found";
                            return this.Result;
                        }
                        if (DirData.UserID != UserID)
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                    }
                    else
                    {
                        var UserInfo = this.UserModel.Find(UserID);
                        if (UserInfo.ID == 0)
                        {
                            this.Result.Memo = "User data error";
                            return this.Result;
                        }
                        var RootDirInfo = this.DirModel.RootDir(UserInfo.ID);
                        Data.DirID = RootDirInfo.ID;
                    }

                    //Data.FileName = Data.FileName + "." + Tools.TimeMS().ToString();
                    var FileExistData = this.FileModel.FileExist(Data.DirID, Data.FileName);
                    if (FileExistData.ID > 0)
                    {
                        this.Result.Memo = "File is exist";
                    }
                    else
                    {
                        var UserData = this.UserModel.Find(UserID);
                        if (UserData.ID == 0)
                        {
                            this.Result.Memo = "User data error";
                            return this.Result;
                        }

                        var CreateTimeMS = Tools.TimeMS();
                        var FileDir = Tools.BaseDir() + UserData.Account + "/" + Data.FileName + "." + CreateTimeMS.ToString() + "/";

                        var TA = this.BeginTransaction();

                        Entity.FileEntity FileData = new();
                        FileData.FileName = Data.FileName;
                        FileData.UserID = UserID;
                        FileData.Createtime = CreateTimeMS;
                        FileData.FileType = Data.FileType;
                        FileData.State = 1;
                        FileData.FileSize = Data.FileSize.ToString();
                        var BlockSizeDecimal = Convert.ToDecimal(Convert.ToDouble(Data.FileSize) / Convert.ToDouble(this.ConfigModel.CheckConfigValue("BlockSize")));
                        FileData.BlockSize = (int)Math.Ceiling(BlockSizeDecimal);
                        FileData.UploadBlockSize = 0;
                        FileData.ServerStoragePath = FileDir.Replace("\\", "/");
                        FileData.UploadPath = Data.UploadPath;
                        FileData.DirID = Data.DirID;
                        FileData.MD5 = Data.MD5;

                        try
                        {
                            this.FileModel.Insert(FileData);
                            this.DbContent.SaveChanges();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                            this.Result.ID = FileData.ID;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            TA.Rollback();
                            this.Result.Memo = "Create error";
                        }

                        // 建立文件目录
                        if (!Tools.DirIsExists(FileDir))
                        {
                            if (!Tools.CreateDir(FileDir))
                            {
                                this.Result.Memo = "Create file dir error";
                                TA.Rollback();
                                return this.Result;
                            }
                        }

                        TA.Commit();
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity UploadFileEntity(string Token, int TokenType, int ID, string FileSectionName, IFormFile FileEntity)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (FileSectionName == null)
            {
                this.Result.Memo = "FileSectionName error";
            }
            else if (FileSectionName == "")
            {
                this.Result.Memo = "FileSectionName error";
            }
            else if (FileEntity == null)
            {
                this.Result.Memo = "FileEntity error";
            }
            else if (FileEntity.Length > Tools.StrToInt(this.ConfigModel.CheckConfigValue("BlockSize")))
            {
                this.Result.Memo = "FileEntity size error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 6))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(ID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else if (FileInfo.State != 1)
                    {
                        this.Result.ResultStatus = true;
                        this.Result.Memo = "Upload complete";
                    }
                    else
                    {
                        try
                        {
                            string FilePath = Path.Combine(FileInfo.ServerStoragePath, FileSectionName);
                            using (var Stream = System.IO.File.Create(FilePath))
                            {
                                FileEntity.CopyTo(Stream);
                                Stream.Flush();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Upload error";
                            return this.Result;
                        }

                        try
                        {
                            if (FileInfo.UploadBlockSize == 0)
                            {
                                FileInfo.UploadBlockSize = 1;
                            }
                            else
                            {
                                FileInfo.UploadBlockSize++;
                            }
                            if (FileInfo.UploadBlockSize == FileInfo.BlockSize)
                            {
                                FileInfo.State = 2;
                            }
                            this.FileModel.Modify(FileInfo.ID, FileInfo);
                            this.DbContent.SaveChanges();
                            this.Result.Memo = "Success";
                            this.Result.ResultStatus = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Upload error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.DownloadFileEntity DownloadFileEntity(string Token, int TokenType, int ID, int POS)
        {
            Entity.DownloadFileEntity Result = new();
            if (Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                Result.Memo = "ID error";
            }
            else if (POS <= 0)
            {
                Result.Memo = "POS error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 5))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(ID);
                    if (FileInfo.ID == 0)
                    {
                        Result.Memo = "Data not found";
                    }
                    else if (FileInfo.State == 1 || FileInfo.State == 4)
                    {
                        Result.Memo = "Data state error";
                    }
                    else if (FileInfo.ServerStoragePath == "")
                    {
                        Result.Memo = "Data error";
                    }
                    else
                    {
                        var FileEntityList = Tools.SelectAllFile(FileInfo.ServerStoragePath);
                        if (FileEntityList.Length == 0)
                        {
                            Result.Memo = "File entity not found";
                        }
                        else
                        {
                            var FileEntity = FileEntityList[POS - 1];
                            var FileEntityInfo = Tools.FileInfo(FileEntity);
                            var FileSize = FileEntityInfo.Length;

                            try
                            {
                                using (var FS = File.OpenRead(FileEntity))
                                {
                                    var Data = new byte[FileSize];
                                    FS.Read(Data, 0, Data.Length);
                                    var FileEntityPathList = Tools.Explode("/", FileEntity);
                                    Result.FileEntityName = FileEntityPathList[FileEntityPathList.Length - 1];
                                    Result.Data = Tools.ByteToBase64(Data);
                                    Result.ResultStatus = true;
                                    Result.Memo = "Success";
                                }
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine(e.Message);
                                Result.Memo = "Cannot open file";
                            }
                        }
                    }
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity DeleteFile(string Token, int TokenType, int ID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 4))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(UserID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "User data not found";
                    }
                    else
                    {
                        var FileInfo = this.FileModel.Find(ID);
                        if (FileInfo.ID == 0)
                        {
                            this.Result.Memo = "Data not found";
                        }
                        else if (FileInfo.UserID != UserID)
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            var TA = this.BeginTransaction();
                            try
                            {
                                this.FileModel.Delete(ID);
                                this.DbContent.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                TA.Rollback();
                                this.Result.Memo = "Delete error";
                                return this.Result;
                            }

                            var FileDir = Tools.BaseDir() + UserInfo.Account + "/" + FileInfo.FileName + "." + FileInfo.Createtime;
                            if (Tools.DirIsExists(FileDir))
                            {
                                if (!Tools.DelDir(FileDir, true))
                                {
                                    TA.Rollback();
                                    this.Result.Memo = "Delete error";
                                    return this.Result;
                                }
                            }
                            TA.Commit();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity ModifyFile(string Token, int TokenType, int ID, Entity.FileEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (Data.FileName == null || Data.FileName == "")
            {
                this.Result.Memo = "FileName error";
            }
            //else if (!Tools.RegCheckPro(Data.FileName))
            //{
            //    this.Result.Memo = "FileName format error";
            //}
            else if (Data.State <= 0)
            {
                this.Result.Memo = "State error";
            }
            else if (Convert.ToInt32(Data.FileSize) <= 0)
            {
                this.Result.Memo = "FileSize error";
            }
            else if (Data.BlockSize <= 0)
            {
                this.Result.Memo = "BlockSize error";
            }
            else if (Data.UploadBlockSize < 0)
            {
                this.Result.Memo = "UploadBlockSize error";
            }
            else if (Data.DirID < 0)
            {
                this.Result.Memo = "DirID error";
            }
            else if (Data.MD5 == "")
            {
                this.Result.Memo = "MD5 error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 3))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    if (Data.DirID > 0)
                    {
                        var DirInfo = this.DirModel.Find(Data.DirID);
                        if (DirInfo.ID == 0)
                        {
                            this.Result.Memo = "Dir not found";
                            return this.Result;
                        }
                        if (DirInfo.UserID != UserID)
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                    }

                    var FileInfo = this.FileModel.Find(ID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "File not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    //else if (FileInfo.State != 2)
                    //{
                    //    this.Result.Memo = "File state error";
                    //}
                    else
                    {
                        var UserInfo = this.UserModel.Find(UserID);
                        if (UserInfo.ID == 0)
                        {
                            this.Result.Memo = "User data not found";
                        }
                        else if (FileInfo.State != 2 && Data.FileName != FileInfo.FileName)
                        {
                            this.Result.Memo = "FileName error";
                        }
                        else
                        {
                            var TA = this.BeginTransaction();
                            var OldFileName = FileInfo.FileName;
                            //var CheckNameArr = Tools.Explode(".", Data.FileName);
                            //if (Convert.ToInt32(CheckNameArr[CheckNameArr.Length - 1].Substring(0, CheckNameArr.Length - 3)) != FileInfo.Createtime)
                            //{
                            //    var OldNameArr = Tools.Explode(".", FileInfo.FileName);
                            //    var OldName = Tools.Implode(".", OldNameArr.Take(OldNameArr.Length - 1).ToArray());
                            //    if (OldName != Data.FileName)
                            //    {
                            //        var FileDir = Tools.BaseDir() + UserInfo.Account + "/" + FileInfo.FileName;
                            //        var NewName = Data.FileName + "." + OldNameArr[OldNameArr.Length - 1];
                            //        if (!Tools.DirIsExists(FileDir))
                            //        {
                            //            TA.Rollback();
                            //            this.Result.Memo = "error";
                            //            return this.Result;
                            //        }
                            //        else
                            //        {
                            //            if (!Tools.RenameDir(FileDir, NewName))
                            //            {
                            //                TA.Rollback();
                            //                this.Result.Memo = "error";
                            //                return this.Result;
                            //            }
                            //        }
                            //        FileInfo.FileName = NewName;
                            //    }
                            //}

                            FileInfo.FileName = Data.FileName;
                            FileInfo.State = Data.State;
                            FileInfo.FileSize = Data.FileSize.ToString();
                            FileInfo.BlockSize = Data.BlockSize;
                            FileInfo.UploadBlockSize = Data.UploadBlockSize;
                            FileInfo.DirID = Data.DirID;
                            FileInfo.MD5 = Data.MD5;
                            try
                            {
                                this.FileModel.Modify(ID, FileInfo);
                                this.DbContent.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                TA.Rollback();
                                this.Result.Memo = "error";
                                return this.Result;
                            }

                            if (OldFileName != Data.FileName)
                            {
                                var FileOldDir = Tools.BaseDir() + UserInfo.Account + "/" + OldFileName + "." + FileInfo.Createtime;
                                var FileNewDir = Data.FileName + "." + FileInfo.Createtime;
                                if (!Tools.RenameDir(FileOldDir, FileNewDir))
                                {
                                    TA.Rollback();
                                    this.Result.Memo = "error";
                                    return this.Result;
                                }
                            }

                            TA.Commit();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity MoveFile(string Token, int TokenType, int ID, int DirID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (DirID < 0)
            {
                this.Result.Memo = "DirID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 8))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(ID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "File not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        if (DirID > 0)
                        {
                            var DirInfo = this.DirModel.Find(DirID);
                            if (DirInfo.ID == 0)
                            {
                                this.Result.Memo = "Dir not found";
                                return this.Result;
                            }
                            else if (DirInfo.UserID != UserID)
                            {
                                this.Result.Memo = "Permission denied";
                                return this.Result;
                            }
                            else if (DirInfo.ID == FileInfo.DirID)
                            {
                                this.Result.Memo = "DirID error";
                                return this.Result;
                            }
                            else
                            {
                                FileInfo.DirID = DirInfo.ID;
                            }
                        }
                        else
                        {
                            var UserInfo = this.UserModel.Find(UserID);
                            if (UserInfo.ID == 0)
                            {
                                this.Result.Memo = "User not found";
                                return this.Result;
                            }
                            else
                            {
                                var DirInfo = this.DirModel.RootDir(UserInfo.ID);
                                FileInfo.DirID = DirInfo.ID;
                            }
                        }

                        if (FileInfo.State == 4)
                        {
                            FileInfo.State = 2;
                        }

                        try
                        {
                            this.FileModel.Modify(ID, FileInfo);
                            this.DbContent.SaveChanges();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Move error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CheckFile(string Token, int TokenType, int FileID)
        {
            if (Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                Result.Memo = "FileID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 2))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(FileID);
                    if (FileInfo.ID == 0)
                    {
                        Result.Memo = "File not found";
                    }
                    else
                    {
                        if (FileInfo.UserID != UserID)
                        {
                            var UserInfo = this.UserModel.Find(UserID);
                            if (UserInfo.ID == 0)
                            {
                                Result.Memo = "User data error";
                            }
                            else
                            {
                                // 文件是否已经分享到部门
                                Entity.DepartmentFileSelectParamEntity CheckData = new();
                                CheckData.DepartmentID = UserInfo.DepartmentID;
                                CheckData.FileID = FileInfo.ID;
                                CheckData.UserID = FileInfo.UserID;
                                var CheckShareFile = this.DepartmentFileModel.Select(CheckData);
                                if (CheckShareFile.Count == 0)
                                {
                                    Result.Memo = "Permission denied";
                                    return Result;
                                }
                            }
                        }

                        Result.ResultStatus = true;
                        Result.Memo = "Success";
                        Result.Data = FileInfo;
                    }
                }
            }

            return Result;
        }

        public Entity.CommonResultEntity CreateFileExtra(string Token, int TokenType, Entity.FileExtraEntity Data)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.FileID <= 0)
            {
                this.Result.Memo = "FileID error";
            }
            else if (Data.ExtraDesc == "")
            {
                this.Result.Memo = "ExtraDesc error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraDesc))
            {
                this.Result.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraType < 0)
            {
                this.Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue == "")
            {
                this.Result.Memo = "ExtraValue error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraValue))
            {
                this.Result.Memo = "ExtraValue format error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 1))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(Data.FileID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "File not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        Entity.FileExtraEntity ExtraData = new();
                        ExtraData.FileID = Data.FileID;
                        ExtraData.ExtraDesc = Data.ExtraDesc;
                        ExtraData.ExtraType = Data.ExtraType;
                        ExtraData.ExtraValue = Data.ExtraValue;
                        try
                        {
                            this.FileExtraModel.Insert(ExtraData);
                            this.DbContent.SaveChanges();
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Create error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteFileExtra(string Token, int TokenType, int ID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 4))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(ID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        var ExtraInfo = this.FileExtraModel.Find(ID);
                        if (ExtraInfo.ID == 0)
                        {
                            this.Result.Memo = "Data not found";
                        }
                        else
                        {
                            try
                            {
                                this.FileExtraModel.Delete(ID);
                                this.DbContent.SaveChanges();
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                this.Result.Memo = "Delete error";
                            }
                        }
                    }
                }
            }

            return this.Result;
        }

        public Entity.CommonListResultEntity SelectFileExtra(string Token, int TokenType, Entity.FileExtraSelectParamEntity Data)
        {
            if (Token == "")
            {
                this.ResultList.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.ResultList.Memo = "TokenType error";
            }
            else if (Data.FileID <= 0)
            {
                this.ResultList.Memo = "FileID error";
            }
            else if (Data.ExtraDesc != "" && !Tools.RegCheckPro(Data.ExtraDesc))
            {
                this.ResultList.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraValue != "" && !Tools.RegCheckPro(Data.ExtraValue))
            {
                this.ResultList.Memo = "ExtraValue format error";
            }
            else
            {
                var FileInfo = this.FileModel.Find(Data.FileID);
                if (FileInfo.ID == 0)
                {
                    this.ResultList.Memo = "File not found";
                }
                else
                {
                    this.ResultList.ResultStatus = true;
                    this.ResultList.Memo = "Success";
                    this.ResultList.DataList = this.FileExtraModel.Select(Data);
                }
            }
            return this.ResultList;
        }

        public Entity.CommonResultEntity CopyFile(string Token, int TokenType, int DirID, int FileID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (DirID <= 0)
            {
                this.Result.Memo = "DirID error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "FileID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 7))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var DirInfo = DirModel.Find(DirID);
                    if (DirInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (DirInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        var FileInfo = FileModel.Find(FileID);
                        if (FileInfo.ID == 0)
                        {
                            this.Result.Memo = "Data not found";
                        }
                        else if (FileInfo.UserID != UserID)
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            var CheckFileInfo = this.FileModel.FileExist(DirInfo.ID, FileInfo.FileName);
                            if (CheckFileInfo.ID > 0)
                            {
                                this.Result.Memo = "File is exist";
                            }
                            else
                            {
                                var UserData = this.UserModel.Find(UserID);
                                var CreateTimeMS = Tools.TimeMS();
                                var FileDir = Tools.BaseDir() + UserData.Account + "/" + FileInfo.FileName + "." + CreateTimeMS.ToString() + "/";

                                var TA = this.BeginTransaction();

                                Entity.FileEntity FileData = new();
                                FileData.FileName = FileInfo.FileName;
                                FileData.UserID = FileInfo.UserID;
                                FileData.Createtime = CreateTimeMS;
                                FileData.FileType = FileInfo.FileType;
                                FileData.State = FileInfo.State;
                                FileData.FileSize = FileInfo.FileSize.ToString();
                                FileData.BlockSize = FileInfo.BlockSize;
                                FileData.UploadBlockSize = FileInfo.UploadBlockSize;
                                FileData.ServerStoragePath = FileDir;
                                FileData.UploadPath = FileInfo.UploadPath;
                                FileData.DirID = DirID;
                                FileData.MD5 = FileInfo.MD5;
                                try
                                {
                                    this.FileModel.Insert(FileData);
                                    this.DbContent.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    this.Result.Memo = "Create error";
                                    TA.Rollback();
                                    return this.Result;
                                }

                                // 建立文件目录
                                if (!Tools.DirIsExists(FileDir))
                                {
                                    if (!Tools.CreateDir(FileDir))
                                    {
                                        this.Result.Memo = "Create file dir error";
                                        TA.Rollback();
                                        return this.Result;
                                    }
                                }

                                // 复制文件
                                if (!Tools.DirectoryCopy(FileInfo.ServerStoragePath, FileDir))
                                {
                                    this.Result.Memo = "Create file entity error";
                                    TA.Rollback();
                                    return this.Result;
                                }

                                TA.Commit();
                                this.Result.ID = FileData.ID;
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity FileLockSwitch(string Token, int TokenType, int FileID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "FileID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 2))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(FileID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "File not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        if (FileInfo.State == 1 || FileInfo.State == 4)
                        {
                            this.Result.Memo = "Lock failed";
                        }
                        else
                        {
                            var Switch = 0;
                            if (FileInfo.State == 3)
                            {
                                Switch = 2;
                            }
                            if (FileInfo.State == 2)
                            {
                                Switch = 3;
                            }
                            try
                            {
                                FileInfo.State = Switch;
                                this.FileModel.Modify(FileID, FileInfo);
                                this.DbContent.SaveChanges();
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                this.Result.Memo = "Modify error";
                            }
                        }
                    }
                }
            }

            return this.Result;
        }

        public Entity.CommonListResultEntity FileLockList(string Token, int TokenType)
        {
            if (Token == "")
            {
                this.ResultList.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.ResultList.Memo = "TokenType error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.ResultList.Memo = "Token lost";
                }
                else
                {
                    try
                    {
                        Entity.FileSelectParamEntity Data = new();
                        Data.State = 3;
                        Data.UserID = UserID;
                        this.ResultList.DataList = this.FileModel.Select(Data);
                        this.ResultList.ResultStatus = true;
                        this.ResultList.Memo = "Success";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.ResultList.Memo = "Error";
                    }
                }
            }
            return this.ResultList;
        }

        public Entity.CommonResultEntity FileEntitySyncPrefix(string Token, int TokenType, int FileID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserData = this.UserModel.Find(UserID);
                    if (UserData.ID == 0)
                    {
                        this.Result.Memo = "User not found";
                    }
                    else
                    {
                        var FileInfo = this.FileModel.Find(FileID);
                        if (FileInfo.ID == 0)
                        {
                            this.Result.Memo = "Data not found";
                        }
                        else if (FileInfo.UserID != UserID)
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            var SyncDir = Tools.BaseDir() + UserData.Account + "/" + FileInfo.FileName + "." + FileInfo.Createtime.ToString() + "_Sync";
                            if (Tools.DirIsExists(SyncDir))
                            {
                                if (!Tools.DelDir(SyncDir, true))
                                {
                                    this.Result.Memo = "Sync error";
                                    return this.Result;
                                }
                            }
                            if (Tools.CreateDir(SyncDir))
                            {
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                        }
                    }
                }
            }

            return this.Result;
        }

        public Entity.CommonResultEntity FileEntitySync(string Token, int TokenType, int FileID, string FileSectionName, IFormFile FileEntity)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (FileSectionName == null)
            {
                this.Result.Memo = "FileSectionName error";
            }
            else if (FileSectionName == "")
            {
                this.Result.Memo = "FileSectionName error";
            }
            else if (FileEntity == null)
            {
                this.Result.Memo = "FileEntity error";
            }
            else if (FileEntity.Length > Tools.StrToInt(this.ConfigModel.CheckConfigValue("BlockSize")))
            {
                this.Result.Memo = "FileEntity size error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(FileID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        // 获取同步文件夹
                        var SyncDir = FileInfo.ServerStoragePath.Remove(FileInfo.ServerStoragePath.Length - 1) + "_Sync/";
                        if (!Tools.DirIsExists(SyncDir))
                        {
                            this.Result.Memo = "Sync error";
                        }
                        else
                        {
                            // 上传实体到指定目录
                            try
                            {
                                string FilePath = Path.Combine(SyncDir, FileSectionName);
                                using (var Stream = System.IO.File.Create(FilePath))
                                {
                                    FileEntity.CopyTo(Stream);
                                    Stream.Flush();
                                }
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                this.Result.Memo = "Sync error";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity FileEntitySyncDefer(string Token, int TokenType, int FileID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(FileID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (FileInfo.UserID != UserID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        var SyncDir = FileInfo.ServerStoragePath.Remove(FileInfo.ServerStoragePath.Length - 1) + "_Sync";
                        var OldPath = FileInfo.ServerStoragePath.Remove(FileInfo.ServerStoragePath.Length - 1);
                        var BakName = FileInfo.FileName + "." + FileInfo.Createtime + "_bak";
                        var DoRename1 = Tools.RenameDir(OldPath, BakName);
                        if (!DoRename1)
                        {
                            this.Result.Memo = "dir error";
                        }
                        else
                        {
                            var DoRename2 = Tools.RenameDir(SyncDir, FileInfo.FileName + "." + FileInfo.Createtime);
                            if (DoRename2)
                            {
                                Tools.DelDir(OldPath + "_bak", true); // 删除备份文件夹
                                this.Result.ResultStatus = true;
                                this.Result.Memo = "Success";
                            }
                            else
                            {
                                this.Result.Memo = "Sync error";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity FileEntitySyncFail(string Token, int TokenType, int FileID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var FileInfo = this.FileModel.Find(FileID);
                    if (FileInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        var SyncDir = FileInfo.ServerStoragePath.Remove(FileInfo.ServerStoragePath.Length - 1) + "_Sync/";
                        if (!Tools.DelDir(SyncDir, true))
                        {
                            this.Result.Memo = "Delete error";
                        }
                        else
                        {
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity SendFileToUser(string Token, int TokenType, int FileID, int UID)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (UID <= 0)
            {
                this.Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.PermissionVerify(UserID, 9))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(UID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "User not found";
                    }
                    else
                    {
                        var FileInfo = this.FileModel.Find(FileID);
                        if (FileInfo.ID == 0)
                        {
                            this.Result.Memo = "File not found";
                        }
                        else
                        {
                            if (UserInfo.ID == FileInfo.UserID)
                            {
                                this.Result.Memo = "Sharing failed";
                            }
                            else
                            {
                                var UserRootDir = this.RootPath(UserInfo.ID);
                                if (UserRootDir.ID == 0)
                                {
                                    this.Result.Memo = "Sharing failed";
                                }
                                else
                                {
                                    var CreateTimeMS = Tools.TimeMS();
                                    var FileName = FileInfo.FileName + "_AT" + CreateTimeMS.ToString();
                                    var FileDir = Tools.BaseDir() + UserInfo.Account + "/" + FileName + "." + CreateTimeMS.ToString() + "/";

                                    Entity.FileEntity NewFile = new();
                                    NewFile.FileName = FileName;
                                    NewFile.UserID = UserInfo.ID;
                                    NewFile.Createtime = CreateTimeMS;
                                    NewFile.FileType = FileInfo.FileType;
                                    NewFile.State = 2;
                                    NewFile.FileSize = FileInfo.FileSize;
                                    NewFile.BlockSize = FileInfo.BlockSize;
                                    NewFile.UploadBlockSize = FileInfo.UploadBlockSize;
                                    NewFile.ServerStoragePath = FileDir;
                                    NewFile.UploadPath = "SharedFile";
                                    NewFile.DirID = UserRootDir.ID;
                                    NewFile.MD5 = FileInfo.MD5;

                                    var TA = this.BeginTransaction();

                                    try
                                    {
                                        this.FileModel.Insert(NewFile);
                                        this.DbContent.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        TA.Rollback();
                                        this.Result.Memo = "Sharing failed";
                                        return this.Result;
                                    }

                                    // 建立文件目录
                                    if (Tools.DirIsExists(FileDir))
                                    {
                                        if (!Tools.DelDir(FileDir, true))
                                        {
                                            TA.Rollback();
                                            this.Result.Memo = "Sharing failed";
                                            return this.Result;
                                        }
                                    }
                                    else
                                    {
                                        if (!Tools.CreateDir(FileDir))
                                        {
                                            TA.Rollback();
                                            this.Result.Memo = "Sharing failed";
                                            return this.Result;
                                        }
                                    }

                                    // 复制文件
                                    if (!Tools.DirectoryCopy(FileInfo.ServerStoragePath, FileDir))
                                    {
                                        TA.Rollback();
                                        this.Result.Memo = "Sharing failed";
                                        return this.Result;
                                    }

                                    this.Result.ResultStatus = true;
                                    this.Result.Memo = "Success";
                                    this.Result.ID = NewFile.ID;
                                    TA.Commit();
                                }
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.DownloadFileEntity DownloadDemo(string Token, int TokenType, string LangType)
        {
            Entity.DownloadFileEntity Result = new();
            if (Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (this.TokenVerify(Token, TokenType) == 0)
            {
                Result.Memo = "Token lost";
            }
            else if (LangType == "")
            {
                Result.Memo = "Lang type error";
            }
            else
            {
                var FileEntity = Tools.RootPath() + "ImportDemo_" + LangType.ToLower() + ".xlsx";
                if (!Tools.FileIsExists(FileEntity))
                {
                    Result.Memo = "File not found";
                }
                else
                {
                    var FileEntityInfo = Tools.FileInfo(FileEntity);
                    var FileSize = FileEntityInfo.Length;
                    try
                    {
                        using (var FS = File.OpenRead(FileEntity))
                        {
                            var Data = new byte[FileSize];
                            FS.Read(Data, 0, Data.Length);
                            var FileEntityPathList = Tools.Explode("/", FileEntity);
                            Result.FileEntityName = FileEntityPathList[FileEntityPathList.Length - 1];
                            Result.Data = Tools.ByteToBase64(Data);
                            Result.ResultStatus = true;
                            Result.Memo = "Success";
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        Result.Memo = "Cannot open file";
                    }
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity ImportUser(string Token, int TokenType, IFormFile FileEntity)
        {
            if (Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileEntity == null)
            {
                this.Result.Memo = "FileEntity error";
            }
            else
            {
                var User = this.TokenVerify(Token, TokenType);
                if (User == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.MasterVerify(User))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var UploadPath = Tools.RootPath() + "Upload";
                    if (!Tools.DirIsExists(UploadPath))
                    {
                        if (!Tools.CreateDir(UploadPath))
                        {
                            this.Result.Memo = "Upload error";
                            return this.Result;
                        }
                    }

                    var FileData = UploadPath + "/" + FileEntity.FileName;
                    if (Tools.FileIsExists(FileData))
                    {
                        if (!Tools.DelFile(FileData))
                        {
                            this.Result.Memo = "Upload error";
                            return this.Result;
                        }
                    }

                    try
                    {
                        using (var Stream = System.IO.File.Create(Path.Combine(UploadPath, FileEntity.FileName)))
                        {
                            FileEntity.CopyTo(Stream);
                            Stream.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Upload error";
                        return this.Result;
                    }

                    // 读取文件
                    var ExcelReader = new UserExcelHandler(FileData);
                    var UserList = ExcelReader.Reader();
                    if (!this.AccountStat(UserList.Count))
                    {
                        this.Result.Memo = "The number of accounts has reached the upper limit";
                        return this.Result;
                    }

                    var TA = this.BeginTransaction();
                    foreach (var u in UserList)
                    {
                        if (u.Account == "")
                        {
                            this.Result.Memo = "Account error";
                            return this.Result;
                        }
                        if (!Tools.RegCheck(u.Account))
                        {
                            this.Result.Memo = "Account format error";
                            return this.Result;
                        }
                        if (u.Account.Length < 4)
                        {
                            this.Result.Memo = "Account Length error";
                            return this.Result;
                        }
                        var UserInfo = this.UserModel.FindByAccount(u.Account);
                        if (UserInfo.ID > 0)
                        {
                            this.Result.Memo = "Account is exists";
                            return this.Result;
                        }

                        if (u.Name == "")
                        {
                            this.Result.Memo = "Name error";
                            return this.Result;
                        }
                        else if (u.Name.Length < 2)
                        {
                            this.Result.Memo = "Name Length error";
                            return this.Result;
                        }
                        if (!Tools.RegCheckPro(u.Name))
                        {
                            this.Result.Memo = "Name format error";
                            return this.Result;
                        }

                        if (u.Password == "")
                        {
                            this.Result.Memo = "Password error";
                            return this.Result;
                        }
                        else if (!Tools.RegCheck(u.Password))
                        {
                            this.Result.Memo = "Password format error";
                            return this.Result;
                        }
                        else if (u.Password.Length < 6)
                        {
                            this.Result.Memo = "Password Length error";
                            return this.Result;
                        }

                        var UserObject = new Entity.UserEntity();
                        var Secret = Tools.Random(5);
                        UserObject.Account = u.Account.ToLower();
                        UserObject.Name = u.Name;
                        UserObject.Password = Tools.UserPWD(u.Password, Secret.ToString());
                        UserObject.Secret = Secret;
                        UserObject.Admin = 1;
                        UserObject.Master = 1;
                        UserObject.Createtime = Tools.Time32();

                        try
                        {
                            this.UserModel.Insert(UserObject);
                            this.Result.ResultStatus = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            TA.Rollback();
                            this.Result.Memo = "Create error";
                        }

                        // 新建用户根目录
                        if (!Tools.MKDir(Tools.BaseDir(), UserObject.Account))
                        {
                            TA.Rollback();
                            this.Result.Memo = "";
                            return this.Result;
                        }
                    }
                    this.DbContent.SaveChanges();
                    TA.Commit();

                    Tools.DelFile(FileData);
                    this.Result.ResultStatus = true;
                    this.Result.Memo = "Success";
                }
            }

            return this.Result;
        }

    }
}