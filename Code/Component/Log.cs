using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Submitter.Code.Component
{
    internal class Log
    {
        static FileStream fs;
        internal static void init()
        {
            if (!File.Exists(Constant.logPath))
            {
                fs = File.Create(Constant.logPath);
                fs.Dispose();
            }
            FileInfo info = new(Constant.logPath);
            if (info.Length > 500 * 1024)
            {
                fs = File.OpenWrite(Constant.logPath);
                IOManager.Clear(fs);
                fs.Dispose();
                fs = File.OpenWrite(Constant.logPath);
            }
            else
            {
                fs = File.Open(Constant.logPath, FileMode.Append);
            }
            information("log system startup");
        }
        internal static void quit()
        {
            fs.Dispose();
        }
        internal static void information(string x)
        {
            IOManager.WriteLine(fs, $"[Information] {DateTime.Now.ToShortTimeString()} : " + x);
            fs.Flush();
        }
        internal static void warning(string x)
        {
            IOManager.WriteLine(fs, $"[Warning] {DateTime.Now.ToShortTimeString()} : " + x);
            fs.Flush();
        }
        internal static void error(string x)
        {
            IOManager.WriteLine(fs, $"[Error] {DateTime.Now.ToShortTimeString()} : " + x);
            fs.Flush();
        }
    }
}
