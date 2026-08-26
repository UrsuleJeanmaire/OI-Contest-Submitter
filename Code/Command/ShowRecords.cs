using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Ability;
using Submitter.Code.Component;
using Submitter.Code.Data;

namespace Submitter.Command
{
    internal class ShowRecords
    {
        private static void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
        }
        private static void SortAsFileCreationTime(ref List<FileInfo> arrFi)
        {
            arrFi.Sort(delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
        }
        internal static void handle(string matchName, FileManager file)
        {
            if (matchName == "")
            {
                Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                return;
            }
            string userPath = file.ip + "\\" + matchName + "\\#submit\\" + file.name;
            if (!file.directoryExistFullPath(userPath + '\\' + "#record"))
            {
                Displayer.output("还没有提交记录", final: '\n');
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(userPath + '\\' + "#record");
            FileInfo[] files = dir.GetFiles();
            SortAsFileCreationTime(ref files);
            foreach (FileInfo fi in files)
            {
                Record rec = new(fi.FullName);
                Displayer.output("提交记录" + rec.get("id") + ":", final: '\n');
                Displayer.output("\t提交者:" + rec.get("submitter"), final: '\n');
                Displayer.output("\t提交题目:" + rec.get("problem"), final: '\n');
                Displayer.output("\t提交于" + rec.get("time"), final: '\n');
                Displayer.output("\t评测状态:", final: ' ');
                if (IsEvaluation.isEvaluation(rec))
                {
                    Displayer.output("已评测", textColor: ConsoleColor.Green, final: '\n');
                    Displayer.output("\t评测记录为:" + rec.get("result"), final: '\n');
                }
                else Displayer.output("未评测", textColor: ConsoleColor.Red, final: '\n');
                Displayer.output("", final: '\n');
            }
        }
        internal static void handleHost(string matchName, FileManager file)
        {
            if(matchName == "")
            {
                Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                return;
            }
            string userPath = file.ip + "\\" + matchName + "\\#submit";
            DirectoryInfo dir = file.getDirectoryInfoFullPath(userPath);
            FileSystemInfo[] info = dir.GetFileSystemInfos();
            List<FileInfo> all = new();
            foreach(FileSystemInfo child in info)
            {
                if (child is not DirectoryInfo) continue;
                DirectoryInfo nxtDir = file.getDirectoryInfoFullPath(child.FullName);
                FileSystemInfo[] nxtInfo = nxtDir.GetFileSystemInfos();
                foreach(FileSystemInfo childchild in nxtInfo)
                {
                    if (childchild.Name != "#record") continue;
                    DirectoryInfo newDir = file.getDirectoryInfoFullPath(childchild.FullName);
                    FileInfo[] files = newDir.GetFiles();
                    foreach (FileInfo fi in files) all.Add(fi);
                }
            }
            SortAsFileCreationTime(ref all);
            foreach (FileInfo fi in all)
            {
                Record rec = new(fi.FullName);
                Displayer.output("提交记录" + rec.get("id") + ":", final: '\n');
                Displayer.output("\t提交者:" + rec.get("submitter"), final: '\n');
                Displayer.output("\t提交题目:" + rec.get("problem"), final: '\n');
                Displayer.output("\t提交于" + rec.get("time"), final: '\n');
                Displayer.output("\t评测状态:", final: ' ');
                if (IsEvaluation.isEvaluation(rec))
                {
                    Displayer.output("已评测", textColor: ConsoleColor.Green, final: '\n');
                    Displayer.output("\t评测记录为:" + rec.get("result"), final: '\n');
                }
                else Displayer.output("未评测", textColor: ConsoleColor.Red, final: '\n');
                Displayer.output("", final: '\n');
            }
        }
    }
}
