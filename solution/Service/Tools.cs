using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace Service
{
    public static class Tools
    {
        private static readonly string AES_key1 = "Fuck.Peace&Love!"; // 秘钥1
        private static readonly string AES_key2 = "Fuck.Love&Peace!"; // 秘钥2
        private static readonly string AES_key3 = "<HAKUNA.MATATA!>"; // 秘钥3

        public static void WarningConsole(string Content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Content);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void CorrectConsole(string Content)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Content);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 按指定符号把字符串转为数组
        /// </summary>
        /// <param name="KeyStr"></param>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string[] Explode(string KeyStr, string Param)
        {
            return Param.Split(KeyStr);
        }

        /// <summary>
        /// 按指定符号把数组转为字符串
        /// </summary>
        /// <param name="KeyStr"></param>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string Implode(string KeyStr, string[] Param)
        {
            return string.Join(KeyStr, Param);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static bool DirIsExists(string DirPath)
        {
            return Directory.Exists(DirPath);
        }

        /// <summary>
        /// 获取当前模块路径
        /// </summary>
        /// <returns></returns>
        public static string CurrentPath()
        {
            return (Directory.GetCurrentDirectory() + "/").Replace("\\", "/");
        }

        /// <summary>
        /// 获取当前程序根目录
        /// </summary>
        /// <returns></returns>
        public static string RootPath()
        {
            return (Directory.GetParent("../") + "/").Replace("\\", "/");
        }

        /// <summary>
        /// 用户根目录
        /// </summary>
        /// <returns></returns>
        public static string UserBaseDir()
        {
            return (Directory.GetParent("../") + "/Matrix/").Replace("\\", "/");
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="DirName"></param>
        /// <returns></returns>
        public static bool MKDir(string Path, string DirName)
        {
            Path = Path.Replace(@"\", "/");
            string DirPath = Path + "/" + DirName;
            if (!Directory.Exists(DirPath))
            {
                try
                {
                    Directory.CreateDirectory(DirPath);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static bool CreateDir(string DirPath)
        {
            DirPath = DirPath.Replace(@"\", "/");
            if (!Directory.Exists(DirPath))
            {
                try
                {
                    Directory.CreateDirectory(DirPath);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取文件夹下的文件
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static string[] SelectAllFile(string DirPath)
        {
            DirPath = DirPath.Replace(@"\", "/");
            if (Directory.Exists(DirPath))
            {
                string[] Result = Directory.GetFiles(DirPath, "*.*");
                for (int i = 0; i < Result.Length; i++)
                {
                    Result[i] = Result[i].Replace(@"\", "/");
                }
                return Result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取文件夹下所有子文件夹
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static string[] SelectAllDir(string DirPath)
        {
            DirPath = DirPath.Replace(@"\", "/");
            if (Directory.Exists(DirPath))
            {
                string[] Result = Directory.GetDirectories(DirPath, "*.*");
                for (int i = 0; i < Result.Length; i++)
                {
                    Result[i] = Result[i].Replace(@"\", "/");
                }
                return Result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="DirPath"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        public static bool RenameDir(string DirPath, string NewName)
        {
            string DirPathRe = DirPath.Replace("\\", "/");
            string[] DirPathSub = Explode("/", DirPathRe);
            DirPathSub[DirPathSub.Length - 1] = NewName;
            string NewDirPath = Implode("/", DirPathSub);
            if (Directory.Exists(DirPath) && !Directory.Exists(NewDirPath))
            {
                try
                {
                    Directory.Move(DirPath, NewDirPath);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestPath"></param>
        /// <param name="overwriteexisting"></param>
        /// <returns></returns>
        public static bool CopyDir(string SourcePath, string DestPath, bool overwriteexisting = false)
        {
            SourcePath = SourcePath.Replace(@"\", "/");
            DestPath = DestPath.Replace(@"\", "/");
            string[] SourcePathArr = Explode("/", SourcePath);
            string DestinationPath = DestPath + "/" + SourcePathArr[SourcePathArr.Length - 1];
            if (Directory.Exists(DestinationPath))
            {
                return false;
            }
            try
            {
                SourcePath = SourcePath.EndsWith("/") ? SourcePath : SourcePath + "/";
                DestinationPath = DestinationPath.EndsWith("/") ? DestinationPath : DestinationPath + "/";
                if (Directory.Exists(SourcePath))
                {
                    if (!Directory.Exists(DestinationPath))
                    {
                        Directory.CreateDirectory(DestinationPath);
                    }
                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new(fls);
                        try
                        {
                            flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return false;
                        }
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new(drs);
                        if (CopyDir(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="SourceDirectory"></param>
        /// <param name="TargetDirectory"></param>
        /// <returns></returns>
        public static bool DirectoryCopy(string SourceDirectory, string TargetDirectory)
        {
            try
            {
                DirectoryInfo Dir = new(SourceDirectory);
                //获取目录下（不包含子目录）的文件和子目录
                var FileInfo = Dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in FileInfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(TargetDirectory + "/" + i.Name))
                        {
                            //目标目录下不存在此文件夹即创建子文件夹
                            Directory.CreateDirectory(TargetDirectory + "/" + i.Name);
                        }
                        //递归调用复制子文件夹
                        if (!DirectoryCopy(i.FullName, TargetDirectory + "/" + i.Name))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //不是文件夹即复制文件，true表示可以覆盖同名文件
                        File.Copy(i.FullName, TargetDirectory + "/" + i.Name, true);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="DirPath"></param>
        /// <param name="DelAll"></param>
        /// <returns></returns>
        public static bool DelDir(string DirPath, bool DelAll = false)
        {
            if (Directory.Exists(DirPath))
            {
                try
                {
                    Directory.Delete(DirPath, DelAll);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <returns></returns>
        public static bool ClearDir(string DirPath)
        {
            if (!Directory.Exists(DirPath)) { return false; }
            DirectoryInfo Dir = new(DirPath);
            FileSystemInfo[] SubDir = Dir.GetFileSystemInfos(); // 返回目录中所有文件和子目录
            if (SubDir.Length == 0) { return true; }
            foreach (var Item in SubDir)
            {
                if (Item is DirectoryInfo)
                {
                    DirectoryInfo _Item = new(Item.FullName);
                    try
                    {
                        _Item.Delete(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(Item.FullName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FileMD5(string FilePath, int bufferSize = 128)
        {
            if (!File.Exists(FilePath)) { return ""; }
            int BufferSize = 1024 * bufferSize; // 自定义缓冲区大小16K
            byte[] Buffer = new byte[BufferSize];
            Stream InputStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            HashAlgorithm HashAlgorithm = new MD5CryptoServiceProvider();
            int ReadLength = 0; // 每次读取长度
            var output = new byte[BufferSize];
            while ((ReadLength = InputStream.Read(Buffer, 0, Buffer.Length)) > 0)
            {
                HashAlgorithm.TransformBlock(Buffer, 0, ReadLength, output, 0); // 计算MD5
            }
            // 完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)
            HashAlgorithm.TransformFinalBlock(Buffer, 0, 0);
            string MD5 = BitConverter.ToString(HashAlgorithm.Hash).Replace("-", "");
            HashAlgorithm.Clear();
            InputStream.Close();
            return MD5;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool FileIsExists(string FilePath)
        {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool MKFile(string Path, string FileName)
        {
            Path = Path.Replace(@"\", "/");
            string FilePath = Path + "/" + FileName;
            if (!File.Exists(FilePath))
            {
                try
                {
                    StreamWriter FC = File.CreateText(FilePath);
                    FC.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool CreateFile(string FilePath)
        {
            FilePath = FilePath.Replace(@"\", "/");
            if (!File.Exists(FilePath))
            {
                try
                {
                    StreamWriter FC = File.CreateText(FilePath);
                    FC.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FileType(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return Path.GetExtension(FilePath);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 文件信息
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo FileInfo(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                FileInfo FileObject = new(FilePath);
                return FileObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 文件名(不带扩展名)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FileName(string FilePath)
        {
            return Path.GetFileNameWithoutExtension(FilePath.Trim()).Trim();
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        //public static bool RenameFile(string OldFilePath, string NewName)
        //{
        //    if (!File.Exists(OldFilePath))
        //    {
        //        return false;
        //    }
        //    OldFilePath = OldFilePath.Replace(@"\", "/");
        //    string[] OldFilePathArr = Explode("/", OldFilePath);
        //    string OldFileName = OldFilePathArr[OldFilePathArr.Length - 1];
        //    string[] OldFileNameArr = Explode(".", OldFileName);
        //    string NewFileName = NewName + "." + OldFileNameArr[1];
        //    OldFilePathArr[OldFilePathArr.Length - 1] = NewFileName;
        //    string NewFilePath = Implode("/", OldFilePathArr);
        //    if (File.Exists(NewFilePath))
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        File.Move(OldFilePath, NewFilePath);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return false;
        //    }
        //}
        public static bool RenameFile(string OldFilePath, string NewName)
        {
            if (!File.Exists(OldFilePath))
            {
                return false;
            }
            try
            {
                string OldFullPath = Path.GetDirectoryName(OldFilePath); // 获取当前文件全路径
                string NewFilePath = Path.Combine(OldFullPath, NewName); // 组合新文件路径
                FileInfo fileInfo = new(OldFilePath);
                fileInfo.MoveTo(NewFilePath); // 修改文件名称
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 复制文件到指定文件夹路径
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="DestPath"></param>
        /// <returns></returns>
        public static bool CopyFile(string FilePath, string DestPath)
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }
            else if (!Directory.Exists(DestPath))
            {
                return false;
            }
            else
            {
                FilePath = FilePath.Replace(@"\", "/");
                DestPath = DestPath.Replace(@"\", "/");
                string[] FilePathArr = Explode("/", FilePath);
                string FileName = FilePathArr[FilePathArr.Length - 1];
                string DestFilePath = DestPath + FileName;
                if (File.Exists(DestFilePath))
                {
                    return false;
                }
                else
                {
                    try
                    {
                        File.Copy(FilePath, DestFilePath);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Content"></param>
        /// <param name="AppendState"></param>
        /// <returns></returns>
        public static bool WriteFile(string FilePath, string Content, bool AppendState = false)
        {
            try
            {
                using StreamWriter FileWriter = new(FilePath, append: AppendState);
                FileWriter.WriteLine(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 以UTF8编码写入文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static bool WriteFileUTF8(string FilePath, string Content)
        {
            try
            {
                File.WriteAllText(FilePath, Content, new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 文件读取
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string ReadFile(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                StreamReader SR = File.OpenText(FilePath);
                string Content = SR.ReadToEnd();
                SR.Close();
                return Content;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool DelFile(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 上传到外网
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static IFormFile ToFormFileUpload(string FilePath)
        {
            using var FStream = new FileStream(FilePath, FileMode.Open);
            var MStream = new MemoryStream();
            FStream.CopyTo(MStream);
            return new FormFile(MStream, 0, MStream.Length, null, FStream.Name) { Headers = new HeaderDictionary(), ContentType = "application/" + Path.GetExtension(FilePath), };
        }

        /// <summary>
        /// 上传到服务器处理
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static IFormFile ToFormFileAnalysis(string FilePath)
        {
            var FileBytes = File.ReadAllBytes(FilePath);
            var FileMS = new MemoryStream(FileBytes) { Position = 0 };
            IFormFile _FormFile = new FormFile(FileMS, 0, FileMS.Length, Path.GetFileNameWithoutExtension(FilePath), Path.GetFileName(FilePath));
            return _FormFile;
        }

        /// <summary>
        /// 获取http请求 Headers 文件名
        /// </summary>
        /// <param name="Response"></param>
        /// <returns></returns>
        public static string CheckHttpFileName(HttpWebResponse Response)
        {
            var FileName = Response.Headers["Content-Disposition"];
            if (String.IsNullOrEmpty(FileName))
            {
                FileName = Response.ResponseUri.Segments[^1];
            }
            return FileName.Trim();
        }

        /// <summary>
        /// 文件切片
        /// </summary>
        /// <param name="ResourcePath"></param>
        /// <param name="TargetPath"></param>
        /// <param name="BlockSize"></param>
        /// <returns></returns>
        public static bool FileSlice(string ResourcePath, string TargetPath, int BlockSize, bool CleanUpTraces = false)
        {
            if (String.IsNullOrEmpty(ResourcePath)) { return false; }
            else if (String.IsNullOrEmpty(TargetPath)) { return false; }
            else if (BlockSize <= 0) { return false; }
            else if (!FileIsExists(ResourcePath)) { return false; }
            else if (!DirIsExists(TargetPath)) { return false; }
            else
            {
                try
                {
                    FileInfo _fileInfo = new(ResourcePath);
                    string FileName = _fileInfo.Name.Replace(_fileInfo.Extension, "");
                    FileStream FSRead = new(ResourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryReader BR = new(FSRead);
                    byte[] Buffer = new byte[BlockSize];
                    int FileIndex = 1;
                    long FileLength = _fileInfo.Length;
                    long ReadFileLength = 0;
                    while (ReadFileLength < FileLength)
                    {
                        var PartStr = "";
                        if (FileIndex > 0 && FileIndex < 10)
                        {
                            PartStr = "part.00";
                        }
                        else if (FileIndex >= 10 && FileIndex < 100)
                        {
                            PartStr = "part.0";
                        }
                        else
                        {
                            PartStr = "part.";
                        }
                        string WriteFile = Path.Combine(TargetPath, PartStr + FileIndex.ToString());
                        FileStream FSWrite = new(WriteFile, FileMode.CreateNew, FileAccess.Write);
                        BinaryWriter BW = new(FSWrite);
                        long singleFileLength = 0;
                        int ReadLength;
                        while ((ReadLength = BR.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            BW.Write(Buffer, 0, ReadLength);
                            ReadFileLength += ReadLength;
                            singleFileLength += ReadLength;
                            if (singleFileLength >= BlockSize)
                            {
                                BW?.Close();
                                BW?.Dispose();
                                FSWrite?.Close();
                                FSWrite?.Dispose();
                                break;
                            }
                        }
                        BW?.Close();
                        BW?.Dispose();
                        FSWrite?.Close();
                        FSWrite?.Dispose();
                        FileIndex++;
                    }
                    BR?.Close();
                    BR?.Dispose();
                    FSRead?.Close();
                    FSRead.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取毫秒时间戳
        /// </summary>
        /// <returns></returns>
        public static long TimeMS()
        {
            TimeSpan TS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(TS.TotalMilliseconds);
        }

        /// <summary>
        /// 64位时间戳
        /// </summary>
        /// <returns></returns>
        public static long Time()
        {
            TimeSpan TS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(TS.TotalSeconds);
        }

        /// <summary>
        /// 32位时间戳
        /// </summary>
        /// <returns></returns>
        public static int Time32()
        {
            TimeSpan TS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt32(TS.TotalSeconds);
        }

        /// <summary>
        /// 时间戳转日期
        /// </summary>
        /// <param name="TS"></param>
        /// <returns></returns>
        public static string TimeToStr(long TS)
        {
            long unixTimeStamp = TS * 1000;
            DateTime start = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 日期转时间戳
        /// </summary>
        /// <param name="TS"></param>
        /// <returns></returns>
        public static long StrToTime(string TS)
        {
            DateTime DTime = DateTime.Parse(TS.Trim());
            DateTimeOffset DTO = new(DTime);
            return DTO.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取N年后的时间戳
        /// </summary>
        /// <param name="ActTime"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static long GetTimeStampYear(string ActTime, int Year)
        {
            var Time = Convert.ToDateTime(ActTime).AddYears(Year);
            //var Time = DateTime.Now.AddYears(Year);
            return StrToTime(Time.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 获取N天后的时间戳
        /// </summary>
        /// <param name="ActTime"></param>
        /// <param name="Day"></param>
        /// <returns></returns>
        public static long GetTimeStampDay(string ActTime, int Day)
        {
            var Time = Convert.ToDateTime(ActTime).AddDays(Day);
            //var Time = DateTime.Now.AddYears(Year);
            return StrToTime(Time.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="ActTime"></param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(long ActTime)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            TimeSpan toNow = new TimeSpan(ActTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="DT"></param>
        /// <returns></returns>
        public static long DateTimeToTimeStamp(DateTime DT)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (DT.Ticks - dt1970.Ticks) / 10000000;
        }

        /// <summary>
        /// 获取当前年份
        /// </summary>
        /// <returns></returns>
        public static string NowYear()
        {
            return DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// 获取当前月份
        /// </summary>
        /// <returns></returns>
        public static string NowMonth()
        {
            return DateTime.Now.Month.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// 获取当前日期
        /// </summary>
        /// <returns></returns>
        public static string NowDay()
        {
            return DateTime.Now.Day.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="EncypStr"></param>
        /// <returns></returns>
        public static string MD5(string EncypStr)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new();
            byte[] emailBytes = Encoding.UTF8.GetBytes(EncypStr.ToLower());
            byte[] hashedEmailBytes = md5.ComputeHash(emailBytes);
            StringBuilder sb = new();
            foreach (var b in hashedEmailBytes)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            EncypStr = sb.ToString();
            return EncypStr;
        }

        public static string UserToken(string UserID, string UserName)
        {
            return MD5(UserID + TimeToStr(Time()).Remove(0, 5) + UserName);
        }

        /// <summary>
        /// 系统用户密码加密方式
        /// </summary>
        /// <param name="PWD"></param>
        /// <param name="Secret"></param>
        /// <returns></returns>
        public static string UserPWD(string PWD, string Secret)
        {
            return MD5(MD5(PWD + Secret) + Secret);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Random(int n = 4)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            Random random = new(iSeed);
            int ResultINT = random.Next();
            string ResultStr = ResultINT.ToString();
            ResultStr = ResultStr.Substring(0, n);
            ResultINT = Convert.ToInt32(ResultStr);
            return ResultINT;
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string RandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = null)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;

            if (useNum == true)
            {
                str += "0123456789";
            }

            if (useLow == true)
            {
                str += "abcdefghijklmnopqrstuvwxyz";
            }

            if (useUpp == true)
            {
                str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (useSpe == true)
            {
                str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }

            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }

            return s;
        }

        /// <summary>
        /// 列表乱序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Sources"></param>
        /// <returns></returns>
        public static List<T> ListRandom<T>(List<T> Sources)
        {
            var Random = new Random();
            var ResultList = new List<T>();
            foreach (var item in Sources)
            {
                ResultList.Insert(Random.Next(ResultList.Count), item);
            }
            return ResultList;
        }

        /// <summary>
        /// 数组中随机抽取元素
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int[] L)
        {
            Random rnd = new Random();
            int index = rnd.Next(L.Length);
            return L[index];
        }

        /// <summary>
        /// 数组中随机抽取指定数量的不同元素
        /// </summary>
        /// <param name="L"></param>
        /// <param name="Num"></param>
        /// <returns></returns>
        public static int[] GetRandomNumberRange(int[] L, int Num)
        {
            var s = new List<string>();
            foreach (var Item in L)
            {
                s.Add(Item.ToString());
            }
            var NewArr = string.Join(",", s.OrderBy(d => Guid.NewGuid()).Take(Num));
            var StrArr = NewArr.Split(",");
            var IntArr = new int[StrArr.Length];
            for (int i = 0; i < StrArr.Length; i++)
            {
                IntArr[i] = Convert.ToInt32(StrArr[i]);
            }
            return IntArr;
        }

        /// <summary>
        /// 字母和数字
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static bool RegCheck(string Param)
        {
            Regex Reg = new(@"^[A-Za-z0-9]+$");
            return Reg.IsMatch(Param);
        }

        /// <summary>
        /// 字母 数字 汉字 下划线 点
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static bool RegCheckPro(string Param)
        {
            Regex Reg = new(@"^[\u4E00-\u9FA5A-Za-z0-9_.]+$");
            return Reg.IsMatch(Param);
        }

        /// <summary>
        /// 数字
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static bool RegNum(string Param)
        {
            Regex Reg = new(@"^[0-9]*$");
            return Reg.IsMatch(Param);
        }

        /// <summary>
        /// 中文
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static bool RegChinese(string Param)
        {
            Regex Reg = new(@"^[\u4e00-\u9fa5]{0,}$");
            return Reg.IsMatch(Param);
        }

        /// <summary>
        /// 字符串转int32
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static int StrToInt32(string Param)
        {
            return Convert.ToInt32(Param);
        }

        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static long StrToInt(string Param)
        {
            return Convert.ToInt64(Param);
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string ByteToBase64(byte[] Param)
        {
            //if (Param == null || Param == "")
            //{
            //    return "";
            //}
            //byte[] bytes = Encoding.UTF8.GetBytes(Param);
            return Convert.ToBase64String(Param);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string Base64ToByte(byte[] Param)
        {
            //if (Param == null || Param == "")
            //{
            //    return "";
            //}
            //byte[] bytes = Convert.FromBase64String(Param);
            return Encoding.UTF8.GetString(Param);
        }

        /// <summary>
        /// 获取所有进程
        /// </summary>
        /// <returns></returns>
        public static Process[] GetProcesses()
        {
            Process[] Pro = Process.GetProcesses();
            return Pro;
        }

        /// <summary>
        /// 获取当前进程
        /// </summary>
        /// <returns></returns>
        public static Process GetCurrentProcess()
        {
            Process Pro = Process.GetCurrentProcess();
            return Pro;
        }

        /// <summary>
        /// 获取系统类型
        /// </summary>
        /// <returns></returns>
        public static string OSType()
        {
            var OSTypeData = "Unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OSTypeData = "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OSTypeData = "OSX";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OSTypeData = "Windows";
            }
            return OSTypeData;
        }

        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string SysShell(string Param1, string Param2)
        {
            var Result = "";
            if (Param1 != "" && Param2 != "")
            {
                // 创建一个ProcessStartInfo对象 使用系统shell 指定命令和参数 设置标准输出
                var PSI = new ProcessStartInfo(Param1, Param2) { RedirectStandardOutput = true };
                var PROC = Process.Start(PSI); // 启动
                if (PROC != null)
                {
                    var OutPut = PROC.StandardOutput;
                    while (!OutPut.EndOfStream)
                    {
                        Result = Result + OutPut.ReadLine();
                    }
                    if (!PROC.HasExited)
                    {
                        PROC.Kill();
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// AES Encode
        /// </summary>
        /// <param name="EncryptStr"></param>
        /// <param name="AESType"></param>
        /// <returns></returns>
        public static string AES_Encrypt(string EncryptStr, int AESType = 1)
        {
            var AES_key = "";
            if (AESType == 1)
            {
                AES_key = AES_key1;
            }
            else if (AESType == 2)
            {
                AES_key = AES_key2;
            }
            else if (AESType == 3)
            {
                AES_key = AES_key3;
            }
            else
            {
                return AES_key;
            }

            if (string.IsNullOrEmpty(EncryptStr)) return "";
            try
            {
                Byte[] ToEncryptArray = Encoding.UTF8.GetBytes(EncryptStr);
                RijndaelManaged RM = new()
                {
                    Key = Encoding.UTF8.GetBytes(AES_key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform CTransform = RM.CreateEncryptor();
                Byte[] ResultArray = CTransform.TransformFinalBlock(ToEncryptArray, 0, ToEncryptArray.Length);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(ResultArray, 0, ResultArray.Length)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return "";
            }
        }

        /// <summary>
        /// AES Decode
        /// </summary>
        /// <param name="DecryptStr"></param>
        /// <param name="AESType"></param>
        /// <returns></returns>
        public static string AES_Decrypt(string DecryptStr, int AESType = 1)
        {
            var AES_key = "";
            if (AESType == 1)
            {
                AES_key = AES_key1;
            }
            else if (AESType == 2)
            {
                AES_key = AES_key2;
            }
            else if (AESType == 3)
            {
                AES_key = AES_key3;
            }
            else
            {
                return AES_key;
            }

            if (string.IsNullOrEmpty(DecryptStr)) return "";
            try
            {
                Byte[] ToEncryptArray = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(DecryptStr)));
                RijndaelManaged RM = new()
                {
                    Key = Encoding.UTF8.GetBytes(AES_key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform CTransform = RM.CreateDecryptor();
                Byte[] ResultArray = CTransform.TransformFinalBlock(ToEncryptArray, 0, ToEncryptArray.Length);
                return Encoding.UTF8.GetString(ResultArray);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return "";
            }
        }

        /// <summary>
        /// Windows命令行
        /// </summary>
        /// <param name="CommandLine"></param>
        /// <returns></returns>
        public static string CMD(string CommandLine)
        {
            CommandLine = CommandLine.Trim().TrimStart('&') + "&exit";//&执行两条命令的标识，这里第二条命令的目的是当调用ReadToEnd()方法是，不会出现假死状态
            string OutputMsg = "";
            Process pro = new();
            pro.StartInfo.FileName = "cmd.exe"; // 调用cmd.exe
            pro.StartInfo.UseShellExecute = false; // 是否启用shell启动进程
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true; // 重定向的设置
            pro.StartInfo.CreateNoWindow = true; // 不创建窗口
            pro.Start();
            pro.StandardInput.WriteLine(CommandLine); // 执行cmd语句
            pro.StandardInput.AutoFlush = true;
            OutputMsg += pro.StandardOutput.ReadToEnd(); //读取返回信息
            //OutputMsg=OutputMsg.Substring(OutputMsg.IndexOf(commandLine)+commandLine.Length);//返回发送命令之后的信息
            pro.WaitForExit(); //等待程序执行完退出，不过感觉不用这条命令，也可以达到同样的效果
            pro.Close();
            return OutputMsg;
        }

        /// <summary>
        /// Linux命令行
        /// </summary>
        /// <param name="CommandLine"></param>
        /// <returns></returns>
        public static string Shell(string CommandLine)
        {
            var CommandFile = "../Wrapper.sh";
            if (FileIsExists(CommandFile))
                DelFile(CommandFile);
            if (!CreateFile(CommandFile))
                return "error";
            if (!WriteFileUTF8(CommandFile, CommandLine))
                return "error";
            try
            {
                var pro = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "bash",
                        Arguments = CommandFile,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                pro.Start();
                string OutputMsg = pro.StandardOutput.ReadToEnd();
                string ErrorMsg = pro.StandardError.ReadToEnd();
                pro.WaitForExit();
                DelFile(CommandFile);
                if (String.IsNullOrEmpty(ErrorMsg))
                    return OutputMsg;
                else
                    return ErrorMsg;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DelFile(CommandFile);
                return e.Message;
            }
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        /// <returns></returns>
        public static string LocalIP()
        {
            string HostName = Dns.GetHostName();
            var IPAddrList = Dns.GetHostAddresses(HostName);
            var Result = "";
            foreach (IPAddress IP in IPAddrList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    Result = IP.ToString();
                }
            }
            return Result;
        }

        /// <summary>
        /// 本机CPU核心数
        /// </summary>
        /// <returns></returns>
        public static int EP()
        {
            return Environment.ProcessorCount;
        }

        /// <summary>
        /// 发送Post
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="PostDict"></param>
        /// <returns></returns>
        public static string PostHelper(string URL, Dictionary<string, string> PostDict)
        {
            string Result = "";
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(URL);
            Req.Method = "POST";
            Req.ContentType = "application/x-www-form-urlencoded";
            StringBuilder Builder = new();
            int i = 0;
            foreach (var item in PostDict)
            {
                if (i > 0)
                    Builder.Append('&');
                Builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] Data = Encoding.UTF8.GetBytes(Builder.ToString());
            Req.ContentLength = Data.Length;
            using (Stream ReqStream = Req.GetRequestStream())
            {
                ReqStream.Write(Data, 0, Data.Length);
                ReqStream.Close();
            }
            HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
            Stream Stm = Resp.GetResponseStream();
            //获取响应内容
            using (StreamReader Reader = new(Stm, Encoding.UTF8))
            {
                Result = Reader.ReadToEnd();
            }
            return Result;
        }

        /// <summary>
        /// 发送Get
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string GetHelper(string URL)
        {
            string Result = "";
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
            Stream Stm = Resp.GetResponseStream();
            try
            {
                using StreamReader Reader = new StreamReader(Stm);
                Result = Reader.ReadToEnd();
            }
            finally
            {
                Stm.Close();
            }
            return Result;
        }
    }

    public class FormFile : IFormFile
    {
        public string ContentDisposition { get { throw null; } set { } }
        public string ContentType { get { throw null; } set { } }
        public string FileName
        {
            [CompilerGenerated]
            get { throw null; }
        }
        public IHeaderDictionary Headers
        {
            [CompilerGenerated]
            get { throw null; }
            [CompilerGenerated]
            set { }
        }
        public long Length
        {
            [CompilerGenerated]
            get { throw null; }
        }
        public string Name
        {
            [CompilerGenerated]
            get { throw null; }
        }
        public FormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName) { }
        public void CopyTo(Stream target) { }
        [DebuggerStepThrough]
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken)) { throw null; }
        public Stream OpenReadStream() { throw null; }
    }

    public class HeaderDictionary : IHeaderDictionary, ICollection<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable, IDictionary<string, StringValues>
    {
        public struct Enumerator : IEnumerator<KeyValuePair<string, StringValues>>, IEnumerator, IDisposable
        {
            //private object _dummy;
            //private int _dummyPrimitive;
            public KeyValuePair<string, StringValues> Current { get { throw null; } }
            object IEnumerator.Current { get { throw null; } }
            public void Dispose() { }
            public bool MoveNext() { throw null; }
            void IEnumerator.Reset() { }
        }
        public long? ContentLength { get { throw null; } set { } }
        public int Count { get { throw null; } }
        public bool IsReadOnly
        {
            [CompilerGenerated]
            get { throw null; }
            [CompilerGenerated]
            set { }
        }
        public StringValues this[string key] { get { throw null; } set { } }
        public ICollection<string> Keys { get { throw null; } }
        StringValues IDictionary<string, StringValues>.this[string key] { get { throw null; } set { } }
        public ICollection<StringValues> Values { get { throw null; } }
        public HeaderDictionary() { }
        public HeaderDictionary(Dictionary<string, StringValues> store) { }
        public HeaderDictionary(int capacity) { }
        public void Add(KeyValuePair<string, StringValues> item) { }
        public void Add(string key, StringValues value) { }
        public void Clear() { }
        public bool Contains(KeyValuePair<string, StringValues> item) { throw null; }
        public bool ContainsKey(string key) { throw null; }
        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex) { }
        public Enumerator GetEnumerator() { throw null; }
        public bool Remove(KeyValuePair<string, StringValues> item) { throw null; }
        public bool Remove(string key) { throw null; }
        IEnumerator<KeyValuePair<string, StringValues>> IEnumerable<KeyValuePair<string, StringValues>>.GetEnumerator() { throw null; }
        IEnumerator IEnumerable.GetEnumerator() { throw null; }
        public bool TryGetValue(string key, out StringValues value) { throw null; }
    }

}
