using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public DnnModule module { get; set; }
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
    }

    public class DnnModuleDefinition
    {
        public string friendlyName { get; set; }
        public int defaultCacheTime { get; set; } = -1;
        public DnnModuleControl[] moduleControls { get; set; }
    }

    public class DnnModuleControl
    {
        public string controlKey { get; set; }
        public string controlSrc { get; set; }
        public string supportsPartialRendering { get; set; }
        public string controlType { get; set; }
        public string iconFile { get; set; }
        public string helpUrl { get; set; }
        public int viewOrder { get; set; } = 0;
    }

    public class ProjectPathsAndFiles
    {
        public string pathToScripts { get; set; } = "";
        public string pathToCleanupFiles { get; set; } = "";
        public string[] assemblies { get; set; }
        public string[] excludeFilter { get; set; }
        public string[] releaseFiles { get; set; }
    }

    public enum ProjectType
    {
        module,
        skin,
        container,
        library
    }
}
