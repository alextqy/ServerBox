using System;
using System.IO;
using Models;
using Service;

namespace Logic
{
    public class ConfigLogic : Base
    {
        public ConfigLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Entity.CommonResultEntity CheckConfig(string Token, int TokenType, int ID)
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
                else if (!this.MasterVerify(UserID))
                {
                    Result.Memo = "Permission denied";
                }
                else
                {
                    Result.Data = this.ConfigModel.Find(ID);
                    Result.State = true;
                    Result.Memo = "Success";
                }
            }

            return Result;
        }

        public Entity.CommonResultEntity ModifyConfig(string Token, int TokenType, int ID, Entity.ConfigEntity Data)
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
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    Entity.ConfigEntity ConfigData = new();
                    ConfigData.ConfigKey = Data.ConfigKey;
                    ConfigData.ConfigDesc = Data.ConfigDesc;
                    ConfigData.ConfigType = Data.ConfigType;
                    ConfigData.ConfigValue = Data.ConfigValue;
                    try
                    {
                        this.Result.State = true;
                        this.Result.Memo = "Success";
                        this.ConfigModel.Modify(ID, ConfigData);
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

        //public Entity.CommonResultEntity DecryptProInspection(string Token, int TokenType, string Code, int CodeType = 1)
        //{
        //    if (String.IsNullOrEmpty(Token))
        //    {
        //        Result.Memo = "Token error";
        //    }
        //    else if (TokenType <= 0)
        //    {
        //        Result.Memo = "TokenType error";
        //    }
        //    else if (Code == "")
        //    {
        //        Result.Memo = "Param error";
        //    }
        //    else if (CodeType <= 0)
        //    {
        //        Result.Memo = "CodeType error";
        //    }
        //    else
        //    {
        //        var UserID = this.TokenVerify(Token, TokenType);
        //        if (UserID == 0)
        //        {
        //            Result.Memo = "Token lost";
        //        }
        //        else
        //        {
        //            Result.State = true;
        //            Result.Memo = "Success";
        //            Result.Data = Tools.AES_Decrypt(Code, CodeType);
        //        }
        //    }
        //    return Result;
        //}

        //public Entity.CommonResultEntity GenerateActivationCodeInspection(string Token, int TokenType, string EncryptedCode, int NumberOfAccounts = 5)
        //{
        //    if (String.IsNullOrEmpty(Token))
        //    {
        //        Result.Memo = "Token error";
        //    }
        //    else if (TokenType <= 0)
        //    {
        //        Result.Memo = "TokenType error";
        //    }
        //    else if (EncryptedCode == "")
        //    {
        //        Result.Memo = "EncryptedCode error";
        //    }
        //    else if (NumberOfAccounts <= 0)
        //    {
        //        Result.Memo = "NumberOfAccounts error";
        //    }
        //    else if (NumberOfAccounts % 5 != 0)
        //    {
        //        Result.Memo = "NumberOfAccounts error";
        //    }
        //    else
        //    {
        //        var DeCode = Tools.AES_Decrypt(EncryptedCode, 1); // 解密
        //        var DeCodeArr = Tools.Explode("_", DeCode);
        //        DeCodeArr[2] = (Tools.StrToInt(DeCodeArr[2]) + NumberOfAccounts).ToString(); // 计算总账号数
        //        var EnCode = Tools.Implode("_", DeCodeArr);
        //        Result.Data = Tools.AES_Encrypt(EnCode, 2);
        //        Result.State = true;
        //        Result.Memo = "Success";
        //    }
        //    return Result;
        //}

        public Entity.CommonResultEntity GetHardwareCode(string Token, int TokenType)
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
                        var ActivationCode = ConfigHelper.AppSettingsHelper.ActivationCode();
                        if (ActivationCode != "")
                        {
                            var DeCode = Tools.AES_Decrypt(ActivationCode, 3);
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
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (EncryptedCode == "")
            {
                this.Result.Memo = "EncryptedCode error";
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
                    var DeCode = Tools.AES_Decrypt(EncryptedCode, 2); // 解密
                    if (DeCode != "")
                    {
                        var DeCodeArr = Tools.Explode("_", DeCode);
                        var ActivationCode = ConfigHelper.AppSettingsHelper.ActivationCode();
                        if (ActivationCode != "")
                        {
                            if (ActivationCode != EncryptedCode)
                            {
                                var NewHardwareCode = DeCodeArr[1];
                                var OldHardwareCode = Tools.Explode("_", Tools.AES_Decrypt(ActivationCode, 3))[1];
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
                        ActivationCode = Tools.AES_Encrypt(Tools.Implode("_", DeCodeArr), 3); // 解密
                        if (ConfigHelper.AppSettingsHelper.Activation(ActivationCode)) // 写入文件
                        {
                            this.Result.State = true;
                            var NumberOfAccounts = Tools.StrToInt32(DeCodeArr[2]) + 5;
                            this.Result.Memo = "The system has opened " + NumberOfAccounts.ToString() + " accounts";
                            this.Result.ID = Tools.StrToInt32(NumberOfAccounts.ToString());
                        }
                        else
                        {
                            this.Result.Memo = "Activation fails";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity AccountNumberStatistics(string Token, int TokenType)
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
                    var UserCount = this.UserModel.CountUser(); // 统计已使用的账号
                    var ActivationCode = ConfigHelper.AppSettingsHelper.ActivationCode();
                    if (ActivationCode == "")
                    {
                        Result.Data = UserCount.ToString() + "_5";
                    }
                    else
                    {
                        var ActivationCodeString = Tools.AES_Decrypt(ActivationCode, 3);
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
