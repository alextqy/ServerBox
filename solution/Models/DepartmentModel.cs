using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class DepartmentModel : Base
    {
        public DepartmentModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.DepartmentEntity Data)
        {
            try
            {
                this.DbContent.DepartmentEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.DepartmentEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentEntity.Where(p => p.ID == ID).FirstOrDefault();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.DepartmentEntity.Remove(Data);
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

        public void Modify(int ID, Entity.DepartmentEntity Data)
        {
            Entity.DepartmentEntity Info = new();
            try
            {
                Info = this.DbContent.DepartmentEntity.Where(p => p.ID == ID).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.DepartmentName != null)
            {
                if (Data.DepartmentName != "" && Data.DepartmentName != Info.DepartmentName)
                {
                    Info.DepartmentName = Data.DepartmentName;
                }
            }
            if (Data.ParentID > 0 && Data.ParentID != Info.ParentID)
            {
                Info.ParentID = Data.ParentID;
            }
            if (Data.State > 0 && Data.State != Info.State)
            {
                Info.State = Data.State;
            }
            if (Data.Createtime > 0 && Data.Createtime != Info.Createtime)
            {
                Info.Createtime = Data.Createtime;
            }
        }

        public Entity.DepartmentEntity Find(int ID)
        {
            Entity.DepartmentEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentEntity.Where(p => p.ID == ID).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.DepartmentEntity> Select(Entity.DepartmentSelectParamEntity Data)
        {
            List<Entity.DepartmentEntity> Result = new();
            var List = this.DbContent.DepartmentEntity.Where(p => p.ID > 0);

            if (Data.DepartmentName != null)
            {
                if (Data.DepartmentName != "")
                {
                    List = List.Where(p => p.DepartmentName.Contains(Data.DepartmentName.Trim()));
                }
            }
            if (Data.ParentID >= 0)
            {
                List = List.Where(p => p.ParentID == Data.ParentID);
            }
            if (Data.State > 0)
            {
                List = List.Where(p => p.State == Data.State);
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

        public Entity.DepartmentEntity FindByName(string DepartmentName, int ParentID)
        {
            Entity.DepartmentEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentEntity.Where(p => p.ParentID == ParentID && p.DepartmentName == DepartmentName).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }
    }
}
