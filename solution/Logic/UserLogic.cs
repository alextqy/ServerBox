using System;
using System.Linq;
using Models;
using Service;

namespace Logic
{
    public class UserLogic : Base
    {
        public UserLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Entity.LoginResultEntity SignIn(string Account, string Password, int TokenType)
        {
            Entity.LoginResultEntity Result = new();
            if (Account == "")
            {
                Result.Memo = "Account error";
            }
            else if (Password == "")
            {
                Result.Memo = "Password error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else
            {
                Account = Account.ToLower();
                var Info = this.UserModel.FindByAccount(Account);
                if (Info.ID == 0)
                {
                    Result.Memo = "Data not found";
                }
                else if (Info.Status != 1)
                {
                    Result.Memo = "Disabled account";
                }
                else
                {
                    var PWD = Tools.UserPWD(Password, Info.Secret.ToString());
                    if (Info.Password == PWD)
                    {
                        var Token = Tools.UserToken(Info.ID.ToString(), Info.Name);
                        Entity.TokenEntity TokenData = new();
                        TokenData.UserID = Info.ID;
                        TokenData.Token = Token;
                        TokenData.TokenType = TokenType;
                        TokenData.Createtime = Convert.ToInt32(Tools.Time());
                        try
                        {
                            var TA = this.BeginTransaction();
                            try
                            {
                                this.TokenModel.DeleteByUserID(Info.ID, TokenType);
                                this.TokenModel.Insert(TokenData);
                                this.WTL("User " + Info.Name + " sign in, (Account:" + Account + ", ID:" + Info.ID + ")", 1);
                                this.DbContent.SaveChanges();

                                if (!Tools.DirIsExists(Tools.UserBaseDir() + Account))
                                {
                                    if (Tools.CreateDir(Tools.UserBaseDir() + Account))
                                    {
                                        Result.State = true;
                                        Result.Memo = "success";
                                        Result.Token = Token;
                                        TA.Commit();
                                    }
                                    else
                                    {
                                        Result.Memo = "Login error";
                                        TA.Rollback();
                                    }
                                }
                                else
                                {
                                    Result.State = true;
                                    Result.Memo = "success";
                                    Result.Token = Token;
                                    TA.Commit();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Result.Memo = "Login error";
                                TA.Rollback();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Result.Memo = "Login error";
                        }
                    }
                    else
                    {
                        Result.Memo = "Password error";
                    }
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity SignOut(string Token, int TokenType)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token lost";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    try
                    {
                        this.TokenModel.DeleteByUserID(UserID, TokenType);
                        this.DbContent.SaveChanges();
                        Result.Memo = "Success";
                        Result.State = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Result.Memo = "Logout error";
                    }
                }
            }

            return Result;
        }

        public Entity.CommonResultEntity TokenRunningState(string Token, int TokenType)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else
            {
                var Data = this.TokenModel.FindByToken(Token, TokenType);
                if (Data.ID == 0)
                {
                    this.Result.Memo = "Fail";
                }
                else
                {
                    this.Result.State = true;
                    this.Result.Memo = "Success";
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CheckSelf(string Token, int TokenType)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    Result.Data = this.UserModel.Find(UserID);
                    Result.State = true;
                    Result.Memo = "Success";
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity UserModify(string Token, int TokenType, int ID, Entity.UserEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID < 0)
            {
                this.Result.Memo = "ID error";
            }
            else if (ID == 1)
            {
                this.Result.Memo = "Permission denied";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    Entity.UserEntity UserData;
                    if (ID == 0 || ID == UserID)
                    {
                        UserData = this.UserModel.Find(UserID);
                    }
                    else
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                        UserData = this.UserModel.Find(ID);
                        UserID = UserData.ID;
                    }

                    if (UserData.ID > 0)
                    {
                        try
                        {
                            if (Data.Name != "" && !Tools.RegCheckPro(Data.Name))
                            {
                                this.Result.Memo = "Name format error";
                            }
                            else if (Data.Password != "" && !Tools.RegCheck(Data.Password))
                            {
                                this.Result.Memo = "Password format error";
                            }
                            else if (Data.DepartmentID < 0)
                            {
                                this.Result.Memo = "DepartmentID error";
                            }
                            else
                            {
                                if (Data.DepartmentID > 0)
                                {
                                    var DepartmentInfo = this.DepartmentModel.Find(Data.DepartmentID);
                                    if (DepartmentInfo.ID == 0)
                                    {
                                        this.Result.Memo = "Department not found";
                                        return this.Result;
                                    }
                                }

                                try
                                {
                                    if (UserID == 1)
                                    {
                                        Data.Admin = 2;
                                        Data.Master = 2;
                                        Data.Permission = "1,2,3,4,5,6,7,8,9";
                                    }
                                    this.UserModel.Modify(UserID, Data);
                                    this.DbContent.SaveChanges();
                                    this.Result.State = true;
                                    this.Result.Memo = "Success";
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    this.Result.Memo = "Modify error";
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Modify error";
                        }
                    }
                    else
                    {
                        this.Result.Memo = "Data not found";
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity IsMaster(string Token, int TokenType)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    if (this.MasterVerify(UserID))
                    {
                        this.Result.State = true;
                        this.Result.Memo = "Master";
                    }
                    else
                    {
                        this.Result.Memo = "Not master";
                    }
                }
            }

            return this.Result;
        }

        public Entity.CommonResultEntity CreateUser(string Token, int TokenType, Entity.UserEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else
            {
                Data.Account = Data.Account.ToLower();
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var IsMaster = this.MasterVerify(UserID);
                    if (!IsMaster)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        if (Data.Account == "")
                        {
                            this.Result.Memo = "Account error";
                        }
                        else if (!Tools.RegCheck(Data.Account))
                        {
                            this.Result.Memo = "Account format error";
                        }
                        else if (Data.Account.Length < 4)
                        {
                            this.Result.Memo = "Account Length error";
                        }

                        else if (Data.Name == "")
                        {
                            this.Result.Memo = "Name error";
                        }
                        else if (!Tools.RegCheckPro(Data.Name))
                        {
                            this.Result.Memo = "Name format error";
                        }
                        else if (Data.Name.Length < 2)
                        {
                            this.Result.Memo = "Name Length error";
                        }

                        else if (Data.Password == "")
                        {
                            this.Result.Memo = "Password error";
                        }
                        else if (!Tools.RegCheck(Data.Password))
                        {
                            this.Result.Memo = "Password format error";
                        }
                        else if (Data.Password.Length < 6)
                        {
                            this.Result.Memo = "Password Length error";
                        }

                        else if (Data.Admin <= 0)
                        {
                            this.Result.Memo = "Admin error";
                        }

                        else if (Data.Status <= 0)
                        {
                            this.Result.Memo = "Status error";
                        }

                        //else if (Data.Permission == "")
                        //{
                        //    this.Result.Memo = "Permission error";
                        //}

                        else if (Data.Master <= 0)
                        {
                            this.Result.Memo = "Master error";
                        }

                        else if (Data.DepartmentID < 0)
                        {
                            this.Result.Memo = "DepartmentID error";
                        }

                        else
                        {
                            if (Data.DepartmentID > 0)
                            {
                                var DepartmentInfo = this.DepartmentModel.Find(Data.DepartmentID);
                                if (DepartmentInfo.ID == 0)
                                {
                                    this.Result.Memo = "Department not found";
                                    return this.Result;
                                }
                            }

                            var UserInfo = this.UserModel.FindByAccount(Data.Account);
                            if (UserInfo.ID > 0)
                            {
                                this.Result.Memo = "Account is exists";
                            }
                            else
                            {
                                var CountUser = this.UserModel.CountUser(); // 验证用户数
                                var ActivationCode = ConfigHelper.AppSettingsHelper.ActivationCode();
                                if (ActivationCode != "")
                                {
                                    var OSType = Tools.OSType();
                                    string Motherboard;
                                    if (OSType == "Linux")
                                    {
                                        Motherboard = Tools.SysShell("dmidecode", "-s system-uuid").Trim();
                                    }
                                    else if (OSType == "Windows")
                                    {
                                        Motherboard = Tools.SysShell("wmic", "csproduct get UUID").Replace("UUID", "").Trim();
                                    }
                                    else
                                    {
                                        Motherboard = "";
                                    }

                                    if (Motherboard != "")
                                    {
                                        var DeCode = Tools.AES_Decrypt(ActivationCode, 3);
                                        var DeCodeArr = Tools.Explode("_", DeCode);
                                        var HardwareCode = DeCodeArr[1];
                                        if (HardwareCode != Motherboard)
                                        {
                                            ConfigHelper.AppSettingsHelper.Unactivation(); // 清空当前激活码
                                            this.Result.Memo = "Create error";
                                            return this.Result;
                                        }
                                        var UserLimit = Tools.StrToInt32(DeCodeArr[2]) + 5;
                                        if (CountUser >= UserLimit)
                                        {
                                            this.Result.Memo = "The number of accounts has reached the upper limit";
                                            return this.Result;
                                        }
                                    }
                                    else
                                    {
                                        this.Result.Memo = "Create error";
                                        return this.Result;
                                    }
                                }
                                else
                                {
                                    if (CountUser > 5)
                                    {
                                        this.Result.Memo = "The number of accounts has reached the upper limit";
                                        return this.Result;
                                    }
                                }

                                Entity.UserEntity UserData = new();
                                var Secret = Tools.Random(5);
                                UserData.Account = Data.Account;
                                UserData.Name = Data.Name;
                                UserData.Password = Tools.UserPWD(Data.Password, Secret.ToString());
                                UserData.Avatar = Data.Avatar;
                                UserData.Wallpaper = Data.Wallpaper;
                                UserData.Admin = Data.Admin;
                                UserData.Status = Data.Status;
                                UserData.Permission = Data.Permission == "" ? "1,2,3,4,5,6,7,8,9" : Data.Permission;
                                UserData.Master = Data.Master;
                                UserData.Createtime = Tools.Time32();
                                UserData.Secret = Secret;

                                var TA = this.BeginTransaction();
                                try
                                {
                                    this.UserModel.Insert(UserData);
                                    this.WTL("User（ID " + UserID + ")" + " Create User (Account " + Data.Account + ")", 2);
                                    this.DbContent.SaveChanges();

                                    Entity.DirEntity DirData = new();
                                    DirData.DirName = UserData.Name;
                                    DirData.ParentID = 0;
                                    DirData.UserID = UserData.ID;
                                    DirData.Createtime = Tools.Time32();
                                    this.DirModel.Insert(DirData);
                                    this.DbContent.SaveChanges();

                                    if (Tools.MKDir(Tools.UserBaseDir(), Data.Account))
                                    {
                                        TA.Commit();
                                        this.Result.Memo = "Success";
                                        this.Result.State = true;
                                        this.Result.ID = UserData.ID;
                                    }
                                    else
                                    {
                                        TA.Rollback();
                                        this.Result.Memo = "Create error";
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    TA.Rollback();
                                    this.Result.Memo = "Create error";
                                }
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity RemoveUser(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0 || ID == 1)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(ID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "Data not Found";
                    }
                    else if (ID == UserID)
                    {
                        this.Result.Memo = "ID error";
                    }
                    else
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            var TA = this.BeginTransaction();
                            try
                            {
                                this.UserModel.Delete(UserInfo.ID);
                                this.UserExtraModel.DeleteByUserID(UserInfo.ID);
                                this.FileModel.DeleteByUserID(UserInfo.ID);
                                this.DirModel.DeleteByUserID(UserInfo.ID);
                                this.MessageModel.DeleteByReceiverID(UserInfo.ID);
                                this.MessageModel.DeleteBySenderID(UserInfo.ID);
                                this.OuterTokenModel.DeleteByUserID(UserInfo.ID);
                                this.TokenModel.DeleteByUserID(UserInfo.ID, 0);
                                this.DepartmentFileModel.DeleteByUserID(UserInfo.ID);
                                this.DbContent.SaveChanges();
                                if (Tools.DelDir(Tools.UserBaseDir() + UserInfo.Account, true))
                                {
                                    TA.Commit();
                                    this.Result.State = true;
                                    this.Result.Memo = "Success";
                                }
                                else
                                {
                                    TA.Rollback();
                                    this.Result.Memo = "Delete error";
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                TA.Rollback();
                                this.Result.Memo = "Delete error";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity UserInfo(string Token, int TokenType, int UID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (UID <= 0)
            {
                Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                //else if (!this.MasterVerify(UserID))
                //{
                //    Result.Memo = "Permission denied";
                //}
                else
                {
                    var Data = this.UserModel.Find(UID);
                    Data.Password = "";
                    Data.Secret = 0;

                    Result.Data = Data;
                    Result.State = true;
                    Result.Memo = "Success";
                }
            }

            return Result;
        }

        public Entity.CommonResultEntity SelectUser(string Token, int TokenType, Entity.UserSelectParamEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    Result.Data = this.UserModel.Select(Data);
                    for (var i = 0; i < Result.Data.Count; i++)
                    {
                        Result.Data[i].Password = "";
                        Result.Data[i].Secret = 0;
                    }

                    Result.State = true;
                    Result.Memo = "Success";
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CreateUserExtra(string Token, int TokenType, Entity.UserExtraEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.UserID < 0)
            {
                this.Result.Memo = "UserID error";
            }
            else if (Data.ExtraDesc == "")
            {
                this.Result.Memo = "ExtraDesc error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraDesc))
            {
                this.Result.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraType < 0)
            {
                this.Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue == "")
            {
                this.Result.Memo = "ExtraValue error";
            }
            else if (!Tools.RegCheckPro(Data.ExtraValue))
            {
                this.Result.Memo = "ExtraValue format error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    if (Data.UserID == 0)
                    {
                        Data.UserID = UserID;
                    }
                    if (Data.UserID > 0 && Data.UserID != UserID)
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                    }

                    if (UserModel.Find(Data.UserID).ID == 0)
                    {
                        this.Result.Memo = "User data does not exist";
                        return this.Result;
                    }

                    Entity.UserExtraEntity ExtraData = new();
                    ExtraData.UserID = Data.UserID;
                    ExtraData.ExtraDesc = Data.ExtraDesc;
                    ExtraData.ExtraType = Data.ExtraType;
                    ExtraData.ExtraValue = Data.ExtraValue;
                    try
                    {
                        this.Result.State = true;
                        this.Result.Memo = "Success";
                        this.UserExtraModel.Insert(ExtraData);
                        this.DbContent.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Create error";
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteUserExtra(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.UserExtraModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (Info.UserID != UserID && !this.MasterVerify(UserID))
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        try
                        {
                            this.UserExtraModel.Delete(ID);
                            this.DbContent.SaveChanges();
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Delete error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity SelectUserExtra(string Token, int TokenType, Entity.UserExtraSelectParamEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.UserID < 0)
            {
                this.Result.Memo = "UserID error";
            }
            else if (Data.ExtraDesc != "" && !Tools.RegCheckPro(Data.ExtraDesc))
            {
                this.Result.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraType < 0)
            {
                this.Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue != "" && !Tools.RegCheckPro(Data.ExtraValue))
            {
                this.Result.Memo = "ExtraValue format error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    if (Data.UserID > 0 && Data.UserID != UserID)
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                    }
                    if (Data.UserID == 0)
                    {
                        Data.UserID = UserID;
                    }
                    this.Result.State = true;
                    this.Result.Memo = "Success";
                    this.Result.Data = this.UserExtraModel.Select(Data);
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity SelectLog(string Token, int TokenType, int YMD = 0)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else
            {
                this.Result.State = true;
                this.Result.Memo = "Success";
                this.Result.Data = this.RL(YMD);
            }
            return this.Result;
        }

        public Entity.CommonResultEntity ClearLog(string Token, int TokenType)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else
            {
                this.Result.State = true;
                this.Result.Memo = "Success";
                this.Result.Data = this.CL();
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CreateOuterToken(string Token, int TokenType, Entity.OuterTokenEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (String.IsNullOrEmpty(Data.OuterToken))
            {
                this.Result.Memo = "OuterToken error";
            }
            else if (Data.TokenDesc == "")
            {
                this.Result.Memo = "TokenDesc error";
            }
            else if (!Tools.RegCheckPro(Data.TokenDesc))
            {
                this.Result.Memo = "TokenDesc format error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    try
                    {
                        this.OuterTokenModel.DeleteByUserID(UserID);
                        this.DbContent.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Create error";
                    }

                    Entity.OuterTokenEntity OuterTokenData = new();
                    OuterTokenData.UserID = UserID;
                    OuterTokenData.OuterToken = Data.OuterToken;
                    OuterTokenData.TokenDesc = Data.TokenDesc;
                    OuterTokenData.Createtime = Tools.Time32();
                    try
                    {
                        this.OuterTokenModel.Insert(OuterTokenData);
                        this.DbContent.SaveChanges();
                        this.Result.State = true;
                        this.Result.Memo = "Success";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Create error";
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CheckOuterToken(string OuterToken)
        {
            Result.Data = this.OuterTokenModel.FindByToken(OuterToken);
            Result.State = true;
            Result.Memo = "";
            return Result;
        }

        public Entity.CommonResultEntity CreateMessage(string Token, int TokenType, Entity.MessageEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.Title == "")
            {
                this.Result.Memo = "Title error";
            }
            else if (!Tools.RegCheckPro(Data.Title))
            {
                this.Result.Memo = "Title format error";
            }
            else if (Data.Content == "")
            {
                this.Result.Memo = "Content error";
            }
            else if (!Tools.RegCheckPro(Data.Content))
            {
                this.Result.Memo = "Content format error";
            }
            else if (Data.ReceiverID <= 0)
            {
                this.Result.Memo = "ReceiverID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (UserID == Data.ReceiverID)
                {
                    this.Result.Memo = "User error";
                }
                else
                {
                    var ReceiverInfo = this.UserModel.Find(Data.ReceiverID);
                    if (ReceiverInfo.ID == 0)
                    {
                        this.Result.Memo = "Receiver not found";
                    }
                    else
                    {
                        Entity.MessageEntity MessageData = new();
                        MessageData.Title = Data.Title;
                        MessageData.Content = Data.Content;
                        MessageData.SenderID = UserID;
                        MessageData.ReceiverID = Data.ReceiverID;
                        MessageData.State = 1;
                        MessageData.Createtime = Tools.Time32();
                        try
                        {
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                            this.MessageModel.Insert(MessageData);
                            this.DbContent.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Create error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CheckMessage(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        Result.Memo = "Data not found";
                    }
                    else if (UserID != Info.SenderID && UserID != Info.ReceiverID)
                    {
                        Result.Memo = "Permission denied";
                    }
                    else
                    {
                        if (UserID == Info.ReceiverID && Info.State == 1)
                        {
                            try
                            {
                                Info.State = 2;
                                this.MessageModel.Modify(Info.ID, Info);
                                this.DbContent.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Result.Memo = "Check error";
                                return Result;
                            }
                        }
                        Result.Data = Info;
                        Result.State = true;
                        Result.Memo = "Success";
                    }
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity MessageList(string Token, int TokenType, int MessageType, int UID, int State, int StartPoint = 0, int EndPoint = 0)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (MessageType <= 0)
            {
                this.Result.Memo = "MessageType error";
            }
            //else if (UID <= 0)
            //{
            //    Result.Memo = "UserID error";
            //}
            else if (State < 0)
            {
                this.Result.Memo = "State error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    Entity.MessageSelectParamEntity Data = new();
                    if (MessageType == 1)
                    {
                        Data.ReceiverID = UserID;
                        Data.SenderID = UID;
                    }
                    else
                    {
                        Data.SenderID = UserID;
                        Data.ReceiverID = UID;
                    }
                    if (State > 0)
                    {
                        Data.State = State;
                    }
                    if (StartPoint > 0)
                    {
                        Data.StartPoint = StartPoint;
                    }
                    if (EndPoint > 0)
                    {
                        Data.EndPoint = EndPoint;
                    }

                    this.Result.State = true;
                    this.Result.Memo = "Success";
                    this.Result.Data = this.MessageModel.Select(Data);
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteMessage(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (UserID != Info.ReceiverID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        try
                        {
                            this.MessageModel.Delete(ID);
                            this.DbContent.SaveChanges();
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Delete error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity SetMessage(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else if (UserID != Info.ReceiverID)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        try
                        {
                            //switch (Info.State)
                            //{
                            //    case 1:
                            //        Info.State = 2;
                            //        break;
                            //    case 2:
                            //        Info.State = 1;
                            //        break;
                            //    default:
                            //        Info.State = 2;
                            //        break;
                            //}

                            if (Info.State == 1)
                            {
                                Info.State = 2;
                                this.MessageModel.Modify(ID, Info);
                                this.DbContent.SaveChanges();
                            }

                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Set error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity ShareFilesToDepartment(string Token, int TokenType, int FileID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "FileID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(UserID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "User not found";
                    }
                    else if (UserInfo.DepartmentID <= 0)
                    {
                        this.Result.Memo = "The user has no department";
                    }
                    else
                    {
                        var DepartmentInfo = this.DepartmentModel.Find(UserInfo.DepartmentID);
                        if (DepartmentInfo.ID == 0)
                        {
                            this.Result.Memo = "Department not found";
                        }
                        else if (DepartmentInfo.ID != UserInfo.DepartmentID)
                        {
                            this.Result.Memo = "Department error";
                        }
                        else
                        {
                            var FileInfo = this.FileModel.Find(FileID);
                            if (FileInfo.ID == 0)
                            {
                                this.Result.Memo = "File not found";
                            }
                            else if (FileInfo.State != 2)
                            {
                                this.Result.Memo = "File status is abnormal";
                            }
                            else if (FileInfo.UserID != UserID)
                            {
                                this.Result.Memo = "Permission denied";
                            }
                            else
                            {
                                // 确认文件是否被分享过
                                Entity.DepartmentFileSelectParamEntity CheckData = new();
                                CheckData.DepartmentID = DepartmentInfo.ID;
                                CheckData.FileID = FileInfo.ID;
                                CheckData.UserID = UserInfo.ID;
                                var DataList = this.DepartmentFileModel.Select(CheckData);
                                if (DataList.Count > 0)
                                {
                                    this.Result.Memo = "Data is exists";
                                }
                                else
                                {
                                    Entity.DepartmentFileEntity Data = new();
                                    Data.DepartmentID = UserInfo.DepartmentID;
                                    Data.FileID = FileID;
                                    Data.UserID = UserID;
                                    Data.Createtime = Tools.Time32();
                                    try
                                    {
                                        this.DepartmentFileModel.Insert(Data);
                                        this.DbContent.SaveChanges();
                                        this.Result.State = true;
                                        this.Result.Memo = "success";
                                        this.Result.ID = Data.ID;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        this.Result.Memo = "Share error";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteDepartmentFile(string Token, int TokenType, int ID)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(UserID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "User not found";
                    }
                    else if (UserInfo.Admin != 2)
                    {
                        this.Result.Memo = "Permission denied";
                    }
                    else
                    {
                        var Info = this.DepartmentFileModel.Find(ID);
                        if (Info.ID == 0)
                        {
                            this.Result.Memo = "Data not found";
                        }
                        else if (Info.DepartmentID != UserInfo.DepartmentID)
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            try
                            {
                                this.DepartmentFileModel.Delete(ID);
                                this.DbContent.SaveChanges();
                                this.Result.Memo = "Success";
                                this.Result.State = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                this.Result.Memo = "Delete error";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity SelectDepartmentFile(string Token, int TokenType, Entity.DepartmentFileSelectParamEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.DepartmentID < 0)
            {
                this.Result.Memo = "DepartmentID error";
            }
            else if (Data.FileID < 0)
            {
                this.Result.Memo = "FileID error";
            }
            else if (Data.UserID < 0)
            {
                this.Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Token, TokenType);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserModel.Find(UserID);
                    if (UserInfo.ID == 0)
                    {
                        this.Result.Memo = "User not found";
                    }
                    else if (UserInfo.DepartmentID == 0)
                    {
                        this.Result.Memo = "Department error";
                    }
                    else
                    {
                        if (Data.UserID > 0 && Data.UserID != UserID)
                        {
                            var CheckUser = this.UserModel.Find(Data.UserID);
                            if (CheckUser.ID == 0)
                            {
                                this.Result.Memo = "User not found";
                                return this.Result;
                            }
                            if (CheckUser.DepartmentID != Data.DepartmentID)
                            {
                                this.Result.Memo = "Department error";
                                return this.Result;
                            }
                        }

                        if (Data.DepartmentID == 0)
                        {
                            Data.DepartmentID = UserInfo.DepartmentID;
                        }
                        var DepartmentInfo = this.DepartmentModel.Find(Data.DepartmentID);
                        if (DepartmentInfo.ID == 0)
                        {
                            this.Result.Memo = "Department not found";
                        }
                        else if (UserInfo.DepartmentID != DepartmentInfo.ID)
                        {
                            this.Result.Memo = "Permission denied";
                        }
                        else
                        {
                            this.Result.Data = this.DepartmentFileModel.Select(Data);
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity ResetMaster()
        {
            var Info = this.UserModel.Find(1);
            if (Info.ID == 0)
            {
                this.Result.Memo = "Data error";
                return this.Result;
            }
            var Data = new Entity.UserEntity
            {
                Password = "000000",
                Name = "Admin",
                Avatar = Info.Avatar,
                Wallpaper = Info.Wallpaper,
                Admin = Info.Admin,
                Status = Info.Status,
                Permission = Info.Permission,
                Master = Info.Master,
                DepartmentID = Info.DepartmentID,
            };
            try
            {
                this.UserModel.Modify(1, Data);
                this.DbContent.SaveChanges();
                this.Result.State = true;
                this.Result.Memo = "Success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.Result.Memo = "Modify error";
            }
            return this.Result;
        }
    }
}
