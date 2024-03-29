﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class TagModel : Base
    {
        public TagModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.TagEntity Data)
        {
            try
            {
                this.DbContent.TagEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.TagEntity Data = new();
            try
            {
                Data = this.DbContent.TagEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.TagEntity.Remove(Data);
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

        public void Modify(int ID, Entity.TagEntity Data)
        {
            Entity.TagEntity Info = new();
            try
            {
                Info = this.DbContent.TagEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (!String.IsNullOrEmpty(Data.TagName) && Data.TagName != Info.TagName)
            {
                Info.TagName = Data.TagName;
            }
            if (!String.IsNullOrEmpty(Data.TagMemo) && Data.TagMemo != Info.TagMemo)
            {
                Info.TagMemo = Data.TagMemo;
            }
            if (Data.UserID > 0 && Data.UserID != Info.UserID)
            {
                Info.UserID = Data.UserID;
            }
        }

        public Entity.TagEntity Find(int ID)
        {
            Entity.TagEntity Data = new();
            try
            {
                Data = this.DbContent.TagEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.TagEntity> Select(Entity.TagSelectParamEntity Data)
        {
            List<Entity.TagEntity> Result = new();
            var List = this.DbContent.TagEntity.Where(p => p.ID > 0);
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
    }
}
