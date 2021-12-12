using System;
using Models;
using Service;

namespace Logic
{
    public class DepartmentLogic : Base
    {
        public DepartmentLogic(string IP, DbContentCore DbContent) : base(IP, DbContent) { }

        public Entity.CommonResultEntity CreateDepartment(string Token, int TokenType, Entity.DepartmentEntity Data)
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
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DepartmentModel.FindByName(Data.DepartmentName, Data.ParentID);
                    if (Info.ID != 0)
                    {
                        this.Result.Memo = "DepartmentName is exists";
                    }
                    else
                    {
                        if (Data.DepartmentName == "")
                        {
                            this.Result.Memo = "DepartmentName error";
                        }
                        else if (Data.DepartmentName.Length < 2)
                        {
                            this.Result.Memo = "DepartmentName length error";
                        }
                        else if (!Tools.RegCheckPro(Data.DepartmentName))
                        {
                            this.Result.Memo = "DepartmentName format error";
                        }
                        else if (Data.ParentID < 0)
                        {
                            this.Result.Memo = "ParentID error";
                        }
                        else
                        {
                            if (Data.ParentID > 0)
                            {
                                var ParentInfo = this.DepartmentModel.Find(Data.ParentID);
                                if (ParentInfo.ID == 0)
                                {
                                    this.Result.Memo = "Parent Department is not exists";
                                    return Result;
                                }
                            }

                            Entity.DepartmentEntity DepartmentData = new();
                            DepartmentData.DepartmentName = Data.DepartmentName;
                            DepartmentData.ParentID = Data.ParentID;
                            DepartmentData.State = 1;
                            DepartmentData.Createtime = Tools.Time32();
                            try
                            {
                                this.DepartmentModel.Insert(DepartmentData);
                                this.DbContent.SaveChanges();
                                this.Result.ID = DepartmentData.ID;
                                this.Result.Memo = "Success";
                                this.Result.State = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                this.Result.Memo = "Create error";
                            }
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteDepartment(string Token, int TokenType, int ID)
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
                    var DepartmentInfo = this.DepartmentModel.Find(ID);
                    if (DepartmentInfo.ID > 0)
                    {
                        var TA = this.BeginTransaction();
                        try
                        {
                            this.DepartmentFileModel.DeleteByDepartmentID(DepartmentInfo.ID);
                            this.DbContent.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            TA.Rollback();
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Delete error";
                        }

                        var Result = this.DeleteDepartmentInRecursion(ID);
                        if (!Result)
                        {
                            TA.Rollback();
                            this.Result.Memo = "Delete error";
                        }
                        else
                        {
                            TA.Commit();
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                    }
                    else
                    {
                        this.Result.Memo = "Department is not exists";
                    }
                }
            }

            return this.Result;
        }

        public bool DeleteDepartmentInRecursion(int ID)
        {
            // 获取部门下的所有人员 并设置为无部门状态
            Entity.UserSelectParamEntity UserSelectParam = new();
            UserSelectParam.DepartmentID = ID;
            var UserList = this.UserModel.Select(UserSelectParam);
            if (UserList.Count > 0)
            {
                try
                {
                    for (var i = 0; i < UserList.Count; i++)
                    {
                        Entity.UserEntity Data = new();
                        Data.DepartmentID = 0;
                        Data.Admin = 1;
                        this.UserModel.Modify(UserList[i].ID, Data);
                    }
                    this.DbContent.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            // 子部门
            Entity.DepartmentSelectParamEntity SubData = new();
            SubData.ParentID = ID;
            var SubList = this.DepartmentModel.Select(SubData);
            var SubState = true;
            if (SubList.Count > 0)
            {
                for (var i = 0; i < SubList.Count; i++)
                {
                    if (!DeleteDepartmentInRecursion(SubList[i].ID))
                    {
                        SubState = false;
                        break;
                    }
                }
            }

            if (!SubState)
            {
                return false;
            }

            try
            {
                this.DepartmentModel.Delete(ID);
                this.DbContent.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Entity.CommonResultEntity ToggleDepartment(string Token, int TokenType, int ID)
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
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DepartmentModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        if (Info.State == 1)
                        {
                            Info.State = 2;
                        }
                        else
                        {
                            Info.State = 1;
                        }
                        try
                        {
                            this.DepartmentModel.Modify(ID, Info);
                            this.DbContent.SaveChanges();
                            this.Result.State = true;
                            this.Result.Memo = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            this.Result.Memo = "Toggle error";
                        }
                    }
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity ModifyDepartment(string Token, int TokenType, int ID, Entity.DepartmentEntity Data)
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
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    var Info = this.DepartmentModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        if (Data.DepartmentName == "")
                        {
                            this.Result.Memo = "DepartmentName error";
                        }
                        else if (Data.ParentID < 0)
                        {
                            this.Result.Memo = "ParentID error";
                        }
                        else
                        {
                            if (Data.ParentID > 0)
                            {
                                var ParentInfo = this.DepartmentModel.Find(Data.ParentID);
                                if (ParentInfo.ID == 0)
                                {
                                    this.Result.Memo = "Parent Department is not exists";
                                    return this.Result;
                                }
                            }

                            if (Info.DepartmentName != Data.DepartmentName || Info.ParentID != Data.ParentID)
                            {
                                var CheckInfo = this.DepartmentModel.FindByName(Data.DepartmentName, Data.ParentID);
                                if (CheckInfo.ID != 0)
                                {
                                    this.Result.Memo = "Department is exists";
                                    return this.Result;
                                }
                            }

                            Info.DepartmentName = Data.DepartmentName;
                            Info.ParentID = Data.ParentID;
                            try
                            {
                                this.DepartmentModel.Modify(ID, Info);
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
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DepartmentInfo(string Token, int TokenType, int ID)
        {
            Entity.DepartmentEntity Data = new();
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
                    Result.Data = this.DepartmentModel.Find(ID);
                    Result.State = true;
                    Result.Memo = "success";
                }
            }
            return Result;
        }

        public Entity.CommonResultEntity SelectDepartment(string Token, int TokenType, Entity.DepartmentSelectParamEntity Data)
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
                    this.Result.Memo = "Token lost";
                }
                else if (!this.MasterVerify(UserID))
                {
                    this.Result.Memo = "Permission denied";
                }
                else
                {
                    this.Result.State = true;
                    this.Result.Memo = "Success";
                    this.Result.Data = this.DepartmentModel.Select(Data);
                }
            }
            return this.Result;
        }

        public Entity.CommonResultEntity CreateDepartmentExtra(string Token, int TokenType, Entity.DepartmentExtraEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                this.Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                this.Result.Memo = "TokenType error";
            }
            else if (Data.DepartmentID <= 0)
            {
                this.Result.Memo = "DepartmentID error";
            }
            else if (Data.ExtraDesc == "")
            {
                this.Result.Memo = "ExtraDesc error";
            }
            else if (Data.ExtraType <= 0)
            {
                this.Result.Memo = "ExtraType error";
            }
            else if (Data.ExtraValue == "")
            {
                this.Result.Memo = "ExtraValue error";
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
                    var DepartmentInfo = this.DepartmentModel.Find(Data.DepartmentID);
                    if (DepartmentInfo.ID == 0)
                    {
                        this.Result.Memo = "Department not found";
                    }
                    else
                    {
                        Entity.DepartmentExtraEntity DepartmentExtraData = new();
                        DepartmentExtraData.DepartmentID = Data.DepartmentID;
                        DepartmentExtraData.ExtraDesc = Data.ExtraDesc;
                        DepartmentExtraData.ExtraType = Data.ExtraType;
                        DepartmentExtraData.ExtraValue = Data.ExtraValue;
                        try
                        {
                            this.DepartmentExtraModel.Insert(DepartmentExtraData);
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
            }
            return this.Result;
        }

        public Entity.CommonResultEntity DeleteDepartmentExtra(string Token, int TokenType, int ID)
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
                    var Info = this.DepartmentExtraModel.Find(ID);
                    if (Info.ID == 0)
                    {
                        this.Result.Memo = "Data not found";
                    }
                    else
                    {
                        try
                        {
                            this.DepartmentExtraModel.Delete(ID);
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

        public Entity.CommonResultEntity SelectDepartmentExtra(string Token, int TokenType, Entity.DepartmentExtraSelectParamEntity Data)
        {
            if (String.IsNullOrEmpty(Token))
            {
                Result.Memo = "Token error";
            }
            else if (TokenType <= 0)
            {
                Result.Memo = "TokenType error";
            }
            else if (Data.DepartmentID < 0)
            {
                Result.Memo = "DepartmentID error";
            }
            else if (Data.ExtraType < 0)
            {
                Result.Memo = "ExtraType error";
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
                    Result.State = true;
                    Result.Memo = "Success";
                    Result.Data = this.DepartmentExtraModel.Select(Data);
                }
            }
            return this.Result;
        }
    }
}
