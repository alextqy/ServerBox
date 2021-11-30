using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    internal class DepartmentExtraModel : Base
    {
        public DepartmentExtraModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.DepartmentExtraEntity Data)
        {
            try
            {
                this.DbContent.DepartmentExtraEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.DepartmentExtraEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentExtraEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.DepartmentExtraEntity.Remove(Data);
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

        public void Modify(int ID, Entity.DepartmentExtraEntity Data)
        {
            Entity.DepartmentExtraEntity Info = new();
            try
            {
                Info = this.DbContent.DepartmentExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.DepartmentID > 0 && Data.DepartmentID != Info.DepartmentID)
            {
                Info.DepartmentID = Data.DepartmentID;
            }
            if (Data.ExtraDesc != null)
            {
                if (Data.ExtraDesc != "" && Data.ExtraDesc != Info.ExtraDesc)
                {
                    Info.ExtraDesc = Data.ExtraDesc;
                }
            }
            if (Data.ExtraType > 0 && Data.ExtraType != Info.ExtraType)
            {
                Info.ExtraType = Data.ExtraType;
            }
            if (Data.ExtraValue != null)
            {
                if (Data.ExtraValue != "" && Data.ExtraValue != Info.ExtraValue)
                {
                    Info.ExtraValue = Data.ExtraValue;
                }
            }
        }

        public Entity.DepartmentExtraEntity Find(int ID)
        {
            Entity.DepartmentExtraEntity Data = new();
            try
            {
                Data = this.DbContent.DepartmentExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.DepartmentExtraEntity> Select(Entity.DepartmentExtraSelectParamEntity Data)
        {
            List<Entity.DepartmentExtraEntity> Result = new();
            var List = this.DbContent.DepartmentExtraEntity.Where(p => p.ID > 0);
            if (Data.DepartmentID > 0)
            {
                List = List.Where(p => p.DepartmentID == Data.DepartmentID);
            }
            if (Data.ExtraType > 0)
            {
                List = List.Where(p => p.ExtraType == Data.ExtraType);
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
    }
}
