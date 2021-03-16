using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

using Cake.Core.IO;

namespace Dnn.CakeUtils
{
    public static class Compression
    {
        public static void AddBinaryFileToZip(FilePath zipFile, byte[] content, FilePath name)
        {
            AddBinaryFileToZip(zipFile, content, name, false);
        }
        public static void AddBinaryFileToZip(FilePath zipFile, byte[] content, FilePath name, bool append)
        {
            using (var sr = new MemoryStream(content))
            {
                AddStreamToZip(zipFile, sr, name, append);
            }
        }
        public static void AddXmlFileToZip(FilePath zipFile, XmlDocument content, FilePath name)
        {
            AddXmlFileToZip(zipFile, content, name, false);
        }
        public static void AddXmlFileToZip(FilePath zipFile, XmlDocument content, FilePath name, bool append)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                content.WriteContentTo(writer);
                writer.Flush();
                ms.Flush();
                ms.Position = 0;
                AddStreamToZip(zipFile, ms, name, append);
            }
        }
        public static void AddTextFileToZip(FilePath zipFile, string content, FilePath name)
        {
            AddTextFileToZip(zipFile, content, name, false);
        }
        public static void AddTextFileToZip(FilePath zipFile, string content, FilePath name, bool append)
        {
            using (var sr = GenerateStreamFromString(content))
            {
                AddStreamToZip(zipFile, sr, name, append);
            }
        }
        public static void AddFilesToZip(FilePath zipFile, FilePathCollection files)
        {
            AddFilesToZip(zipFile, ".", "", files, false);
        }
        public static void AddFilesToZip(FilePath zipFile, FilePathCollection files, bool append)
        {
            AddFilesToZip(zipFile, ".", "", files, append);
        }
        public static void AddFilesToZip(FilePath zipFile, DirectoryPath root, FilePathCollection files)
        {
            AddFilesToZip(zipFile, root, "", files, false);
        }
        public static void AddFilesToZip(FilePath zipFile, DirectoryPath root, FilePathCollection files, bool append)
        {
            AddFilesToZip(zipFile, root, "", files, append);
        }
        public static void AddFilesToZip(FilePath zipFile, DirectoryPath root, DirectoryPath newRoot, FilePathCollection files)
        {
            AddFilesToZip(zipFile, root, newRoot, files, false);
        }
        public static void AddFilesToZip(FilePath zipFile, DirectoryPath root, DirectoryPath newRoot, FilePathCollection files, bool append)
        {
            if (!append)
            {
                if (File.Exists(zipFile.FullPath))
                {
                    File.Delete(zipFile.FullPath);
                }
            }
            
            var rootPath = root.FullPath;
            foreach (var file in files)
            {
                Console.WriteLine("Reading " + file.FullPath);
                using (var fileToZip = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                {
                    var pathInZip = newRoot.CombineWithFilePath(file.FullPath.Substring(rootPath.Length + 1));
                    AddStreamToZip(zipFile, fileToZip, pathInZip, true);
                }
            }
        }
        public static void AddStreamToZip(FilePath zipFile, Stream fileToZip, FilePath name)
        {
            AddStreamToZip(zipFile, fileToZip, name, false);
        }
        public static void AddStreamToZip(FilePath zipFile, Stream fileToZip, FilePath name, bool append)
        {
            if (!append)
            {
                if (File.Exists(zipFile.FullPath))
                {
                    File.Delete(zipFile.FullPath);
                }
            }
            using (var outFile = File.Open(zipFile.FullPath, FileMode.OpenOrCreate))
            {
                using (var outStream = new ZipArchive(outFile, append ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                {
                    Console.WriteLine("Zipping " + name);
                    var entry = outStream.CreateEntry(name.FullPath);
                    fileToZip.CopyTo(entry.Open());
                }
            }
        }

        public static Stream ZipToStream(DirectoryPath root, FilePathCollection files)
        {
            var rootPath = root.FullPath;
            var outStream = new MemoryStream();
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    Console.WriteLine("Zipping " + file.FullPath);
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

        public static byte[] ZipToBytes(DirectoryPath root, FilePathCollection files)
        {
            var rootPath = root.FullPath;
            byte[] compressedBytes;

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine("Zipping " + file.FullPath);
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

        public static void AddFile(FilePath sourcePath, Stream zip)
        {
            using (FileStream src = new FileStream(sourcePath.FullPath, FileMode.Open, FileAccess.Read))
            {
                src.CopyToAsync(zip);
            }
        }

        public static void CopyStream(Stream source, Stream destination, int bufferSize)
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

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value ?? ""));
        }

        public static void SaveStream(Stream input, FilePath filePath)
        {
            if (input.CanSeek)
            {
                input.Seek(0, SeekOrigin.Begin);
            }
            using (var outFile = new FileStream(filePath.FullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                input.CopyTo(outFile);
            }
        }

        public static void SaveBytes(byte[] input, FilePath filePath)
        {
            using (var outFile = new FileStream(filePath.FullPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var fileToSave = new MemoryStream(input))
            {
                fileToSave.CopyTo(outFile);
            }
        }

    }
}
