using Submitter.Code.Component;

namespace Submitter.Command
{
    internal class SubmitCode
    {
        internal static void saveRecord(string competition, string name, string id, FileManager file)
        {
            string userPath = file.ip + "\\" + competition + "\\#submit\\" + file.name;
            if (!file.directoryExistFullPath(userPath + '\\' + "#record")) file.createDirectoryFullPath(userPath + '\\' + "#record");
            Log.information("save record to : " + userPath + '\\' + "#record");
            FileStream fs = File.Create(userPath + '\\' + "#record" + '\\' + id + ".rec");
            IOManager.WriteLine(fs, id);
            IOManager.WriteLine(fs, name);
            IOManager.WriteLine(fs, file.name);
            IOManager.WriteLine(fs, DateTime.Now.ToString());
            fs.Dispose();
        }
        internal static bool handle(string competition, string name, string path, FileManager file)
        {
            bool flag = false;
            foreach(var x in JoinCompetition.problemName)
                if(x == name) { flag = true; break; }
            if (!flag)
                return false;

            string userPath = file.ip + "\\" + competition + "\\#submit\\" + file.name;
            string problemPath = userPath + "\\" + name;
            if (!file.directoryExistFullPath(userPath))
                file.createDirectoryFullPath(userPath);
            if (!file.directoryExistFullPath(problemPath))
                file.createDirectoryFullPath(problemPath);
            if (!File.Exists(problemPath + '\\' + name + ".ini"))
            {
                FileStream fss = File.Create(problemPath + '\\' + name + ".ini");
                IOManager.WriteLine(fss, "0");
                fss.Dispose();
            }
            string[] str = IOManager.ReadAll(problemPath + '\\' + name + ".ini").Split('\n');
            int cnt = Convert.ToInt32(str[0]) + 1;
            string hashResult = Hash.generate(file.name + "_" + name + "_" + cnt.ToString());
            string newFileName = hashResult + ".cpp";
            FileStream fs = File.OpenWrite(problemPath + '\\' + name + ".ini");
            IOManager.WriteLine(fs, cnt.ToString());
            fs.Dispose();
            try
            {
                Log.information($"submit files from {path} to {problemPath + '\\' + newFileName}");
                file.uploadFile(path, problemPath, newFileName);
                Displayer.output("提交成功，目标路径为" + problemPath + '\\' + newFileName, textColor: ConsoleColor.Green, final: '\n');
                Displayer.output("提交记录编号为" + hashResult, textColor: ConsoleColor.Yellow, final: '\n');
            }
            catch (Exception ex)
            {
                Log.warning($"submit fails, the reason is {ex.Message}");
                Displayer.output("因为如下原因，提交失败......", textColor: ConsoleColor.Red, final: '\n');
                Displayer.output(ex.Message, textColor: ConsoleColor.Red, final: '\n');
                return true;
            }
            RETRY:
            int cntr = 0;
            try
            {
                if (!Directory.Exists(file.ip + "\\" + competition + "\\#submit"))
                    file.createDirectoryFullPath(file.ip + "\\" + competition + "\\#submit");
                if (!File.Exists(file.ip + "\\" + competition + "\\#submit\\" + "uncheck_id.txt"))
                    fs = File.Create(file.ip + "\\" + competition + "\\#submit\\" + "uncheck_id.txt");
                else
                    fs = File.Open(file.ip + "\\" + competition + "\\#submit\\" + "uncheck_id.txt", FileMode.Append);
            }
            catch
            {
                if (cntr < 5)
                {
                    cntr++;
                    Thread.Sleep(500);
                    goto RETRY;
                }
                Log.warning($"fail to write to the list of waiting checked");
            }
            saveRecord(competition, name, hashResult, file);
            IOManager.WriteLine(fs, hashResult);
            fs.Dispose();
            return true;
        }
    }
}
