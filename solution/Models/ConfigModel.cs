using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class ConfigModel : Base
    {
        public ConfigModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.ConfigEntity Data)
        {
            try
            {
                this.DbContent.ConfigEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.ConfigEntity Data = new();
            try
            {
                Data = this.DbContent.ConfigEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.ConfigEntity.Remove(Data);
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

        public void Modify(int ID, Entity.ConfigEntity Data)
        {
            Entity.ConfigEntity Info = new();
            try
            {
                Info = this.DbContent.ConfigEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.ConfigKey != null)
            {
                if (Data.ConfigKey != "" && Data.ConfigKey != Info.ConfigKey)
                {
                    Info.ConfigKey = Data.ConfigKey;
                }
            }
            if (Data.ConfigDesc != null)
            {
                if (Data.ConfigDesc != "" && Data.ConfigDesc != Info.ConfigDesc)
                {
                    Info.ConfigDesc = Data.ConfigDesc;
                }
            }
            if (Data.ConfigType > 0 && Data.ConfigType != Info.ConfigType)
            {
                Info.ConfigType = Data.ConfigType;
            }
            if (Data.ConfigValue != null)
            {
                if (Data.ConfigValue != "" && Data.ConfigValue != Info.ConfigValue)
                {
                    Info.ConfigValue = Data.ConfigValue;
                }
            }
        }

        public Entity.ConfigEntity Find(int ID)
        {
            Entity.ConfigEntity Data = new();
            try
            {
                Data = this.DbContent.ConfigEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.ConfigEntity> Select()
        {
            List<Entity.ConfigEntity> Result = new();
            var List = this.DbContent.ConfigEntity.Where(p => p.ID > 0);
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

        public string CheckConfigValue(string Param)
        {
            Entity.ConfigEntity Data = new();
            try
            {
                Data = this.DbContent.ConfigEntity.Where(p => p.ConfigKey == Param).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data.ConfigValue;
        }
    }
}
