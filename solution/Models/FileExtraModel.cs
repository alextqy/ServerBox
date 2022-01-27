using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class FileExtraModel : Base
    {
        public FileExtraModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.FileExtraEntity Data)
        {
            try
            {
                this.DbContent.FileExtraEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.FileExtraEntity Data = new();
            try
            {
                Data = this.DbContent.FileExtraEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.FileExtraEntity.Remove(Data);
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

        public void Modify(int ID, Entity.FileExtraEntity Data)
        {
            Entity.FileExtraEntity Info = new();
            try
            {
                Info = this.DbContent.FileExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.FileID > 0 && Data.FileID != Info.FileID)
            {
                Info.FileID = Data.FileID;
            }
            if (Data.ExtraDesc != "" && Data.ExtraDesc != Info.ExtraDesc)
            {
                Info.ExtraDesc = Data.ExtraDesc;
            }
            if (Data.ExtraType > 0 && Data.ExtraType != Info.ExtraType)
            {
                Info.ExtraType = Data.ExtraType;
            }
            if (Data.ExtraValue != "" && Data.ExtraValue != Info.ExtraValue)
            {
                Info.ExtraValue = Data.ExtraValue;
            }
        }

        public Entity.FileExtraEntity Find(int ID)
        {
            Entity.FileExtraEntity Data = new();
            try
            {
                Data = this.DbContent.FileExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.FileExtraEntity> Select(Entity.FileExtraSelectParamEntity Data)
        {
            List<Entity.FileExtraEntity> Result = new();
            var List = this.DbContent.FileExtraEntity.Where(p => p.ID > 0);

            if (Data.FileID > 0)
            {
                List = List.Where(p => p.FileID == Data.FileID);
            }
            if (Data.ExtraDesc != null)
            {
                if (!String.IsNullOrEmpty(Data.ExtraDesc))
                {
                    List = List.Where(p => p.ExtraDesc.Contains(Data.ExtraDesc));
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
                    List = List.Where(p => p.ExtraValue.Contains(Data.ExtraValue));
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
