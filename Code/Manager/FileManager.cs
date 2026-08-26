using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Submitter.Code.Component;

namespace Submitter
{
    internal class FileManager
    {
        internal string ip = "", user = "", password = "", name = "", savePath = "";
        internal void initSetting()
        {
            Displayer.output("初始化设置：", final: '\n');
            Displayer.output("\t输入服务器文件夹地址："); ip = Console.ReadLine();
            Displayer.output("\t输入服务器账号："); user = Console.ReadLine();
            Displayer.output("\t输入服务器密码："); password = Console.ReadLine();
            Displayer.output("\t输入你的名字："); name = Console.ReadLine();
            Displayer.output("\t输入下载文件的保存路径："); savePath = Console.ReadLine();
            if(File.Exists(Constant.settingPath)) File.Delete(Constant.settingPath);
            FileStream fs = File.Create(Constant.settingPath);
            IOManager.WriteLine(fs, ip);
            IOManager.WriteLine(fs, user);
            IOManager.WriteLine(fs, password);
            IOManager.WriteLine(fs, name);
            IOManager.WriteLine(fs, savePath);
            fs.Dispose();
        }
        bool loadSetting()
        {
            string content = IOManager.ReadAll(Constant.settingPath);
            string[] each = content.Split('\n');
            if (each.Length < 5) return false;
            ip = each[0]; user = each[1]; password = each[2]; name = each[3]; savePath = each[4];
            return true;
        }
        bool connectState(string ip, string user, string password)
        {
            bool flag = false;
            Process proc = new Process();
            Log.information("connect to : " + ip);
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + ip + " /user:" + user + " " + password + " /persistent:yes";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errorMsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errorMsg))
                {
                    flag = true;
                }
                else
                {
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                Log.warning(ex.Message);
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return flag;
        }
        public bool downloadFile(string targetLoacation, string path, string fileName)
        {
            if (!Directory.Exists(path))
                return false;
            path = path + '\\' + fileName;
            targetLoacation = targetLoacation + '\\' + fileName;
            FileStream inFileStream = new FileStream(path, FileMode.Open);
            FileStream outFileStream = new FileStream(targetLoacation, FileMode.OpenOrCreate);
            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
            {
                outFileStream.Write(buf, 0, byteCount);
            }
            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
            return true;
        }
        public void uploadFile(string src, string dst, string fileName)
        {
            FileStream inFileStream = new FileStream(src, FileMode.Open);
            if (!Directory.Exists(dst))
                Directory.CreateDirectory(dst);
            dst = dst + "\\" + fileName;
            FileStream outFileStream = new FileStream(dst, FileMode.OpenOrCreate);
            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
                outFileStream.Write(buf, 0, byteCount);
            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
        }
        public DirectoryInfo getDirectoryInfoFullPath(string path)
        {
            return new DirectoryInfo(path);
        }
        public DirectoryInfo getDirectoryInfoServer(string path)
        {
            return new DirectoryInfo(ip + '\\' + path);
        }
        public DirectoryInfo getDirectoryInfoLocal(string path)
        {
            return new DirectoryInfo(savePath + '\\' + path);
        }
        public bool directoryExistServer(string path)
        {
            if (Directory.Exists(ip + '\\' + path)) return true;
            return false;
        }
        public bool directoryExistLocal(string path)
        {
            if (Directory.Exists(savePath + '\\' + path)) return true;
            return false;
        }
        public bool directoryExistFullPath(string path)
        {
            if (Directory.Exists(path)) return true;
            return false;
        }
        public void createDirectoryServer(string path)
        {
            Directory.CreateDirectory(ip + '\\' + path);
        }
        public void createDirectoryLocal(string path)
        {
            Directory.CreateDirectory(savePath + '\\' + path);
        }
        public void createDirectoryFullPath(string path)
        {
            Directory.CreateDirectory(path);
        }
        internal void init()
        {
            if (!File.Exists(Constant.settingPath)) initSetting();
            Retry:
            if (!loadSetting())
            {
                Displayer.output("输入的配置无效，请重试" +"", textColor: ConsoleColor.Red, final: '\n');
                initSetting();
                goto Retry;
            }
            try
            {
                if (!connectState(ip, user, password))
                    Displayer.output("无法连接至对应路径......", textColor: ConsoleColor.Red, final: '\n');
                else
                    Displayer.output("连接成功", textColor: ConsoleColor.Green, final: '\n');
            }
            catch (Exception ex)
            {
                Displayer.output("因为如下原因，无法连接至对应路径......", textColor: ConsoleColor.Red, final: '\n');
                Displayer.output(ex.Message, textColor: ConsoleColor.Red, final: '\n');
            }
            Displayer.output("初始化完成", textColor: ConsoleColor.Green, final: '\n');
        }
    }
}