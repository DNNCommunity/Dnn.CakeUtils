using Markdig;
using System.IO;

namespace Dnn.CakeUtils
{
    public class Markdown
    {
        public static string ToHtml(string fileName)
        {
            if (File.Exists(fileName))
            {
                var input = "";
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                using (var sr = new StreamReader(fileName))
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
