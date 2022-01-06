using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class FileTagModel : Base
    {
        public FileTagModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.FileTagEntity Data)
        {
            try
            {
                this.DbContent.FileTagEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.FileTagEntity Data = new();
            try
            {
                Data = this.DbContent.FileTagEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.FileTagEntity.Remove(Data);
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

        public void Modify(int ID, Entity.FileTagEntity Data)
        {
            Entity.FileTagEntity Info = new();
            try
            {
                Info = this.DbContent.FileTagEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.FileID > 0 && Data.FileID != Info.FileID)
            {
                Info.FileID = Data.FileID;
            }
            if (Data.TagID > 0 && Data.TagID != Info.TagID)
            {
                Info.TagID = Data.TagID;
            }
        }

        public Entity.FileTagEntity Find(int ID)
        {
            Entity.FileTagEntity Data = new();
            try
            {
                Data = this.DbContent.FileTagEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.FileTagEntity> Select(Entity.FileTagSelectParamEntity Data)
        {
            List<Entity.FileTagEntity> Result = new();
            var List = this.DbContent.FileTagEntity.Where(p => p.ID > 0);

            if (Data.TagID > 0)
            {
                List = List.Where(p => p.TagID == Data.TagID);
            }
            if (Data.FileID > 0)
            {
                List = List.Where(p => p.FileID == Data.FileID);
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

        public void DeleteByTagID(int TagID)
        {
            Entity.FileTagEntity Data = new();
            try
            {
                Data = this.DbContent.FileTagEntity.Where(p => p.TagID == TagID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.FileTagEntity.Remove(Data);
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
    }
}
