using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Submitter.Code.Manager;

namespace Submitter.Code.Component
{
    public class FileFinder
    {
        internal static string find(string parentPath, string x, FileSystemInfo[] info, FileManager file, string id)
        {
            foreach (FileSystemInfo child in info)
            {
                if (child is DirectoryInfo)
                {
                    string result = find(parentPath + '\\' + child.Name, x, file.getDirectoryInfoFullPath(child.FullName).GetFileSystemInfos(), file, id);
                    if (result != "") return result;
                }
                else
                {
                    if (child.Name == id)
                    {
                        Log.information("find target files : " + file.ip + '\\' + parentPath + "\\" + child.Name);
                        return IOManager.ReadAll(file.ip + '\\' + parentPath + "\\" + child.Name);
                    }
                }
            }
            return "";
        }
        internal static string findPath(string parentPath, string x, FileSystemInfo[] info, FileManager file, string id)
        {
            foreach (FileSystemInfo child in info)
            {
                if (child is DirectoryInfo)
                {
                    string result = findPath(parentPath + '\\' + child.Name, x, file.getDirectoryInfoFullPath(child.FullName).GetFileSystemInfos(), file, id);
                    if (result != "") return result;
                }
                else
                {
                    if (child.Name == id)
                    {
                        Log.information("find path : " + file.ip + '\\' + parentPath);
                        return file.ip + '\\' + parentPath;
                    }
                }
            }
            return "";
        }
        internal static string getPath(string matchName, string name, FileManager file)
        {
            Log.information($"try to find file {name}");
            DirectoryInfo dir = file.getDirectoryInfoServer(matchName);
            FileSystemInfo[] info = dir.GetFileSystemInfos();
            string result = findPath(matchName, matchName, info, file, name);
            if (result == "") Log.information($"cannot find file {name}");
            else Log.information($"find file in path {result}");
            return result;
        }
    }
}
