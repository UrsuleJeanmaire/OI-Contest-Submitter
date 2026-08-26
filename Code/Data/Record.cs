using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Manager;

namespace Submitter.Code.Data
{
    internal class Record
    {
        internal Dictionary<string, string> data = new();
        internal string get(string x)
        {
            if (data.ContainsKey(x)) return data[x];
            return "";
        }
        public Record(string path)
        {
            string[] content = IOManager.ReadAll(path).Split('\n');
            for(int i = 0; i < content.Length; i++) content[i] = content[i].Trim();
            data["id"] = content[0];
            data["problem"] = content[1];
            data["submitter"] = content[2];
            data["time"] = content[3];
            if (content.Length >= 6) data["result"] = content[4];
            else data["result"] = "";
        }
    }
}
