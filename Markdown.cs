using System.IO;

using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;

using Markdig;

namespace Dnn.CakeUtils
{
    public static class Markdown
    {
        public static string ToHtml(this ICakeContext context, FilePath fileName)
        {
            if (context.FileExists(fileName))
            {
                var input = "";
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                using (var sr = new StreamReader(context.MakeAbsolute(fileName).FullPath))
                {
                    input = sr.ReadToEnd();
                }
                return Markdig.Markdown.ToHtml(input, pipeline);
            }
            else
            {
                return "";
            }
        }
    }
}
