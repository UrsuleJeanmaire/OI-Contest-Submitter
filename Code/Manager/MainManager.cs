using System.Diagnostics;
using Submitter.Code.Ability;
using Submitter.Code.Command;
using Submitter.Code.Component;
using Submitter.Code.Data;

namespace Submitter.Code.Manager
{
    internal class MainManager
    {
        FileManager file = new();
        string inMatch = "";
        void helpMeErin()
        {
            Displayer.output("help:输出帮助文本", final: '\n');
            Displayer.output("join arg1:加入名为 arg1 的比赛，并尝试下载该比赛的所有文件夹结构及相关文件", final: '\n');
            Displayer.output("redownload:尝试重新下载上一个加入的比赛中，没有被下载下来的数据", final: '\n');
            Displayer.output("submit arg1 arg2:将文件路径为 arg2 的代码，提交到当前加入的比赛中题目名为 arg1 的题目", final: '\n');
            Displayer.output("check arg1:检查编号为 arg1 的提交记录的评测结果（一般需等待5~10分钟）", final: '\n');
            Displayer.output("records:列出当前比赛自己所有的提交记录", final: '\n');
            Displayer.output("quitmatch:退出当前比赛", final: '\n');
            Displayer.output("reset:重新进行参数设置", final: '\n');
            Displayer.output("quit:退出命令行程序", final: '\n');
            Displayer.output("", final: '\n');
            Displayer.output("以下是评测姬用命令，请不要随意使用喵：", final: '\n');
            Displayer.output("starttest:开始评测所有的还未评测提交", final: '\n');
            Displayer.output("singletest arg1:单独评测编号为 arg1 的提交记录", final: '\n');
            Displayer.output("allrecords:查看所有提交记录", final: '\n');
        }
        bool handleComplexCommand(string command)
        {
            string[] each = command.Split(' ');
            if (each[0] == "join")
            {
                if (inMatch != "")
                {
                    Displayer.output("您已在比赛" + inMatch + "中", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                if (each.Length <= 1)
                {
                    Displayer.output("参数数量错误", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                string fullName = each[1];
                for (int i = 2; i < each.Length; i++) fullName += " " + each[i];
                bool status = JoinCompetition.handle(fullName, file);
                if (!status)
                {
                    Displayer.output("目标比赛不存在，或者比赛文件夹结构有误", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                else
                {
                    inMatch = each[1];
                    Displayer.output("已加入比赛" + inMatch, final: '\n');
                }
                return true;
            }
            else if (each[0] == "submit")
            {
                if (inMatch == "")
                {
                    Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                if (each.Length <= 2)
                {
                    Displayer.output("参数数量错误", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                string fullName = each[2];
                for (int i = 3; i < each.Length; i++) fullName += " " + each[i];
                bool status = SubmitCode.handle(inMatch, each[1], fullName, file);
                if (!status) Displayer.output("题目不存在", textColor: ConsoleColor.Red, final: '\n');
                return true;
            }
            else if (each[0] == "check")
            {
                if (inMatch == "")
                {
                    Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                if (each.Length <= 1)
                {
                    Displayer.output("参数数量错误", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                string name = each[1] + ".rec";
                string result = FileFinder.getPath(inMatch, name, file);
                if (result == "")
                {
                    Displayer.output("该提交记录不存在", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                Record rec = new(result + '\\' + name);
                if (!IsEvaluation.isEvaluation(rec)) Displayer.output("该提交记录还未被评测", textColor: ConsoleColor.Red, final: '\n');
                else Process.Start(new ProcessStartInfo(rec.get("result")) { UseShellExecute = true });
                return true;
            }
            else if (each[0] == "singletest")
            {
                if (inMatch == "")
                {
                    Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                if (each.Length <= 1)
                {
                    Displayer.output("参数数量错误", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                string name = each[1] + ".rec";
                string result = FileFinder.getPath(inMatch, name, file);
                if (result == "")
                {
                    Displayer.output("该提交记录不存在", textColor: ConsoleColor.Red, final: '\n');
                    return false;
                }
                Tester.singleTest(inMatch, each[1], file);
                return true;
            }
            Displayer.output("未找到对应命令，或许您需要help？", final: '\n');
            return false;
        }
        public void start()
        {
            file.init();
            while (true)
            {
                Displayer.output("请输入命令，或输入\"help\"以查看帮助文本", final: '\n');
                string command = Console.ReadLine();
                Log.information(command);
                Displayer.output("", final: '\n');
                if (command == null) continue;
                else if (command == "quit") break;
                else if (command == "reset") file.initSetting();
                else if (command == "help") helpMeErin();
                else if (command == "redownload") JoinCompetition.redownload(inMatch, file);
                else if (command == "records") ShowRecords.handle(inMatch, file);
                else if (command == "allrecords") ShowRecords.handleHost(inMatch, file);
                else if (command == "starttest")
                {
                    if (inMatch == "")
                    {
                        Displayer.output("还未加入比赛", textColor: ConsoleColor.Red, final: '\n');
                        continue;
                    }
                    Tester.test(inMatch, file);
                }
                else if (command == "quitmatch")
                {
                    Displayer.output("已退出比赛" + inMatch, final: '\n');
                    JoinCompetition.clear();
                    inMatch = "";
                }
                else handleComplexCommand(command);
                Displayer.output("", final: '\n');
            }
        }
    }
}
