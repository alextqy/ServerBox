using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class TokenModel : Base
    {
        public TokenModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.TokenEntity TokenData)
        {
            try
            {
                this.DbContent.TokenEntity.Add(TokenData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.TokenEntity TokenData = new();
            try
            {
                TokenData = this.DbContent.TokenEntity.Where(p => p.ID == ID).First();
                if (TokenData.ID > 0)
                {
                    try
                    {
                        this.DbContent.TokenEntity.Remove(TokenData);
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

        public void Modify(int ID, Entity.TokenEntity TokenData)
        {
            Entity.TokenEntity TokenInfo = new();
            try
            {
                TokenInfo = this.DbContent.TokenEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (!String.IsNullOrEmpty(TokenInfo.Token) && TokenInfo.Token != TokenData.Token)
            {
                TokenInfo.Token = TokenData.Token;
            }
            if (TokenInfo.TokenType > 0 && TokenInfo.TokenType != TokenData.TokenType)
            {
                TokenInfo.TokenType = TokenData.TokenType;
            }
            if (TokenInfo.Createtime > 0 && TokenInfo.Createtime != TokenData.Createtime)
            {
                TokenInfo.Createtime = TokenData.Createtime;
            }
        }

        public Entity.TokenEntity Find(int ID)
        {
            Entity.TokenEntity TokenData = new();
            try
            {
                TokenData = this.DbContent.TokenEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return TokenData;
        }

        public Entity.TokenEntity FindByToken(string Token, int TokenType)
        {
            Entity.TokenEntity TokenInfo = new();
            try
            {
                TokenInfo = this.DbContent.TokenEntity.Where(p => p.Token == Token && p.TokenType == TokenType).First();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return TokenInfo;
        }

        public void DeleteByUserID(int UserID, int TokenType)
        {
            var TokenData = this.DbContent.TokenEntity.Where(p => p.UserID == UserID);
            if (TokenType > 0)
            {
                TokenData = TokenData.Where(p => p.TokenType == TokenType);
            }
            try
            {
                this.DbContent.TokenEntity.RemoveRange(TokenData); // var List = TokenData.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
