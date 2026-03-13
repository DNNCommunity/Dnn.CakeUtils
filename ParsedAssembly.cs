using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Dnn.CakeUtils
{
  public class ParsedAssembly
  {
    public string Name { get; set; } = "";
    public string PublicKeyToken { get; set; } = "";
    public string FilePath { get; set; } = "";
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

      try
      {
        var targetFrameworkAttributes = assembly.CustomAttributes
              .Where(attribute => attribute.AttributeType.Name == nameof(TargetFrameworkAttribute));
        var customAttribute = targetFrameworkAttributes.FirstOrDefault();
        var customAttributeValue = customAttribute?.ConstructorArguments.FirstOrDefault().Value.ToString();
        this.PublicKeyToken = ReadPublicKey(details);
        if (string.IsNullOrEmpty(customAttributeValue))
        {
          this.Exception = "No TargetFrameworkAttribute found";
          return;
        }

        var parts = customAttributeValue.Split(',');
        TargetFramework = parts[0];
        TargetFrameworkVersion = parts[1];
      }
      catch (Exception ex)
      {
        this.Exception = $"Error reading TargetFrameworkAttribute: {ex.Message}";
        return;
      }

      var references = assembly.GetReferencedAssemblies();
      foreach (var reference in references.OrderBy(r => r.Name))
      {
        References.Add($"{reference.Name} {reference.Version}");
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

    public string AssemblyBindingRedirect()
    {
      return $@"      <dependentAssembly>
        <assemblyIdentity name=""{this.Name}"" publicKeyToken=""{PublicKeyToken}"" culture=""neutral"" />
        <bindingRedirect oldVersion=""0.0.0.0-32767.32767.32767.32767"" newVersion=""{Version}""/>
      </dependentAssembly>";
    }
  }
}
