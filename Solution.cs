using System.Collections.Generic;

using Cake.Core.IO;

using Newtonsoft.Json;

namespace Dnn.CakeUtils
{
    public class Solution
    {
        public string name { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public DnnSolution dnn { get; set; }
        public static Solution New(FilePath packageJsonFilePath)
        {
            var sln = JsonConvert.DeserializeObject<Solution>(Utilities.ReadFile(packageJsonFilePath));
            foreach (var folder in sln.dnn.projectFolders)
            {
                var subproject = JsonConvert.DeserializeObject<Project>(Utilities.ReadFile(folder + "\\dnn.json"));
                sln.dnn.projects[folder] = subproject;
            }
            return sln;
        }
    }

    public class DnnSolution
    {
        public string[] projectFolders { get; set; }
        public Dictionary<string, Project> projects { get; set; } = new Dictionary<string, Project>();
        public Owner owner { get; set; }
        public DnnSolutionPathsAndFiles pathsAndFiles { get; set; }
    }

    public class DnnSolutionPathsAndFiles
    {
        public string devFolder { get; set; } = ".";
        public string solutionFile { get; set; }
        public string devSiteUrl { get; set; }
        public string devSitePath { get; set; }
        public string pathToAssemblies { get; set; } = "./bin";
        public string[] excludeFilter { get; set; }
        public string licenseFile { get; set; } = "";
        public string releaseNotesFile { get; set; } = "";
        public string[] releaseFiles { get; set; }
        public string packagesPath { get; set; } = "./Releases";
        public string zipName { get; set; }
        public string packageAssembliesFolder { get; set; } = "bin";
        public string packageScriptsFolder { get; set; } = "scripts";
        public string packageCleanupFolder { get; set; } = "cleanup";
    }

    public class Owner
    {
        public string name { get; set; }
        public string organization { get; set; }
        public string url { get; set; }
        public string email { get; set; }
    }
}
