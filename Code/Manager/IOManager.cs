using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Component;

namespace Submitter
{
    internal class IOManager
    {
        public static void WriteLine(FileStream fs, string value)
        {
            byte[] buffer = new UTF8Encoding(true).GetBytes(value + "\n");
            fs.Write(buffer, 0, buffer.Length);
        }
        public static void Clear(FileStream fs)
        {
            Log.information("clear file : " + fs.Name);
            fs.SetLength(0);
        }
        public static string ReadAll(string path)
        {
            Log.information("read file : " + path);
            using (FileStream fs = File.OpenRead(path))
            {
                UTF8Encoding encoding = new UTF8Encoding(true);
                byte[] buffer = new byte[4096];
                StringBuilder sb = new StringBuilder();
                int bytesRead = 0;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(encoding.GetString(buffer, 0, bytesRead));
                }
                return sb.ToString();
            }
        }
    }
}
