using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Manager;

namespace Submitter.Code.Ability
{
    internal class RemoveLastLine
    {
        internal static void handle(string path, FileManager file)
        {
            string[] content = IOManager.ReadAll(path).Split('\n');
            FileStream fs = File.OpenWrite(path);
            IOManager.Clear(fs);
            for (int i = 0; i < content.Length - 2; i++) IOManager.WriteLine(fs, content[i]);
            fs.Dispose();
        }
    }
}
