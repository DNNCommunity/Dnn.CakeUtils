using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Dnn.CakeUtils
{
    public class Project
    {
        public string name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ProjectType projectType { get; set; }
        public string friendlyName { get; set; }
        public string description { get; set; }
        public string dnnDependency { get; set; }
        public string packageName { get; set; }
        public string folder { get; set; }
        public string iconFile { get; set; }
        public bool packageSeparately { get; set; } = false;
        public DnnModule module { get; set; }
        public DnnConfig config { get; set; }
        public DnnAuthSystem authenticationSystem { get; set; }
        public PersonaBarMenu personaBarMenu { get; set; }
        public ProjectPathsAndFiles pathsAndFiles { get; set; }
    }

    public class DnnModule
    {
        public string azureCompatible { get; set; } = "True";
        public string moduleName { get; set; }
        public string foldername { get; set; }
        public string businessControllerClass { get; set; }
        public string[] supportedFeatures { get; set; }
        public DnnModuleDefinition[] moduleDefinitions { get; set; }
        public DnnEventMessage eventMessage { get; set; }
    }

    public class DnnModuleDefinition
    {
        public string definitionName { get; set; }
        public string friendlyName { get; set; }
        public int defaultCacheTime { get; set; } = -1;
        public DnnModuleControl[] moduleControls { get; set; }
        public DnnModulePermission[] permissions { get; set; }
    }

    public class DnnModuleControl
    {
        public string controlKey { get; set; }
        public string controlTitle { get; set; }
        public string controlSrc { get; set; }
        public string supportsPartialRendering { get; set; }
        public string controlType { get; set; }
        public string iconFile { get; set; }
        public string helpUrl { get; set; }
        public int viewOrder { get; set; } = 0;
    }

    public class DnnModulePermission
    {
        public string code { get; set; }
        public string key { get; set; }
        public string name { get; set; }
    }

    public class DnnEventMessage
    {
        public string processorType { get; set; }
        public string processorCommand { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class DnnConfig
    {
        public string configFile { get; set; }
        public string install { get; set; }
        public string uninstall { get; set; }
    }

    public class ProjectPathsAndFiles
    {
        public string pathToScripts { get; set; } = "";
        public string pathToCleanupFiles { get; set; } = "";
        public string licenseFile { get; set; } = "";
        public string releaseNotesFile { get; set; } = "";
        public string zipName { get; set; }
        public string[] assemblies { get; set; }
        public string[] excludeFilter { get; set; }
        public string[] releaseFiles { get; set; }
    }

    public class DnnAuthSystem
    {
        public string type { get; set; } = "";
        public string settingsControlSrc { get; set; } = "";
        public string loginControlSrc { get; set; } = "";
        public string logoffControlSrc { get; set; } = "";
    }

    public class PersonaBarMenu
    {
        public string identifier { get; set; } = "";
        public string moduleName { get; set; } = "";
        public string controller { get; set; } = "";
        public string resourceKey { get; set; } = "";
        public string path { get; set; } = "";
        public bool mobileSupport { get; set; } = true;
        public string parent { get; set; } = "";
        public int order { get; set; } = 0;
    }

    public enum ProjectType
    {
        module,
        skin,
        container,
        library,
        provider,
        auth_system,
        personabar
    }
}
