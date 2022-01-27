using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class OfflineTaskModel : Base
    {
        public OfflineTaskModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.OfflineTaskEntity Data)
        {
            try
            {
                this.DbContent.OfflineTaskEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.OfflineTaskEntity Data = new();
            try
            {
                Data = this.DbContent.OfflineTaskEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.OfflineTaskEntity.Remove(Data);
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

        public void Modify(int ID, Entity.OfflineTaskEntity Data)
        {
            Entity.OfflineTaskEntity Info = new();
            try
            {
                Info = this.DbContent.OfflineTaskEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
            }
            if (!String.IsNullOrEmpty(Data.URL) && Data.URL != Info.URL)
            {
                Info.URL = Data.URL;
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

        public Entity.OfflineTaskEntity Find(int ID)
        {
            Entity.OfflineTaskEntity Data = new();
            try
            {
                Data = this.DbContent.OfflineTaskEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.OfflineTaskEntity> Select(Entity.OfflineTaskSelectParamEntity Data)
        {
            List<Entity.OfflineTaskEntity> Result = new();
            var List = this.DbContent.OfflineTaskEntity.Where(p => p.ID > 0);
            if (Data.UserID > 0)
            {
                List = List.Where(p => p.UserID == Data.UserID);
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

        public Entity.OfflineTaskEntity FindURL(int UserID, string URL)
        {
            Entity.OfflineTaskEntity Data = new();
            try
            {
                Data = this.DbContent.OfflineTaskEntity.Where(p => p.UserID == UserID).Where(p => p.URL == URL).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public Entity.OfflineTaskEntity FindAheadState(int State = 1)
        {
            Entity.OfflineTaskEntity Data = new();
            try
            {
                Data = this.DbContent.OfflineTaskEntity.Where(p => p.State == State).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }
    }
}
