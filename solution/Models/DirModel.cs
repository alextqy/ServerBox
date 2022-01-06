using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class DirModel : Base
    {
        public DirModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.DirEntity Data)
        {
            try
            {
                this.DbContent.DirEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.DirEntity Data = new();
            try
            {
                Data = this.DbContent.DirEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.DirEntity.Remove(Data);
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

        public void Modify(int ID, Entity.DirEntity Data)
        {
            Entity.DirEntity Info = new();
            try
            {
                Info = this.DbContent.DirEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.DirName != null)
            {
                if (Data.DirName != "" && Data.DirName != Info.DirName)
                {
                    Info.DirName = Data.DirName;
                }
            }
            if (Data.ParentID > 0 && Data.ParentID != Info.ParentID)
            {
                Info.ParentID = Data.ParentID;
            }
            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
            }
            if (Data.Createtime > 0 && Data.Createtime != Info.Createtime)
            {
                Info.Createtime = Data.Createtime;
            }
        }

        public Entity.DirEntity Find(int ID)
        {
            Entity.DirEntity Data = new();
            try
            {
                Data = this.DbContent.DirEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.DirEntity> Select(Entity.DirSelectParamEntity Data)
        {
            List<Entity.DirEntity> Result = new();
            var List = this.DbContent.DirEntity.Where(p => p.ID > 0);
            if (Data.DirName != null)
            {
                if (Data.DirName != "")
                {
                    List = List.Where(p => p.DirName.Contains(Data.DirName));
                }
            }
            if (Data.ParentID > 0)
            {
                List = List.Where(p => p.ParentID == Data.ParentID);
            }
            if (Data.UserID > 0)
            {
                List = List.Where(p => p.UserID == Data.UserID);
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
            var Data = this.DbContent.DirEntity.Where(p => p.UserID == UserID);
            try
            {
                this.DbContent.DirEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Entity.DirEntity RootDir(int UserID)
        {
            Entity.DirEntity Data = new();
            try
            {
                Data = this.DbContent.DirEntity.Where(p => p.ParentID == 0 && p.UserID == UserID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public Entity.DirEntity IsExistDir(string DirName, int ParentID, int UserID)
        {
            Entity.DirEntity Data = new();
            try
            {
                Data = this.DbContent.DirEntity.Where(p => p.ParentID == ParentID && p.UserID == UserID && p.DirName == DirName).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }
    }
}
