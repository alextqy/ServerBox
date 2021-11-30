using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class FileModel : Base
    {
        public FileModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.FileEntity Data)
        {
            try
            {
                this.DbContent.FileEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.FileEntity Data = new();
            try
            {
                Data = this.DbContent.FileEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.FileEntity.Remove(Data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Modify(int ID, Entity.FileEntity Data)
        {
            Entity.FileEntity Info = new();
            try
            {
                Info = this.DbContent.FileEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.FileName != null)
            {
                if (Data.FileName != "" && Data.FileName != Info.FileName)
                {
                    Info.FileName = Data.FileName;
                }
            }
            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
            }
            if (Data.Createtime > 0 && Data.Createtime != Info.Createtime)
            {
                Info.Createtime = Data.Createtime;
            }
            if (Data.FileType != null)
            {
                if (Data.FileType != "" && Data.FileType != Info.FileType)
                {
                    Info.FileType = Data.FileType;
                }
            }
            if (Data.State > 0 && Data.State != Info.State)
            {
                Info.State = Data.State;
            }
            if (long.Parse(Data.FileSize) > 0 && Data.FileSize != Info.FileSize)
            {
                Info.FileSize = Data.FileSize;
            }
            if (Data.BlockSize > 0 && Data.BlockSize != Info.BlockSize)
            {
                Info.BlockSize = Data.BlockSize;
            }
            if (Data.UploadBlockSize >= 0 && Data.UploadBlockSize != Info.UploadBlockSize)
            {
                Info.UploadBlockSize = Data.UploadBlockSize;
            }
            if (Data.ServerStoragePath != null)
            {
                if (Data.ServerStoragePath != "" && Data.ServerStoragePath != Info.ServerStoragePath)
                {
                    Info.ServerStoragePath = Data.ServerStoragePath;
                }
            }
            if (Data.UploadPath != null)
            {
                if (Data.UploadPath != "" && Data.UploadPath != Info.UploadPath)
                {
                    Info.UploadPath = Data.UploadPath;
                }
            }
            if (Data.DirID > 0 && Data.DirID != Info.DirID)
            {
                Info.DirID = Data.DirID;
            }
            if (Data.MD5 != null)
            {
                if (Data.MD5 != "" && Data.MD5 != Info.MD5)
                {
                    Info.MD5 = Data.MD5;
                }
            }
        }

        public Entity.FileEntity Find(int ID)
        {
            Entity.FileEntity Data = new();
            try
            {
                Data = this.DbContent.FileEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.FileEntity> Select(Entity.FileSelectParamEntity Data)
        {
            List<Entity.FileEntity> Result = new();
            var List = this.DbContent.FileEntity.Where(p => p.ID > 0);

            if (Data.FileName != null)
            {
                if (Data.FileName != "")
                {
                    List = List.Where(p => p.FileName.Contains(Data.FileName));
                }
            }
            if (Data.UserID > 0)
            {
                List = List.Where(p => p.UserID == Data.UserID);
            }
            if (Data.Createtime > 0)
            {
                List = List.Where(p => p.Createtime == Data.Createtime);
            }
            if (Data.FileType != null)
            {
                if (Data.FileType != "")
                {
                    List = List.Where(p => p.FileType.Contains(Data.FileType));
                }
            }
            if (Data.State > 0)
            {
                List = List.Where(p => p.State == Data.State);
            }
            if (Data.FileSize > 0)
            {
                List = List.Where(p => p.FileSize == Data.FileSize.ToString());
            }
            if (Data.BlockSize > 0)
            {
                List = List.Where(p => p.BlockSize == Data.BlockSize);
            }
            if (Data.UploadBlockSize > 0)
            {
                List = List.Where(p => p.UploadBlockSize == Data.UploadBlockSize);
            }
            if (Data.ServerStoragePath != null)
            {
                if (Data.ServerStoragePath != "")
                {
                    List = List.Where(p => p.ServerStoragePath.Contains(Data.ServerStoragePath));
                }
            }
            if (Data.UploadPath != null)
            {
                if (Data.UploadPath != "")
                {
                    List = List.Where(p => p.UploadPath.Contains(Data.UploadPath));
                }
            }
            if (Data.DirID > 0)
            {
                List = List.Where(p => p.DirID == Data.DirID);
            }
            if (Data.MD5 != null)
            {
                if (Data.MD5 != "")
                {
                    List = List.Where(p => p.MD5.Contains(Data.MD5));
                }
            }
            try
            {
                Result = List.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Result = null;
            }
            return Result;
        }

        public void DeleteByUserID(int UserID)
        {
            var Data = this.DbContent.FileEntity.Where(p => p.UserID == UserID);
            try
            {
                this.DbContent.FileEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Entity.FileEntity FileExist(int DirID, string FileName)
        {
            Entity.FileEntity Data = new();
            try
            {
                Data = this.DbContent.FileEntity.Where(p => p.DirID == DirID && p.FileName == FileName).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }
    }
}
