using System;
using System.IO;

using Cake.Core;
using Cake.Core.IO;

namespace Dnn.CakeUtils
{
    public class Utilities
    {
        public static void UpdateAssemblyInfo(Solution sln, FilePath filePath)
        {
            var ai = new AssemblyInfo(filePath);
            ai.SetProperty("AssemblyVersion", sln.version);
            ai.SetProperty("AssemblyFileVersion", sln.version);
            ai.SetProperty("AssemblyTitle", sln.name);
            ai.SetProperty("AssemblyDescription", sln.description);
            ai.SetProperty("AssemblyCompany", sln.dnn.owner.organization);
            ai.SetProperty("AssemblyCopyright", $"Copyright {System.DateTime.Now.Year} by {sln.dnn.owner.organization}");
            ai.Write();
        }

        public static void UpdateAssemblyInfoVersion(Version version, string informationalVersion, FilePath filePath)
        {
            var ai = new AssemblyInfo(filePath);
            ai.SetProperty("AssemblyVersion", version.ToString(3));
            ai.SetProperty("AssemblyFileVersion", version.ToString(4));
            if (!string.IsNullOrEmpty(informationalVersion))
            {
                ai.SetProperty("AssemblyInformationalVersion", informationalVersion);
            }
            ai.Write();
        }

        public static void UpdateCsProjFile(Solution sln, FilePath filePath)
        {
            var projFile = new CsProjFile(filePath);
            if (!projFile.IsNetCore)
            {
                return;
            }
            projFile.SetProperty("AssemblyVersion", sln.version);
            projFile.SetProperty("FileVersion", sln.version);
            projFile.SetProperty("Product", sln.name);
            projFile.SetProperty("Description", sln.description);
            projFile.SetProperty("Company", sln.dnn.owner.organization);
            projFile.SetProperty("Copyright", $"Copyright {System.DateTime.Now.Year} by {sln.dnn.owner.organization}");
            projFile.Write();
        }

        public static string GetTextOrMdFile(FilePath filePathWithExtension)
        {
            var filePath = filePathWithExtension.GetDirectory().CombineWithFilePath(filePathWithExtension.GetFilenameWithoutExtension());
            Console.WriteLine("GetTextOrMdFile {0}", filePath);
            if (File.Exists(filePath + ".md"))
            {
                Console.WriteLine("MD Found");
                return Markdown.ToHtml(filePath + ".md");
            }
            if (File.Exists(filePath + ".txt"))
            {
                Console.WriteLine("TXT Found");
                return ReadFile(filePath + ".txt");
            }
            return "";
        }

        public static string ReadFile(FilePath filePath)
        {
            using (var sr = new System.IO.StreamReader(filePath.FullPath))
            {
                return sr.ReadToEnd();
            }
        }

        public static void CreateResourcesFile(ICakeContext context, FilePath path, FilePath packagePath, FilePath packageName, string[] releaseFiles, string[] excludeFiles)
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
