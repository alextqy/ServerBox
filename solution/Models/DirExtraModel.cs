using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class DirExtraModel : Base
    {
        public DirExtraModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.DirExtraEntity Data)
        {
            try
            {
                this.DbContent.DirExtraEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.DirExtraEntity Data = new();
            try
            {
                Data = this.DbContent.DirExtraEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.DirExtraEntity.Remove(Data);
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

        public void Modify(int ID, Entity.DirExtraEntity Data)
        {
            Entity.DirExtraEntity Info = new();
            try
            {
                Info = this.DbContent.DirExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.DirID > 0 && Data.DirID != Info.DirID)
            {
                Info.DirID = Data.DirID;
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

        public Entity.DirExtraEntity Find(int ID)
        {
            Entity.DirExtraEntity Data = new();
            try
            {
                Data = this.DbContent.DirExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.DirExtraEntity> Select(Entity.DirExtraSelectParamEntity Data)
        {
            List<Entity.DirExtraEntity> Result = new();
            var List = this.DbContent.DirExtraEntity.Where(p => p.ID > 0);

            if (Data.DirID > 0)
            {
                List = List.Where(p => p.DirID == Data.DirID);
            }
            if (Data.ExtraDesc != null)
            {
                if (!String.IsNullOrEmpty(Data.ExtraDesc))
                {
                    List = List.Where(p => p.ExtraDesc.Contains(Data.ExtraDesc.Trim()));
                }
            }
            if (Data.ExtraType > 0)
            {
                List = List.Where(p => p.ExtraType == Data.ExtraType);
            }
            if (Data.ExtraValue != null)
            {
                if (!String.IsNullOrEmpty(Data.ExtraValue))
                {
                    List = List.Where(p => p.ExtraValue.Contains(Data.ExtraValue.Trim()));
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
    }
}