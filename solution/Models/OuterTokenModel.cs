using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class OuterTokenModel : Base
    {
        public OuterTokenModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.OuterTokenEntity Data)
        {
            try
            {
                this.DbContent.OuterTokenEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.OuterTokenEntity Data = new();
            try
            {
                Data = this.DbContent.OuterTokenEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.OuterTokenEntity.Remove(Data);
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

        public void Modify(int ID, Entity.OuterTokenEntity Data)
        {
            Entity.OuterTokenEntity Info = new();
            try
            {
                Info = this.DbContent.OuterTokenEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Data.UserID > 0)
            {
                Info.UserID = Data.UserID;
            }
            if (Data.OuterToken != null)
            {
                if (Data.OuterToken != "")
                {
                    Info.OuterToken = Data.OuterToken;
                }
            }
            if (Data.TokenDesc != null)
            {
                if (Data.TokenDesc != "")
                {
                    Info.TokenDesc = Data.TokenDesc;
                }
            }
        }

        public Entity.OuterTokenEntity Find(int ID)
        {
            Entity.OuterTokenEntity Data = new();
            try
            {
                Data = this.DbContent.OuterTokenEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.OuterTokenEntity> Select(Entity.OuterTokenSelectParamEntity Data)
        {
            List<Entity.OuterTokenEntity> Result = new();
            var List = this.DbContent.OuterTokenEntity.Where(p => p.ID > 0);
            if (Data.UserID > 0)
            {
                List = List.Where(p => p.UserID == Data.UserID);
            }
            if (Data.OuterToken != null)
            {
                if (Data.OuterToken != "")
                {
                    List = List.Where(p => p.OuterToken.Contains(Data.OuterToken));
                }
            }
            if (Data.TokenDesc != null)
            {
                if (Data.TokenDesc != "")
                {
                    List = List.Where(p => p.TokenDesc.Contains(Data.TokenDesc));
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
            var Data = this.DbContent.OuterTokenEntity.Where(p => p.UserID == UserID);
            try
            {
                this.DbContent.OuterTokenEntity.RemoveRange(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Entity.OuterTokenEntity FindByToken(string OuterToken)
        {
            Entity.OuterTokenEntity TokenInfo = new();
            try
            {
                TokenInfo = this.DbContent.OuterTokenEntity.Where(p => p.OuterToken == OuterToken).First();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return TokenInfo;
        }
    }
}
