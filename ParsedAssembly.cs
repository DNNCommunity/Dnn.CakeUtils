using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Dnn.CakeUtils
{
  public class ParsedAssembly
  {
    public string Name { get; set; } = "";
    public string PublicKeyToken { get; set; } = "";
    public string Version { get; set; } = "";
    public string TargetFramework { get; set; } = "";
    public string TargetFrameworkVersion { get; set; } = "";
    public string Exception { get; set; } = "";
    public List<string> References { get; set; } = new List<string>();

    public ParsedAssembly(Assembly assembly)
    {
      var details = assembly.GetName();
      if (details == null)
      {
        this.Name = assembly.FullName;
        this.Exception = "Assembly.GetName() returned null";
        return;
      }

      this.Name = details.Name;
      this.Version = details.Version.ToString();
      this.PublicKeyToken = ReadPublicKey(details);

      var references = assembly.GetReferencedAssemblies();
      foreach (var reference in references.OrderBy(r => r.Name))
      {
        References.Add($"{reference.Name} {reference.Version}");
      }

      try
      {
        var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        var frameworkName = targetFrameworkAttribute?.FrameworkName;
        if (string.IsNullOrEmpty(frameworkName))
        {
          this.Exception = "No TargetFrameworkAttribute found";
          return;
        }

        var parts = frameworkName.Split(',');
        TargetFramework = parts[0];
        TargetFrameworkVersion = parts[1];
      }
      catch (Exception ex)
      {
        this.Exception = $"Error reading TargetFrameworkAttribute: {ex.Message}";
      }
    }

    private static string ReadPublicKey(AssemblyName assemblyName)
    {
      var publicKeyToken = assemblyName.GetPublicKeyToken();
      if (publicKeyToken == null || publicKeyToken.Length == 0)
      {
        return null;
      }

      var builder = new StringBuilder(publicKeyToken.Length * 2);
      foreach (var b in publicKeyToken)
      {
        builder.AppendFormat($"{b:x2}");
      }
		
      return builder.ToString();
    }

    public XElement AssemblyBindingRedirect()
    {
      XNamespace asm = "urn:schemas-microsoft-com:asm.v1";
      return new XElement(asm + "dependentAssembly",
        new XElement(
          asm + "assemblyIdentity",
          new XAttribute("name", this.Name),
          this.PublicKeyToken is null ? null : new XAttribute("publicKeyToken", this.PublicKeyToken)),
        new XElement(
          asm + "bindingRedirect",
          new XAttribute("oldVersion", "0.0.0.0-32767.32767.32767.32767"),
          new XAttribute("newVersion", this.Version)));
    }
  }
}
