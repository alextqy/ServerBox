using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class DepartmentFileModel : Base
    {
        public DepartmentFileModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.DepartmentFileEntity Data)
        {
            try
            {
                this.DbContent.DepartmentFileEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.DepartmentFileEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentFileEntity.Where(p => p.ID == ID).FirstOrDefault();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.DepartmentFileEntity.Remove(Data);
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

        public void Modify(int ID, Entity.DepartmentFileEntity Data)
        {
            Entity.DepartmentFileEntity Info = new();
            try
            {
                Info = this.DbContent.DepartmentFileEntity.Where(p => p.ID == ID).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.DepartmentID > 0 && Data.DepartmentID != Info.DepartmentID)
            {
                Info.DepartmentID = Data.DepartmentID;
            }
            if (Data.FileID > 0 && Data.FileID != Info.FileID)
            {
                Info.FileID = Data.FileID;
            }
            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
            }
        }

        public Entity.DepartmentFileEntity Find(int ID)
        {
            Entity.DepartmentFileEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentFileEntity.Where(p => p.ID == ID).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.DepartmentFileEntity> Select(Entity.DepartmentFileSelectParamEntity Data)
        {
            List<Entity.DepartmentFileEntity> Result = new();
            var List = this.DbContent.DepartmentFileEntity.Where(p => p.ID > 0);
            if (Data.DepartmentID > 0)
            {
                List = List.Where(p => p.DepartmentID == Data.DepartmentID);
            }
            if (Data.FileID > 0)
            {
                List = List.Where(p => p.FileID == Data.FileID);
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
            var Data = this.DbContent.DepartmentFileEntity.Where(p => p.UserID == UserID);
            try
            {
                this.DbContent.DepartmentFileEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DeleteByDepartmentID(int DepartmentID)
        {
            var Data = this.DbContent.DepartmentFileEntity.Where(p => p.DepartmentID == DepartmentID);
            try
            {
                this.DbContent.DepartmentFileEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
