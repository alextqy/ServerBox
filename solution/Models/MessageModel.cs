using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class MessageModel : Base
    {
        public MessageModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.MessageEntity Data)
        {
            try
            {
                this.DbContent.MessageEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.MessageEntity Data = new();
            try
            {
                Data = this.DbContent.MessageEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.MessageEntity.Remove(Data);
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

        public void Modify(int ID, Entity.MessageEntity Data)
        {
            Entity.MessageEntity Info = new();
            try
            {
                Info = this.DbContent.MessageEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.Title != null)
            {
                if (Data.Title != "" && Data.Title != Info.Title)
                {
                    Info.Title = Data.Title;
                }
            }
            if (Data.Content != null)
            {
                if (Data.Content != "" && Data.Content != Info.Content)
                {
                    Info.Content = Data.Content;
                }
            }
            if (Data.SenderID > 0 && Data.SenderID != Info.SenderID)
            {
                Info.SenderID = Data.SenderID;
            }
            if (Data.ReceiverID > 0 && Data.ReceiverID != Info.ReceiverID)
            {
                Info.ReceiverID = Data.ReceiverID;
            }
            if (Data.State > 0 && Data.State != Info.State)
            {
                Info.State = Data.State;
            }
        }

        public Entity.MessageEntity Find(int ID)
        {
            Entity.MessageEntity Data = new();
            try
            {
                Data = this.DbContent.MessageEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.MessageEntity> Select(Entity.MessageSelectParamEntity Data)
        {
            List<Entity.MessageEntity> Result = new();
            var List = this.DbContent.MessageEntity.Where(p => p.ID > 0);
            if (Data.Title != null)
            {
                if (Data.Title != "")
                {
                    List = List.Where(p => p.Title.Contains(Data.Title));
                }
            }
            if (Data.Content != null)
            {
                if (Data.Content != "")
                {
                    List = List.Where(p => p.Content.Contains(Data.Content));
                }
            }
            if (Data.SenderID > 0)
            {
                List = List.Where(p => p.SenderID == Data.SenderID);
            }
            if (Data.ReceiverID > 0)
            {
                List = List.Where(p => p.ReceiverID == Data.ReceiverID);
            }
            if (Data.State > 0)
            {
                List = List.Where(p => p.State == Data.State);
            }
            if (Data.StartPoint > 0)
            {
                List = List.Where(p => p.Createtime >= Data.StartPoint);
            }
            if (Data.EndPoint > 0)
            {
                List = List.Where(p => p.Createtime <= Data.EndPoint);
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

        public void DeleteByReceiverID(int UserID)
        {
            var Data = this.DbContent.MessageEntity.Where(p => p.ReceiverID == UserID);
            try
            {
                this.DbContent.MessageEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DeleteBySenderID(int UserID)
        {
            var Data = this.DbContent.MessageEntity.Where(p => p.SenderID == UserID);
            try
            {
                this.DbContent.MessageEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
