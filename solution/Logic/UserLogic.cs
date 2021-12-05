using System;
using System.Linq;
using Models;
using Service;

namespace Logic
{
    public class UserLogic : Base
    {
        public UserLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Models.LoginResultModel SignIn(Models.LoginParamModel Data)
        {
            Models.LoginResultModel Result = new();
            if (Data.Account == "")
            {
                Result.Memo = "Account error";
            }
            else if (Data.Password == "")
            {
                Result.Memo = "Password error";
            }
            else if (Data.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else
            {
                Data.Account = Data.Account.ToLower();
                var Info = this.UserSQLModel.FindByAccount(Data.Account);
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
                    var PWD = Tools.UserPWD(Data.Password, Info.Secret.ToString());
                    if (Info.Password == PWD)
                    {
                        var Token = Tools.UserToken(Info.ID.ToString(), Info.Name);
                        Models.Entity.TokenModel TokenData = new();
                        TokenData.UserID = Info.ID;
                        TokenData.Token = Token;
                        TokenData.TokenType = Data.Type;
                        TokenData.Createtime = Convert.ToInt32(Tools.Time());
                        try
                        {
                            var TA = this.BeginTransaction();
                            try
                            {
                                this.TokenSQLModel.DeleteByUserID(Info.ID, Data.Type);
                                this.TokenSQLModel.Insert(TokenData);
                                this.LogSQLModel.WTL(this.IP, "User " + Info.Name + " sign in, (Account:" + Data.Account + ", ID:" + Info.ID + ")", 1);
                                this.DbContent.SaveChanges();

                                if (!Tools.DirIsExists(Tools.BaseDir() + Data.Account))
                                {
                                    if (Tools.CreateDir(Tools.BaseDir() + Data.Account))
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

        public Models.Worker.LoginResultModel SignOut(Models.Worker.LogoutParamModel Data)
        {
            Models.Worker.LoginResultModel Result = new();
            if (Data.Token == "")
            {
                Result.Memo = "Token lost";
            }
            else if (Data.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else
            {
                var UserID = this.TokenVerify(Data.Token, Data.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    try
                    {
                        this.TokenSQLModel.DeleteByUserID(UserID, Data.Type);
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

        public Models.Worker.CommonResultModel TokenRunningState(Models.Worker.CommonParamModel Param)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else
            {
                var Data = this.TokenSQLModel.FindByToken(Param.Token, Param.Type);
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

        public Models.Worker.UserInfoModel CheckSelf(Models.Worker.CommonParamModel Param)
        {
            Models.Worker.UserInfoModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    var UserData = this.UserSQLModel.Find(UserID);
                    if (UserData.ID > 0)
                    {
                        Result.State = true;
                        Result.Memo = "Success";
                        Result.Data = UserData;
                    }
                    else
                    {
                        Result.Memo = "Error";
                    }
                }
            }
            return Result;
        }

        public Models.Worker.CommonResultModel UserModify(Models.Worker.CommonParamModel Param, int ID, Models.Worker.UserModifyParamModel Data)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
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
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    Models.Entity.UserModel UserData = new();
                    if (ID == 0 || ID == UserID)
                    {
                        UserData = this.UserSQLModel.Find(UserID);
                    }
                    else
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            this.Result.Memo = "Permission denied";
                            return this.Result;
                        }
                        UserData = this.UserSQLModel.Find(ID);
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
                                    var DepartmentInfo = this.DepartmentSQLModel.Find(Data.DepartmentID);
                                    if (DepartmentInfo.ID == 0)
                                    {
                                        this.Result.Memo = "Department not found";
                                        return this.Result;
                                    }
                                }

                                try
                                {
                                    this.UserSQLModel.Modify(UserID, Data);
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

        public Models.Worker.CommonResultModel IsMaster(Models.Worker.CommonParamModel Param)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
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

        public Models.Worker.CommonResultModel CreateUser(Models.Worker.CommonParamModel Param, Models.Worker.CreateUserParamModel Data)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else
            {
                Data.Account = Data.Account.ToLower();
                var UserID = this.TokenVerify(Param.Token, Param.Type);
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

                        else if (Data.Permission == "")
                        {
                            this.Result.Memo = "Permission error";
                        }

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
                                var DepartmentInfo = this.DepartmentSQLModel.Find(Data.DepartmentID);
                                if (DepartmentInfo.ID == 0)
                                {
                                    this.Result.Memo = "Department not found";
                                    return this.Result;
                                }
                            }

                            var UserInfo = this.UserSQLModel.FindByAccount(Data.Account);
                            if (UserInfo.ID > 0)
                            {
                                this.Result.Memo = "Account is exists";
                            }
                            else
                            {
                                var CountUser = this.UserSQLModel.CountUser(); // 验证用户数

                                var Profile = Tools.RootPath() + "Profile.json"; // 查看配置文件
                                var ProfileObject = JsonTools.CheckProfile(Tools.RootPath() + "Profile.json");
                                if (ProfileObject.ActivationCode != "")
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
                                        var DeCode = Tools.AES_Decrypt(ProfileObject.ActivationCode, 3);
                                        var DeCodeArr = Tools.Explode("_", DeCode);
                                        var HardwareCode = DeCodeArr[1];
                                        if (HardwareCode != Motherboard)
                                        {
                                            ProfileObject.ActivationCode = ""; // 清空当前激活码
                                            var JsonString = JsonTools.ProfileToString(ProfileObject); // 配置项转为json格式
                                            JsonTools.StringToFile(Profile, JsonString); // 写入文件
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

                                Models.Entity.UserModel UserData = new();
                                var Secret = Tools.Random(5);
                                UserData.Account = Data.Account;
                                UserData.Name = Data.Name;
                                UserData.Password = Tools.UserPWD(Data.Password, Secret.ToString());
                                UserData.Avatar = Data.Avatar;
                                UserData.Wallpaper = Data.Wallpaper;
                                UserData.Admin = Data.Admin;
                                UserData.Status = Data.Status;
                                UserData.Permission = Data.Permission;
                                UserData.Master = Data.Master;
                                UserData.Createtime = Tools.Time32();
                                UserData.Secret = Secret;

                                var TA = this.BeginTransaction();
                                try
                                {
                                    this.UserSQLModel.Insert(UserData);
                                    this.LogSQLModel.WTL(this.IP, "User（ID " + UserID + ")" + " Create User (Account " + Data.Account + ")", 2);
                                    this.DbContent.SaveChanges();

                                    Models.Entity.DirModel DirData = new();
                                    DirData.DirName = UserData.Name;
                                    DirData.ParentID = 0;
                                    DirData.UserID = UserData.ID;
                                    DirData.Createtime = Tools.Time32();
                                    this.DirSQLModel.Insert(DirData);
                                    this.DbContent.SaveChanges();

                                    if (Tools.MKDir(Tools.BaseDir(), Data.Account))
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

        public Models.Worker.CommonResultModel RemoveUser(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0 || ID == 1)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserSQLModel.Find(ID);
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
                                this.UserSQLModel.Delete(UserInfo.ID);
                                this.UserExtraSQLModel.DeleteByUserID(UserInfo.ID);
                                this.FileSQLModel.DeleteByUserID(UserInfo.ID);
                                this.DirSQLModel.DeleteByUserID(UserInfo.ID);
                                this.MessageSQLModel.DeleteByReceiverID(UserInfo.ID);
                                this.MessageSQLModel.DeleteBySenderID(UserInfo.ID);
                                this.OuterTokenSQLModel.DeleteByUserID(UserInfo.ID);
                                this.TokenSQLModel.DeleteByUserID(UserInfo.ID, 0);
                                this.DepartmentFileSQLModel.DeleteByUserID(UserInfo.ID);
                                this.DbContent.SaveChanges();
                                if (Tools.DelDir(Tools.BaseDir() + UserInfo.Account, true))
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

        public Models.Worker.UserInfoModel UserInfo(Models.Worker.CommonParamModel Param, int UID)
        {
            Models.Worker.UserInfoModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (UID <= 0)
            {
                Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
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
                    var UserInfo = this.UserSQLModel.Find(UID);
                    UserInfo.Password = "";
                    UserInfo.Secret = 0;

                    Result.Data = UserInfo;
                    Result.State = true;
                    Result.Memo = "Success";
                }
            }

            return Result;
        }

        public Models.Worker.UserSelectResultModel SelectUser(Models.Worker.CommonParamModel Param, Models.Worker.UserSelectParamModel Data)
        {
            Models.Worker.UserSelectResultModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    Result.Data = this.UserSQLModel.Select(Data);
                    for (var i = 0; i < Result.Data.Count; i++)
                    {
                        Result.Data[i].Password = "";
                        Result.Data[i].Secret = 0;
                    }

                    Result.State = true;
                    Result.Memo = "Success";
                }
            }
            return Result;
        }

        public Models.Worker.CommonResultModel CreateUserExtra(Models.Worker.CommonParamModel Param, Models.Worker.CreateUserExtraParamModel Data)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
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
                var UserID = this.TokenVerify(Param.Token, Param.Type);
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

                    Models.Entity.UserExtraModel ExtraData = new();
                    ExtraData.UserID = Data.UserID;
                    ExtraData.ExtraDesc = Data.ExtraDesc;
                    ExtraData.ExtraType = Data.ExtraType;
                    ExtraData.ExtraValue = Data.ExtraValue;
                    try
                    {
                        this.Result.State = true;
                        this.Result.Memo = "Success";
                        this.UserExtraSQLModel.Insert(ExtraData);
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

        public Models.Worker.CommonResultModel DeleteUserExtra(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.UserExtraSQLModel.Find(ID);
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
                            this.UserExtraSQLModel.Delete(ID);
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

        public Models.Worker.UserExtraSelectResultModel SelectUserExtra(Models.Worker.CommonParamModel Param, Models.Worker.UserExtraSelectParamModel Data)
        {
            Models.Worker.UserExtraSelectResultModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (Data.UserID < 0)
            {
                Result.Memo = "UserID error";
            }
            else if (Data.ExtraDesc != "" && !Tools.RegCheckPro(Data.ExtraDesc))
            {
                Result.Memo = "ExtraDesc format error";
            }
            else if (Data.ExtraType < 0)
            {
                Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue != "" && !Tools.RegCheckPro(Data.ExtraValue))
            {
                Result.Memo = "ExtraValue format error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    if (Data.UserID > 0 && Data.UserID != UserID)
                    {
                        if (!this.MasterVerify(UserID))
                        {
                            Result.Memo = "Permission denied";
                            return Result;
                        }
                    }
                    if (Data.UserID == 0)
                    {
                        Data.UserID = UserID;
                    }
                    Result.State = true;
                    Result.Memo = "Success";
                    Result.Data = this.UserExtraSQLModel.Select(Data);
                }
            }
            return Result;
        }

        public Models.Worker.LogSelectResultModel SelectLog(Models.Worker.CommonParamModel Param, Models.Worker.LogSelectParamModel Data)
        {
            Models.Worker.LogSelectResultModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else if (!this.MasterVerify(UserID))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    Result.State = true;
                    Result.Memo = "Success";
                    Result.Data = this.LogSQLModel.Select(Data);
                }
            }
            return Result;
        }

        public Models.Worker.CommonResultModel DeleteLog(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.LogSQLModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        try
                        {
                            this.LogSQLModel.Delete(ID);
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

        public Models.Worker.CommonResultModel CreateOuterToken(Models.Worker.CommonParamModel Param, Models.Worker.CreateOuterTokenParamModel Data)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (Data.OuterToken == "")
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
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    try
                    {
                        this.OuterTokenSQLModel.DeleteByUserID(UserID);
                        this.DbContent.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Create error";
                    }

                    Models.Entity.OuterTokenModel OuterTokenData = new();
                    OuterTokenData.UserID = UserID;
                    OuterTokenData.OuterToken = Data.OuterToken;
                    OuterTokenData.TokenDesc = Data.TokenDesc;
                    OuterTokenData.Createtime = Tools.Time32();
                    try
                    {
                        this.OuterTokenSQLModel.Insert(OuterTokenData);
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

        public Models.Entity.OuterTokenModel CheckOuterToken(string OuterToken)
        {
            Models.Entity.OuterTokenModel Result = new();
            if (OuterToken != "")
            {
                Result = this.OuterTokenSQLModel.FindByToken(OuterToken);
            }
            return Result;
        }

        public Models.Worker.CommonResultModel CreateMessage(Models.Worker.CommonParamModel Param, Models.Worker.CreateMessageParamModel Data)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
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
                var UserID = this.TokenVerify(Param.Token, Param.Type);
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
                    var ReceiverInfo = this.UserSQLModel.Find(Data.ReceiverID);
                    if (ReceiverInfo.ID == 0)
                    {
                        this.Result.Memo = "Receiver not found";
                    }
                    else
                    {
                        Models.Entity.MessageModel MessageData = new();
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
                            this.MessageSQLModel.Insert(MessageData);
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

        public Models.Worker.MessageDataModel CheckMessage(Models.Worker.CommonParamModel Param, int ID)
        {
            Models.Worker.MessageDataModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageSQLModel.Find(ID);
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
                                this.MessageSQLModel.Modify(Info.ID, Info);
                                this.DbContent.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Result.Memo = "Check error";
                                return Result;
                            }
                        }

                        Result.State = true;
                        Result.Memo = "Success";
                        Result.Data = Info;
                    }
                }
            }
            return Result;
        }

        public Models.Worker.MessageSelectResultModel MessageList(Models.Worker.CommonParamModel Param, int MessageType, int UID, int State, int StartPoint = 0, int EndPoint = 0)
        {
            Models.Worker.MessageSelectResultModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (MessageType <= 0)
            {
                Result.Memo = "MessageType error";
            }
            //else if (UID <= 0)
            //{
            //    Result.Memo = "UserID error";
            //}
            else if (State < 0)
            {
                Result.Memo = "State error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    Models.Worker.MessageSelectParamModel Data = new();

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

                    Result.State = true;
                    Result.Memo = "Success";
                    var DataList = this.MessageSQLModel.Select(Data);
                    Result.Data = DataList;
                }
            }
            return Result;
        }

        public Models.Worker.CommonResultModel DeleteMessage(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageSQLModel.Find(ID);
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
                            this.MessageSQLModel.Delete(ID);
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

        public Models.Worker.CommonResultModel SetMessage(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var Info = this.MessageSQLModel.Find(ID);
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
                                this.MessageSQLModel.Modify(ID, Info);
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

        public Models.Worker.CommonResultModel ShareFilesToDepartment(Models.Worker.CommonParamModel Param, int FileID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (FileID <= 0)
            {
                this.Result.Memo = "FileID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserSQLModel.Find(UserID);
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
                        var DepartmentInfo = this.DepartmentSQLModel.Find(UserInfo.DepartmentID);
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
                            var FileInfo = this.FileSQLModel.Find(FileID);
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
                                Models.Worker.DepartmentFileSelectParamModel CheckData = new();
                                CheckData.DepartmentID = DepartmentInfo.ID;
                                CheckData.FileID = FileInfo.ID;
                                CheckData.UserID = UserInfo.ID;
                                var DataList = this.DepartmentFileSQLModel.Select(CheckData);
                                if (DataList.Count > 0)
                                {
                                    this.Result.Memo = "Data is exists";
                                }
                                else
                                {
                                    Models.Entity.DepartmentFileModel Data = new();
                                    Data.DepartmentID = UserInfo.DepartmentID;
                                    Data.FileID = FileID;
                                    Data.UserID = UserID;
                                    Data.Createtime = Tools.Time32();
                                    try
                                    {
                                        this.DepartmentFileSQLModel.Insert(Data);
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

        public Models.Worker.CommonResultModel DeleteDepartmentFile(Models.Worker.CommonParamModel Param, int ID)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (ID <= 0)
            {
                this.Result.Memo = "ID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    this.Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserSQLModel.Find(UserID);
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
                        var Info = this.DepartmentFileSQLModel.Find(ID);
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
                                this.DepartmentFileSQLModel.Delete(ID);
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

        public Models.Worker.DepartmentFileSelectResultModel SelectDepartmentFile(Models.Worker.CommonParamModel Param, Models.Worker.DepartmentFileSelectParamModel Data)
        {
            Models.Worker.DepartmentFileSelectResultModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (Data.DepartmentID < 0)
            {
                Result.Memo = "DepartmentID error";
            }
            else if (Data.FileID < 0)
            {
                Result.Memo = "FileID error";
            }
            else if (Data.UserID < 0)
            {
                Result.Memo = "UserID error";
            }
            else
            {
                var UserID = this.TokenVerify(Param.Token, Param.Type);
                if (UserID == 0)
                {
                    Result.Memo = "Token lost";
                }
                else
                {
                    var UserInfo = this.UserSQLModel.Find(UserID);
                    if (UserInfo.ID == 0)
                    {
                        Result.Memo = "User not found";
                    }
                    else if (UserInfo.DepartmentID == 0)
                    {
                        Result.Memo = "Department error";
                    }
                    else
                    {
                        if (Data.UserID > 0 && Data.UserID != UserID)
                        {
                            var CheckUser = this.UserSQLModel.Find(Data.UserID);
                            if (CheckUser.ID == 0)
                            {
                                Result.Memo = "User not found";
                                return Result;
                            }
                            if (CheckUser.DepartmentID != Data.DepartmentID)
                            {
                                Result.Memo = "Department error";
                                return Result;
                            }
                        }

                        if (Data.DepartmentID == 0)
                        {
                            Data.DepartmentID = UserInfo.DepartmentID;
                        }
                        var DepartmentInfo = this.DepartmentSQLModel.Find(Data.DepartmentID);
                        if (DepartmentInfo.ID == 0)
                        {
                            Result.Memo = "Department not found";
                        }
                        else if (UserInfo.DepartmentID != DepartmentInfo.ID)
                        {
                            Result.Memo = "Permission denied";
                        }
                        else
                        {
                            Result.Data = this.DepartmentFileSQLModel.Select(Data);
                            Result.State = true;
                            Result.Memo = "Success";
                        }
                    }
                }
            }
            return Result;
        }

        public Models.Worker.CommonResultModel ResetMaster()
        {
            var Info = this.UserSQLModel.Find(1);
            if (Info.ID == 0)
            {
                this.Result.Memo = "Data error";
                return this.Result;
            }
            var Data = new Models.Worker.UserModifyParamModel
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
                this.UserSQLModel.Modify(1, Data);
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
