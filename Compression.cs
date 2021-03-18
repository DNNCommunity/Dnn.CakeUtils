using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;

namespace Dnn.CakeUtils
{
    public static class Compression
    {
        public static void AddBinaryFileToZip(this ICakeContext context, FilePath zipFile, byte[] content, string name)
        {
            context.AddBinaryFileToZip(zipFile, content, name, false);
        }
        public static void AddBinaryFileToZip(this ICakeContext context, FilePath zipFile, byte[] content, string name, bool append)
        {
            using (var sr = new MemoryStream(content))
            {
                context.AddStreamToZip(zipFile, sr, name, append);
            }
        }
        public static void AddXmlFileToZip(this ICakeContext context, FilePath zipFile, XmlDocument content, string name)
        {
            context.AddXmlFileToZip(zipFile, content, name, false);
        }
        public static void AddXmlFileToZip(this ICakeContext context, FilePath zipFile, XmlDocument content, string name, bool append)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                content.WriteContentTo(writer);
                writer.Flush();
                ms.Flush();
                ms.Position = 0;
                context.AddStreamToZip(zipFile, ms, name, append);
            }
        }
        public static void AddTextFileToZip(this ICakeContext context, FilePath zipFile, string content, string name)
        {
            context.AddTextFileToZip(zipFile, content, name, false);
        }
        public static void AddTextFileToZip(this ICakeContext context, FilePath zipFile, string content, string name, bool append)
        {
            using (var sr = context.GenerateStreamFromString(content))
            {
                context.AddStreamToZip(zipFile, sr, name, append);
            }
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, FilePathCollection files)
        {
            context.AddFilesToZip(zipFile, ".", "", files, false);
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, FilePathCollection files, bool append)
        {
            context.AddFilesToZip(zipFile, ".", "", files, append);
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, DirectoryPath start, FilePathCollection files)
        {
            context.AddFilesToZip(zipFile, start, "", files, false);
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, DirectoryPath start, FilePathCollection files, bool append)
        {
            context.AddFilesToZip(zipFile, start, "", files, append);
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, DirectoryPath start, string newStart, FilePathCollection files)
        {
            context.AddFilesToZip(zipFile, start, newStart, files, false);
        }
        public static void AddFilesToZip(this ICakeContext context, FilePath zipFile, DirectoryPath start, string newStart, FilePathCollection files, bool append)
        {
            zipFile = context.MakeAbsolute(zipFile);
            if (!append)
            {
                if (context.FileExists(zipFile))
                {
                    context.DeleteFile(zipFile);
                }
            }

            if (newStart != "")
            {
                newStart = newStart.EnsureEndsWith("/");
            }
            
            var rootPath = context.MakeAbsolute(start).FullPath;
            foreach (var file in files)
            {
                context.Information("Reading " + file.FullPath);
                using (var fileToZip = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                {
                    var pathInZip = newStart == ""
                                        ? file.FullPath.Substring(rootPath.Length + 1)
                                        : newStart + file.FullPath.Substring(rootPath.Length + 1);
                    context.AddStreamToZip(zipFile, fileToZip, pathInZip, true);
                }
            }
        }
        public static void AddStreamToZip(this ICakeContext context, FilePath zipFile, Stream fileToZip, string name)
        {
            context.AddStreamToZip(zipFile, fileToZip, name, false);
        }
        public static void AddStreamToZip(this ICakeContext context, FilePath zipFile, Stream fileToZip, string name, bool append)
        {
            zipFile = context.MakeAbsolute(zipFile);
            if (!append)
            {
                if (context.FileExists(zipFile))
                {
                    context.DeleteFile(zipFile);
                }
            }
            using (var outFile = File.Open(zipFile.FullPath, FileMode.OpenOrCreate))
            {
                using (var outStream = new ZipArchive(outFile, append ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                {
                    context.Information("Zipping " + name);
                    var entry = outStream.CreateEntry(name);
                    fileToZip.CopyTo(entry.Open());
                }
            }
        }

        public static Stream ZipToStream(this ICakeContext context, DirectoryPath root, FilePathCollection files)
        {
            var rootPath = context.MakeAbsolute(root).FullPath;
            var outStream = new MemoryStream();
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    context.Information("Zipping " + file.FullPath);
                    var fileInArchive = archive.CreateEntry(file.FullPath.Substring(rootPath.Length + 1), CompressionLevel.Optimal);
                    using (var entryStream = fileInArchive.Open())
                    using (var fileToCompressStream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                    {
                        fileToCompressStream.CopyTo(entryStream);
                    }
                }
            }
            return outStream;
        }

        public static byte[] ZipToBytes(this ICakeContext context, DirectoryPath root, FilePathCollection files)
        {
            var rootPath = context.MakeAbsolute(root).FullPath;
            byte[] compressedBytes;

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        context.Information("Zipping " + file.FullPath);
                        var fileInArchive = archive.CreateEntry(file.FullPath.Substring(rootPath.Length + 1), CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return compressedBytes;
        }

        public static void AddFile(this ICakeContext context, FilePath sourcePath, Stream zip)
        {
            using (FileStream src = new FileStream(context.MakeAbsolute(sourcePath).FullPath, FileMode.Open, FileAccess.Read))
            {
                src.CopyToAsync(zip);
            }
        }

        public static void CopyStream(this ICakeContext context, Stream source, Stream destination, int bufferSize)
        {
            byte[] bBuffer = new byte[bufferSize + 1];
            int iLengthOfReadChunk;
            do
            {
                iLengthOfReadChunk = source.Read(bBuffer, 0, bufferSize);
                destination.Write(bBuffer, 0, iLengthOfReadChunk);
                if (iLengthOfReadChunk == 0)
                {
                    break;
                }
            }
            while (true);
        }

        public static MemoryStream GenerateStreamFromString(this ICakeContext context, string value)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value ?? ""));
        }

        public static void SaveStream(this ICakeContext context, Stream input, FilePath filePath)
        {
            if (input.CanSeek)
            {
                input.Seek(0, SeekOrigin.Begin);
            }
            using (var outFile = new FileStream(context.MakeAbsolute(filePath).FullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                input.CopyTo(outFile);
            }
        }

        public static void SaveBytes(this ICakeContext context, byte[] input, FilePath filePath)
        {
            using (var outFile = new FileStream(context.MakeAbsolute(filePath).FullPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var fileToSave = new MemoryStream(input))
            {
                fileToSave.CopyTo(outFile);
            }
        }

    }
}
