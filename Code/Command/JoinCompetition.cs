using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Component;
using Submitter.Code.Manager;

namespace Submitter.Code.Command
{
    internal class JoinCompetition
    {
        internal static List<string> problemName = new();
        static List<string> unableDownloadPath = new();
        static List<string> unableDownloadName = new();
        static List<bool> flag = new();
        static void checkDirectory(string parentPath, string x, FileSystemInfo[] info, FileManager file)
        {
            foreach (FileSystemInfo child in info)
            {
                if (child is DirectoryInfo)
                {
                    if (child.Name == "#submit") continue;
                    if (child.Name == "!solution") Displayer.output("题解已检测", textColor: ConsoleColor.Green, final: '\n');
                    else
                    {
                        if (!problemName.Contains(child.Name)) problemName.Add(child.Name);
                        Displayer.output("题目" + child.Name + "已检测", textColor: ConsoleColor.Green, final: '\n');
                    }
                    file.createDirectoryLocal(parentPath + "\\" + child.Name);
                    checkDirectory(parentPath + '\\' + child.Name, x, file.getDirectoryInfoFullPath(child.FullName).GetFileSystemInfos(), file);
                }
                else
                {
                    if (File.Exists(file.savePath + "\\" + parentPath + "\\" + child.Name)) continue;
                    try
                    {
                        Log.information("download files : " + parentPath + '\\' + child.Name);
                        Displayer.output("正在下载" + parentPath + '\\' + child.Name, textColor: ConsoleColor.Gray, final: '\n');
                        file.downloadFile(file.savePath + '\\' + parentPath, file.ip + '\\' + parentPath, child.Name);
                        Displayer.output(parentPath + '\\' + child.Name + "下载成功", textColor: ConsoleColor.Gray, final: '\n');
                    }
                    catch (Exception e)
                    {
                        Log.warning("download files fails : " + parentPath + '\\' + child.Name);
                        Log.warning("previous reason is as follows : " + e.Message);
                        unableDownloadPath.Add(parentPath);
                        unableDownloadName.Add(child.Name);
                        flag.Add(false);
                        Displayer.output(parentPath + '\\' + child.Name + "正在被使用，将在稍后重新尝试下载", textColor: ConsoleColor.DarkRed, final: '\n');
                    }
                }
            }
        }
        internal static void clear()
        {
            problemName.Clear();
            unableDownloadName.Clear();
            unableDownloadPath.Clear();
            flag.Clear();
        }
        internal static void redownload(string matchname, FileManager file, bool isInside = false)
        {
            if (matchname == "")
            {
                Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                return;
            }
            int cnt = 0, success_count = 0;
            if (!isInside && cnt == 0)
            {
                DirectoryInfo dir = file.getDirectoryInfoServer(matchname);
                FileSystemInfo[] info = dir.GetFileSystemInfos();
                checkDirectory(matchname, matchname, info, file);
            }
            while (cnt < 10 && unableDownloadName.Count > 0)
            {
                cnt++;
                Displayer.output("第" + cnt.ToString() + "次重新尝试开始", textColor: ConsoleColor.DarkGreen, final: '\n');
                for (int i = 0; i < unableDownloadName.Count; i++)
                {
                    if (flag[i]) continue;
                    try
                    {
                        Log.information("download files : " + unableDownloadPath[i] + '\\' + unableDownloadName[i]);
                        Displayer.output("正在下载" + unableDownloadPath[i] + '\\' + unableDownloadName[i], textColor: ConsoleColor.DarkGreen, final: '\n');
                        file.downloadFile(file.savePath + '\\' + unableDownloadPath[i], file.ip + '\\' + unableDownloadPath[i], unableDownloadName[i]);
                        Displayer.output(unableDownloadPath[i] + '\\' + unableDownloadName[i] + "重新下载成功", textColor: ConsoleColor.DarkGreen, final: '\n');
                        flag[i] = true;
                        success_count++;
                    }
                    catch (Exception e)
                    {
                        Log.warning("download files fails : " + unableDownloadPath[i] + '\\' + unableDownloadName[i]);
                        Log.warning("previous reason is as follows : " + e.Message);
                        Displayer.output(unableDownloadPath[i] + '\\' + unableDownloadName[i] + "正在被使用，将在稍后重新尝试下载", textColor: ConsoleColor.DarkRed, final: '\n');
                    }
                }
                if (success_count == unableDownloadPath.Count) break;
                Thread.Sleep(1000);
            }
            if (success_count > 0 && success_count == unableDownloadPath.Count) Displayer.output("所有文件下载成功", textColor: ConsoleColor.Green, final: '\n');
            else if (success_count == unableDownloadPath.Count) Displayer.output("没有需要重新下载的文件", final: '\n');
            else
            {
                for (int i = 0; i < unableDownloadPath.Count; i++)
                {
                    if (flag[i]) continue;
                    Displayer.output(unableDownloadPath[i] + '\\' + unableDownloadName[i] + "下载失败，请检查是否有人占用该文件", textColor: ConsoleColor.Red, final: '\n');
                }
            }
            for (int i = 0; i < unableDownloadName.Count; i++)
            {
                if (flag[i])
                {
                    unableDownloadName.RemoveAt(i);
                    unableDownloadPath.RemoveAt(i);
                    flag.RemoveAt(i);
                    i--;
                }
            }
        }
        internal static bool handle(string x, FileManager file)
        {
            if (!file.directoryExistServer(x)) return false;
            file.createDirectoryLocal(x);
            DirectoryInfo dir = file.getDirectoryInfoServer(x);
            FileSystemInfo[] info = dir.GetFileSystemInfos();
            checkDirectory(x, x, info, file);
            redownload(x, file, true);
            return true;
        }
    }
}
