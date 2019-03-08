using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using System;

namespace Dnn.CakeUtils
{
    public class Utilities
    {
        public static void UpdateAssemblyInfo(Solution sln, string filePath)
        {
            var ai = new AssemblyInfo(filePath);
            ai.SetProperty("AssemblyVersion", sln.version);
            ai.SetProperty("AssemblyFileVersion", sln.version);
            ai.SetProperty("AssemblyTitle", sln.name);
            ai.SetProperty("AssemblyDescription", sln.description);
            ai.SetProperty("AssemblyCompany", sln.dnn.owner.organization);
            ai.SetProperty("AssemblyCopyright", string.Format("Copyright {0} by {1}", System.DateTime.Now.Year, sln.dnn.owner.organization));
            ai.Write();
        }

        public static FilePathCollection GetFilesByPatterns(ICakeContext context, string[] patterns)
        {
            FilePathCollection res = context.GetFiles(patterns[0]);
            for (var i = 1; i < patterns.Length - 1; i++)
            {
                res += context.GetFiles(patterns[i]);
            }
            return res;
        }

        public static FilePathCollection GetFilesByPatterns(ICakeContext context, string[] patterns, Func<IDirectory, bool> predicate)
        {
            var settings = new GlobberSettings();
            settings.Predicate = predicate;
            settings.IsCaseSensitive = false;
            FilePathCollection res = context.GetFiles(patterns[0], settings);
            for (var i = 1; i < patterns.Length - 1; i++)
            {
                res += context.GetFiles(patterns[i], settings);
            }
            return res;
        }

        public static FilePathCollection GetFilesByPatterns(ICakeContext context, string root, string[] patterns)
        {
            root = root.EnsureEndsWith("/");
            FilePathCollection res = context.GetFiles(root + patterns[0]);
            for (var i = 1; i < patterns.Length - 1; i++)
            {
                res += context.GetFiles(root + patterns[i]);
            }
            return res;
        }

        public static FilePathCollection GetFilesByPatterns(ICakeContext context, string root, string[] patterns, Func<IDirectory, bool> predicate)
        {
            root = root.EnsureEndsWith("/");
            var settings = new GlobberSettings();
            settings.Predicate = predicate;
            settings.IsCaseSensitive = false;
            FilePathCollection res = context.GetFiles(root + patterns[0], settings);
            for (var i = 1; i < patterns.Length - 1; i++)
            {
                res += context.GetFiles(root + patterns[i], settings);
            }
            return res;
        }

        public static Func<IFileSystemInfo, bool> ExcludeFunction(string[] excludeFilter)
        {
            return ExcludeFunction(excludeFilter, true);
        }
        public static Func<IFileSystemInfo, bool> ExcludeFunction(string[] excludeFilter, bool start)
        {
            return fileSystemInfo =>
            {
                var crt = new System.IO.DirectoryInfo(".");
                var rel = fileSystemInfo.Path.FullPath.Substring(crt.FullName.Length + 1);
                foreach (var ef in excludeFilter)
                {
                    //Console.WriteLine("{0} =? {1}", rel, ef);
                    if (start)
                    {
                        if (rel.StartsWith(ef))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (rel.EndsWith(ef))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };
        }

        public static string GetTextOrMdFile(string filePathWithoutExtension)
        {
            if (System.IO.File.Exists(filePathWithoutExtension + ".md"))
            {
                return Markdown.ToHtml(filePathWithoutExtension + ".md");
            }
            if (System.IO.File.Exists(filePathWithoutExtension + ".txt"))
            {
                return ReadFile(filePathWithoutExtension + ".txt");
            }
            return "";
        }

        public static string ReadFile(string filePath)
        {
            var output = "";
            using (var sr = new System.IO.StreamReader(filePath))
            {
                output = sr.ReadToEnd();
            }
            return output;
        }
    }
}
