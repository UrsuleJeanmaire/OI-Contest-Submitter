using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submitter.Code.Component;
using Submitter.Code.Data;

namespace Submitter.Code.Ability
{
    internal class IsEvaluation
    {
        internal static bool isEvaluation(string matchName, string id, FileManager file)
        {
            DirectoryInfo dir = file.getDirectoryInfoServer(matchName);
            FileSystemInfo[] info = dir.GetFileSystemInfos();
            string result = FileFinder.findPath(matchName, matchName, info, file, id + ".rec");
            Record rec = new(result + '\\' + id + ".rec");
            return rec.get("result") != "";
        }
        internal static bool isEvaluation(Record rec)
        {
            return rec.get("result") != "";
        }
        internal static bool isEvaluation(string[] content)
        {
            return content.Length >= 6;
        }
    }
}
