using System.IO;

using Cake.Core.IO;

using Markdig;

namespace Dnn.CakeUtils
{
    public class Markdown
    {
        public static string ToHtml(FilePath fileName)
        {
            if (File.Exists(fileName.FullPath))
            {
                var input = "";
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                using (var sr = new StreamReader(fileName.FullPath))
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
