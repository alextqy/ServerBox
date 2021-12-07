using Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class UserModel : Base
    {
        public UserModel(DbContentCore DbContent) : base(DbContent) { }

        public void Insert(Entity.UserEntity Data)
        {
            Data.Status = 1;
            try
            {
                this.DbContent.UserEntity.Add(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int ID)
        {
            Entity.UserEntity Data = new();
            try
            {
                Data = this.DbContent.UserEntity.Where(p => p.ID == ID).First();
                if (Data.ID > 0)
                {
                    try
                    {
                        this.DbContent.UserEntity.Remove(Data);
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

        public void Modify(int ID, Entity.UserEntity Data)
        {
            Entity.UserEntity Info = new();
            try
            {
                Info = this.DbContent.UserEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //if (Data.Account != null)
            //{
            //    if (Data.Account != "" && Data.Account != Info.Account)
            //    {
            //        Info.Account = Data.Account;
            //    }
            //}
            if (Data.Name != null)
            {
                if (Data.Name != "" && Data.Name != Info.Name && Data.Name.Length >= 4)
                {
                    Info.Name = Data.Name;
                }
            }
            if (Data.Password != "")
            {
                var CheckPassword = Tools.UserPWD(Data.Password, Info.Secret.ToString());
                Console.WriteLine(CheckPassword);
                if (Info.Password != CheckPassword)
                {
                    var Secret = Tools.Random(5);
                    Info.Secret = Secret;
                    var PasswordNew = Tools.UserPWD(Data.Password, Secret.ToString());
                    Info.Password = PasswordNew;
                }
            }
            if (Data.Status > 0 && Data.Status != Info.Status)
            {
                Info.Status = Data.Status;
            }
            if (Data.Admin >= 0 && Data.Admin != Info.Admin)
            {
                Info.Admin = Data.Admin;
            }
            if (Data.Avatar != null)
            {
                if (Data.Avatar != "" && Data.Avatar != Info.Avatar)
                {
                    Info.Avatar = Data.Avatar;
                }
            }
            if (Data.Wallpaper != null)
            {
                if (Data.Wallpaper != "" && Data.Wallpaper != Info.Wallpaper)
                {
                    Info.Wallpaper = Data.Wallpaper;
                }
            }
            if (Data.Permission != null)
            {
                if (Data.Permission != "" && Data.Permission != Info.Permission)
                {
                    Info.Permission = Data.Password;
                }
            }
            if (Data.Master >= 0 && Data.Master != Info.Master)
            {
                Info.Master = Data.Master;
            }
            if (Data.DepartmentID >= 0 && Data.DepartmentID != Info.DepartmentID)
            {
                Info.DepartmentID = Data.DepartmentID;
            }
        }

        public Entity.UserEntity Find(int ID)
        {
            Entity.UserEntity Data = new();
            try
            {
                Data = this.DbContent.UserEntity.Where(p => p.ID == ID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }

        public List<Entity.UserEntity> Select(Entity.UserSelectParamEntity Data)
        {
            List<Entity.UserEntity> Result = new();
            var List = this.DbContent.UserEntity.Where(p => p.ID > 0);

            if (Data.Account != null)
            {
                if (Data.Account != "")
                {
                    List = List.Where(p => p.Account.Contains(Data.Account));
                }
            }
            if (Data.Name != null)
            {
                if (Data.Name != "")
                {
                    List = List.Where(p => p.Name.Contains(Data.Name));
                }
            }
            if (Data.State > 0)
            {
                List = List.Where(p => p.Status == Data.State);
            }
            if (Data.Admin > 0)
            {
                List = List.Where(p => p.Admin == Data.Admin);
            }

            if (Data.Master > 0)
            {
                List = List.Where(p => p.Master == Data.Master);
            }

            if (Data.DepartmentID >= 0)
            {
                List = List.Where(p => p.DepartmentID == Data.DepartmentID);
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

        public int CountUser()
        {
            List<Entity.UserEntity> Result = new();
            return this.DbContent.UserEntity.Where(p => p.ID > 0).Count();
        }

        public Entity.UserEntity FindByAccount(string Account)
        {
            Entity.UserEntity Data = new();
            try
            {
                Data = this.DbContent.UserEntity.Where(p => p.Account == Account).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Data;
        }
    }
}
