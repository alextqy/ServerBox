using System;
using System.IO;
using Models;

namespace Logic
{
    public class ConfigLogic : Base
    {
        public ConfigLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Entity.ConfigEntity CheckConfig(string Token, int TokenType, int ID)
        {
            Models.ConfigDataModel Result = new();

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
                else if (!this.MasterVerify(UserID))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    Result.State = true;
                    Result.Memo = "Success";
                    Result.Data = this.ConfigSQLModel.Find(ID);
                }
            }

            return Result;
        }

        public Entity.CommonResultEntity ModifyConfig(string Token, int TokenType, int ID, Entity.ConfigEntity Data)
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
                    Entity.ConfigModel ConfigData = new();
                    ConfigData.ConfigKey = Data.ConfigKey;
                    ConfigData.ConfigDesc = Data.ConfigDesc;
                    ConfigData.ConfigType = Data.ConfigType;
                    ConfigData.ConfigValue = Data.ConfigValue;
                    try
                    {
                        this.Result.State = true;
                        this.Result.Memo = "Success";
                        this.ConfigSQLModel.Modify(ID, ConfigData);
                        this.DbContent.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Result.Memo = "Modify error";
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity GetHardDiskSpaceInfo(string Token, int TokenType)
        {
            Models.CommonDataModel Result = new();
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
                    foreach (DriveInfo Drive in DriveInfo.GetDrives())
                    {
                        // 判断是否是固定磁盘
                        if (Drive.DriveType == DriveType.Fixed)
                        {
                            var LSUM = Drive.TotalSize / (1024 * 1024 * 1024); // 总空间(GB)
                            var LDR = Drive.TotalFreeSpace / (1024 * 1024 * 1024); // 剩余空间(GB)
                            Result.Data = LSUM.ToString() + "_" + LDR.ToString();
                            Result.State = true;
                            Result.Memo = "Success";
                        }
                    }
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity DecryptProInspection(string Token, int TokenType, string Data, int CodeType = 1)
        {
            Models.CommonDataModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (Data == "")
            {
                Result.Memo = "Param error";
            }
            else if (CodeType <= 0)
            {
                Result.Memo = "CodeType error";
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
                    Result.State = true;
                    Result.Memo = "Success";
                    Result.Data = Tools.AES_Decrypt(Data, CodeType);
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity GenerateActivationCodeInspection(string Token, int TokenType, string EncryptedCode, int NumberOfAccounts = 5)
        {
            Models.CommonDataModel Result = new();
            if (Param.Token == "")
            {
                Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                Result.Memo = "Type error";
            }
            else if (EncryptedCode == "")
            {
                Result.Memo = "EncryptedCode error";
            }
            else if (NumberOfAccounts <= 0)
            {
                Result.Memo = "NumberOfAccounts error";
            }
            else if (NumberOfAccounts % 5 != 0)
            {
                Result.Memo = "NumberOfAccounts error";
            }
            else
            {
                var DeCode = Tools.AES_Decrypt(EncryptedCode, 1); // 解密
                var DeCodeArr = Tools.Explode("_", DeCode);
                DeCodeArr[2] = (Tools.StrToInt(DeCodeArr[2]) + NumberOfAccounts).ToString(); // 计算总账号数
                var EnCode = Tools.Implode("_", DeCodeArr);
                Result.Data = Tools.AES_Encrypt(EnCode, 2);
                Result.State = true;
                Result.Memo = "Success";
            }
            return Result;
        }

        public Entity.CommonResultEntity GetHardwareCode(string Token, int TokenType)
        {
            Models.CommonDataModel Result = new();
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
                    Result.State = true;
                    Result.Memo = "Success";
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
                        var OldAccountsNum = "0";
                        var CheckProfile = JsonTools.CheckProfile(Tools.RootPath() + "Profile.json"); // 是否已经激活过
                        if (CheckProfile.ActivationCode != "")
                        {
                            var DeCode = Tools.AES_Decrypt(CheckProfile.ActivationCode, 3);
                            OldAccountsNum = Tools.Explode("_", DeCode)[2];
                        }
                        Motherboard = Tools.AES_Encrypt(Tools.TimeMS().ToString() + "_" + Motherboard + "_" + OldAccountsNum);
                    }

                    Result.Data = Motherboard;
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity ProductActivation(string Token, int TokenType, string EncryptedCode)
        {
            if (Param.Token == "")
            {
                this.Result.Memo = "Token error";
            }
            else if (Param.Type <= 0)
            {
                this.Result.Memo = "Type error";
            }
            else if (EncryptedCode == "")
            {
                this.Result.Memo = "EncryptedCode error";
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
                    var DeCode = Tools.AES_Decrypt(EncryptedCode, 2); // 解密
                    if (DeCode != "")
                    {
                        var DeCodeArr = Tools.Explode("_", DeCode);
                        var ProfileObject = JsonTools.CheckProfile(Tools.RootPath() + "Profile.json");
                        if (ProfileObject.ActivationCode != "")
                        {
                            if (ProfileObject.ActivationCode != EncryptedCode)
                            {
                                var NewHardwareCode = DeCodeArr[1];
                                var OldHardwareCode = Tools.Explode("_", Tools.AES_Decrypt(ProfileObject.ActivationCode, 3))[1];
                                if (OldHardwareCode != NewHardwareCode)
                                {
                                    this.Result.Memo = "Activation fails";
                                    return this.Result;
                                }
                                string CurrentHardwareCode;
                                var OSType = Tools.OSType();
                                if (OSType == "Linux")
                                {
                                    CurrentHardwareCode = Tools.SysShell("dmidecode", "-s system-uuid").Trim();
                                }
                                else if (OSType == "Windows")
                                {
                                    CurrentHardwareCode = Tools.SysShell("wmic", "csproduct get UUID").Replace("UUID", "").Trim();
                                }
                                else
                                {
                                    CurrentHardwareCode = "";
                                }
                                if (CurrentHardwareCode == "")
                                {
                                    this.Result.Memo = "Activation fails";
                                    return this.Result;
                                }
                                else
                                {
                                    if (CurrentHardwareCode != NewHardwareCode)
                                    {
                                        this.Result.Memo = "Activation fails";
                                        return this.Result;
                                    }
                                    if (CurrentHardwareCode != OldHardwareCode)
                                    {
                                        this.Result.Memo = "Activation fails";
                                        return this.Result;
                                    }
                                }
                            }
                        }
                        ProfileObject.ActivationCode = Tools.AES_Encrypt(Tools.Implode("_", DeCodeArr), 3); // 解密
                        var JsonString = JsonTools.ProfileToString(ProfileObject); // 配置项转为json格式
                        var Profile = Tools.RootPath() + "Profile.json"; // 查看配置文件
                        JsonTools.StringToFile(Profile, JsonString); // 写入文件

                        this.Result.State = true;
                        var NumberOfAccounts = Tools.StrToInt32(DeCodeArr[2]) + 5;
                        this.Result.Memo = "The system has opened " + NumberOfAccounts.ToString() + " accounts";
                        this.Result.ID = Tools.StrToInt32(NumberOfAccounts.ToString());
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity AccountNumberStatistics(string Token, int TokenType)
        {
            Models.CommonDataModel Result = new();
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
                    var UserCount = this.UserSQLModel.CountUser(); // 统计已使用的账号
                    var ProfileObject = JsonTools.CheckProfile(Tools.RootPath() + "Profile.json");
                    if (ProfileObject.ActivationCode == "")
                    {
                        Result.Data = UserCount.ToString() + "_5";
                    }
                    else
                    {
                        var ActivationCodeString = Tools.AES_Decrypt(ProfileObject.ActivationCode, 3);
                        var AccountNumber = (Tools.StrToInt32(Tools.Explode("_", ActivationCodeString)[2]) + 5).ToString();
                        Result.Data = UserCount.ToString() + "_" + AccountNumber;
                    }
                    Result.State = true;
                    Result.Memo = "Success";
                }
            }
            return Result;
        }
    }
}
