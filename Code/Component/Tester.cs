using System.Runtime.InteropServices;
using Submitter.Code.Ability;
using Submitter.Code.Data;
using Submitter.Code.Manager;

namespace Submitter.Code.Component
{
    public static class ClipboardHelper
    {
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(nint hWndNewOwner);
        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();
        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        private static extern nint SetClipboardData(uint uFormat, nint hMem);
        [DllImport("kernel32.dll")]
        private static extern nint GlobalAlloc(uint uFlags, nuint dwBytes);
        [DllImport("kernel32.dll")]
        private static extern nint GlobalLock(nint hMem);
        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(nint hMem);
        [DllImport("user32.dll")]
        private static extern nint GetOpenClipboardWindow();
        private const uint CF_UNICODETEXT = 13;
        private const uint GMEM_MOVEABLE = 0x0002;
        public static bool TrySetText(string text, int timeoutMs = 5000)
        {
            int sleepInterval = 50;
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                if (OpenClipboard(nint.Zero))
                {
                    try
                    {
                        EmptyClipboard();
                        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text + "\0");
                        nint hGlobal = GlobalAlloc(GMEM_MOVEABLE, (nuint)bytes.Length);
                        if (hGlobal == nint.Zero) return false;
                        nint locked = GlobalLock(hGlobal);
                        if (locked == nint.Zero) { GlobalFree(hGlobal); return false; }
                        Marshal.Copy(bytes, 0, locked, bytes.Length);
                        GlobalUnlock(hGlobal);
                        SetClipboardData(CF_UNICODETEXT, hGlobal);
                        return true;
                    }
                    finally
                    {
                        CloseClipboard();
                    }
                }
                Thread.Sleep(sleepInterval);
                elapsed += sleepInterval;
            }
            return false;
        }
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);
        [DllImport("kernel32.dll")]
        private static extern nint GlobalFree(nint hMem);
    }
    internal class Tester
    {
        internal static void singleTest(string matchName, string id, FileManager file)
        {
            FileStream fss;
            Log.information("test id : " + id);
            Displayer.output("评测编号" + id + "代码已复制到剪贴板：", final: '\n');

            DirectoryInfo dir = file.getDirectoryInfoServer(matchName);
            FileSystemInfo[] info = dir.GetFileSystemInfos();
            string code = FileFinder.find(matchName, matchName, info, file, id + ".cpp");
            string path = FileFinder.findPath(matchName, matchName, info, file, id + ".cpp");
            string recPath = FileFinder.findPath(matchName, matchName, info, file, id + ".rec");
            Record rec = new(recPath + "\\" + id + ".rec");

            ClipboardHelper.TrySetText(code);
            Displayer.output(code, textColor: ConsoleColor.Gray, final: '\n');
            Displayer.output("", final: '\n');

            Displayer.output("本份代码是 " + rec.get("problem") + " 的提交", final: '\n', textColor: ConsoleColor.Yellow);
            Displayer.output("评测完成后，在此输入评测记录链接：");
            if (IsEvaluation.isEvaluation(matchName, rec.get("id"), file)) RemoveLastLine.handle(recPath + '\\' + id + ".rec", file);
            string links = Console.ReadLine();
            fss = File.Open(recPath + '\\' + id + ".rec", FileMode.Append);
            IOManager.WriteLine(fss, links);
            fss.Dispose();
        }
        internal static void test(string matchName, FileManager file)
        {
            FileStream fs;
            if (!Directory.Exists(file.ip + "\\" + matchName + "\\#submit"))
            {
                file.createDirectoryFullPath(file.ip + "\\" + matchName + "\\#submit");
            }
            if (!File.Exists(file.ip + "\\" + matchName + "\\#submit\\" + "uncheck_id.txt"))
            {
                fs = File.Create(file.ip + "\\" + matchName + "\\#submit\\" + "uncheck_id.txt");
                Displayer.output("没有新的评测", final: '\n');
                return;
            }
            string[] ids = IOManager.ReadAll(file.ip + "\\" + matchName + "\\#submit\\" + "uncheck_id.txt").Split('\n');
            fs = File.OpenWrite(file.ip + "\\" + matchName + "\\#submit\\" + "uncheck_id.txt");
            IOManager.Clear(fs);
            fs.Dispose();
            for (int i = 0; i < ids.Length - 1; i++)
            {
                if (IsEvaluation.isEvaluation(matchName, ids[i], file)) continue;
                singleTest(matchName, ids[i], file);
            }
            Displayer.output("全部评测结束", final: '\n');
        }
    }
}
