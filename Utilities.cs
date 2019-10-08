using Cake.Core;
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

        public static void CreateResourcesFile(ICakeContext context, string path, string packagePath, string packageName, string[] releaseFiles, string[] excludeFiles)
        {
            var files = context.GetFilesByPatterns(path, releaseFiles, excludeFiles);
            if (files.Count > 0)
            {
                var resZip = Compression.ZipToBytes(path, files);
                Console.WriteLine("Zipped resources file");
                Compression.AddBinaryFileToZip(packagePath, resZip, packageName + ".zip", true);
            }
            Console.WriteLine("Added resources from " + path);
        }
    }
}
