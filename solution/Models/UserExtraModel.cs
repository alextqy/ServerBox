using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserExtraEntity : Base
    {
        public UserExtraEntity(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.UserExtraEntity Data)
        {
            try
            {
                this.DbContent.UserExtraEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.UserExtraEntity Data = new();
            try
            {
                Data = this.DbContent.UserExtraEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.UserExtraEntity.Remove(Data);
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

        public void Modify(int ID, Entity.UserExtraEntity Data)
        {
            Entity.UserExtraEntity Info = new();
            try
            {
                Info = this.DbContent.UserExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
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

        public Entity.UserExtraEntity Find(int ID)
        {
            Entity.UserExtraEntity Data = new();
            try
            {
                Data = this.DbContent.UserExtraEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.UserExtraEntity> Select(Entity.UserExtraSelectParamEntity Data)
        {
            List<Entity.UserExtraEntity> Result = new();
            var List = this.DbContent.UserExtraEntity.Where(p => p.ID > 0);
            if (Data.UserID > 0)
            {
                List = List.Where(p => p.UserID == Data.UserID);
            }
            if (Data.ExtraDesc != null)
            {
                if (Data.ExtraDesc != "")
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
                if (Data.ExtraValue != "")
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

        public void DeleteByUserID(int UserID)
        {
            var Data = this.DbContent.UserExtraEntity.Where(p => p.UserID == UserID);
            try
            {
                this.DbContent.UserExtraEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
