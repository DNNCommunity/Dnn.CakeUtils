using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Dnn.CakeUtils
{
    public class AssemblyInfo
    {
        private string FilePath { get; set; }
        private bool IsVB { get; set; } = false;
        private Dictionary<int, string> Lines { get; set; } = new Dictionary<int, string>();
        private Dictionary<string, string> StringProperties = new Dictionary<string, string>();
        private Dictionary<int, string> StringPropLines = new Dictionary<int, string>();
        public AssemblyInfo(string filePath)
        {
            FilePath = filePath;
            IsVB = Path.GetExtension(filePath).ToLower().EndsWith(".vb");
            var regex = IsVB ? @"^\<Assembly\: ([^\(]+)\(""(.*)\""\)\>" : @"^\[assembly\: ([^\(]+)\(""(.*)\""\)\]";
            var lineNr = 0;
            using (var sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    Lines[lineNr] = line;
                    var m = Regex.Match(line, regex);
                    if (m.Success)
                    {
                        var prop = m.Groups[1].Value.Trim();
                        var val = m.Groups[2].Value;
                        StringProperties[prop] = val;
                        StringPropLines[lineNr] = prop;
                    }
                    lineNr++;
                }
            }
        }
        public void SetProperty(string propName, string propValue)
        {
            if (!StringProperties.ContainsKey(propName))
            {
                StringPropLines[Lines.Count] = propName;
                Lines[Lines.Count] = "";
            }
            StringProperties[propName] = propValue;
        }
        public void Write()
        {
            Write(FilePath);
        }
        public void Write(string filePath)
        {
            var pattern = IsVB ? "<Assembly: {0}(\"{1}\")>" : "[assembly: {0}(\"{1}\")]";
            using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                for (var lineNr = 0; lineNr < Lines.Count; lineNr++)
                {
                    if (StringPropLines.ContainsKey(lineNr))
                    {
                        sw.WriteLine(string.Format(pattern, StringPropLines[lineNr], StringProperties[StringPropLines[lineNr]]));
                    }
                    else
                    {
                        sw.WriteLine(Lines[lineNr]);
                    }
                }
            }
        }
    }
}
