using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using System.Linq;

namespace Dnn.CakeUtils
{
    public static class ScriptAliases
    {
        [CakeMethodAlias]
        public static FilePathCollection GetFilesByPatterns(this ICakeContext context, string[] includePatterns)
        {
            var res = new FilePathCollection();
            for (var i = 0; i < includePatterns.Length; i++)
            {
                var incl = context.GetFiles(includePatterns[i]);
                var crt = res.Select(ii => ii.FullPath);
                foreach (var include in incl)
                {
                    if (!crt.Contains(include.FullPath))
                    {
                        res.Add(include);
                    }
                }
            }
            return res;
        }

        [CakeMethodAlias]
        public static FilePathCollection GetFilesByPatterns(this ICakeContext context, string[] includePatterns, string[] excludePatterns)
        {
            if (excludePatterns.Length == 0)
            {
                return GetFilesByPatterns(context, includePatterns);
            }

            FilePathCollection excludeFiles = context.GetFiles(excludePatterns[0]);
            for (var i = 1; i < excludePatterns.Length; i++)
            {
                excludeFiles.Add(context.GetFiles(excludePatterns[i]));
            }
            var excludePaths = excludeFiles.Select(e => e.FullPath);
            var res = new FilePathCollection();
            for (var i = 0; i < includePatterns.Length; i++)
            {
                var incl = context.GetFiles(includePatterns[i]);
                var crt = res.Select(ii => ii.FullPath);
                foreach (var include in incl)
                {
                    if (!excludeFiles.Contains(include.FullPath) && !crt.Contains(include.FullPath))
                    {
                        res.Add(include);
                    }
                }
            }
            return res;
        }

        [CakeMethodAlias]
        public static FilePathCollection GetFilesByPatterns(this ICakeContext context, DirectoryPath root, string[] includePatterns)
        {
            var res = new FilePathCollection();
            for (var i = 0; i < includePatterns.Length; i++)
            {
                var incl = context.GetFiles(root.CombineWithFilePath(includePatterns[i].TrimStart('/')).ToString());
                var crt = res.Select(ii => ii.FullPath);
                foreach (var include in incl)
                {
                    if (!crt.Contains(include.FullPath))
                    {
                        res.Add(include);
                    }
                }
            }
            return res;
        }

        [CakeMethodAlias]
        public static FilePathCollection GetFilesByPatterns(this ICakeContext context, DirectoryPath root, string[] includePatterns, string[] excludePatterns)
        {
            if (excludePatterns.Length == 0)
            {
                return GetFilesByPatterns(context, includePatterns);
            }
            
            var excludeFiles = context.GetFiles(root.CombineWithFilePath(excludePatterns[0].TrimStart('/')).ToString());
            for (var i = 1; i < excludePatterns.Length; i++)
            {
                excludeFiles.Add(context.GetFiles(root.CombineWithFilePath(excludePatterns[i].TrimStart('/')).ToString()));
            }
            var excludePaths = excludeFiles.Select(e => e.FullPath);
            var res = new FilePathCollection();
            for (var i = 0; i < includePatterns.Length; i++)
            {
                var incl = context.GetFiles(root.CombineWithFilePath(includePatterns[i].TrimStart('/')).ToString());
                var crt = res.Select(ii => ii.FullPath);
                foreach (var include in incl)
                {
                    if (!excludeFiles.Contains(include.FullPath) && !crt.Contains(include.FullPath))
                    {
                        res.Add(include);
                    }
                }
            }
            return res;
        }
    }
}
