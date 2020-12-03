using System;
using System.Xml;

namespace Dnn.CakeUtils.Manifest
{
    public partial class Manifest : XmlDocument
    {
        private XmlNode PackagesNode { get; set; }
        private Solution Solution { get; set; }

        public Manifest(Solution sln) : base()
        {
            Console.WriteLine("Creating Manifest");
            Solution = sln;
            // Set up document
            var rootNode = CreateElement("dotnetnuke");
            AppendChild(rootNode);
            rootNode.AddAttribute("type", "Package");
            rootNode.AddAttribute("version", "5.0");
            PackagesNode = rootNode.AddChildElement("packages");
            foreach (var pFolder in sln.dnn.projectFolders)
            {
                var project = Solution.dnn.projects[pFolder];
                if (!project.packageSeparately)
                {
                    AddPackage(project);
                }
            }
            Console.WriteLine("Finished Creating Manifest");
        }

        public Manifest(Solution sln, Project project) : base()
        {
            Console.WriteLine("Creating Manifest");
            Solution = sln;
            // Set up document
            var rootNode = CreateElement("dotnetnuke");
            AppendChild(rootNode);
            rootNode.AddAttribute("type", "Package");
            rootNode.AddAttribute("version", "5.0");
            PackagesNode = rootNode.AddChildElement("packages");
            AddPackage(project);
            Console.WriteLine("Finished Creating Manifest");
        }

        private void AddPackage(Project project)
        {
            var package = PackagesNode.AddChildElement("package");
            package.AddAttribute("name", project.packageName);
            // Set core attributes
            package.SetAttribute("version", Solution.version);
            package.SetChildElement("friendlyName", project.friendlyName);
            package.SetChildElement("description", project.description.Otherwise(Solution.description));
            package.SetChildElement("iconFile", project.iconFile);
            // Add owner
            var owner = package.SetChildElement("owner");
            owner.SetChildElement("name", Solution.dnn.owner.name);
            owner.SetChildElement("organization", Solution.dnn.owner.organization);
            owner.SetChildElement("url", Solution.dnn.owner.url);
            owner.SetChildElement("email", Solution.dnn.owner.email);
            // core dependency
            var coreRef = string.IsNullOrEmpty(project.dnnDependency) ?
                Common.GetCoreDependency(Solution.dnn.pathsAndFiles.pathToAssemblies) :
                project.dnnDependency;
            if (coreRef != "00.00.00")
            {
                var dependencies = package.SetChildElement("dependencies");
                if (dependencies.SelectNodes("dependency[type='CoreVersion']").Count == 0)
                {
                    var coredep = dependencies.AddChildElement("dependency");
                    coredep.SetAttribute("type", "CoreVersion");
                    coredep.InnerText = coreRef;
                }
            }
            if (!string.IsNullOrEmpty(project.pathsAndFiles.licenseFile))
            {
                package.SetChildElement("license").SetAttribute("src", project.packageName.NoSlashes() + "/License.txt");
            }
            else if (!string.IsNullOrEmpty(Solution.dnn.pathsAndFiles.licenseFile) && !project.packageSeparately)
            {
                package.SetChildElement("license").SetAttribute("src", "License.txt");
            }
            if (!string.IsNullOrEmpty(project.pathsAndFiles.releaseNotesFile))
            {
                package.SetChildElement("releaseNotes").SetAttribute("src", project.packageName.NoSlashes() + "/ReleaseNotes.txt");
            }
            else if (!string.IsNullOrEmpty(Solution.dnn.pathsAndFiles.releaseNotesFile) && !project.packageSeparately)
            {
                package.SetChildElement("releaseNotes").SetAttribute("src", "ReleaseNotes.txt");
            }
            // Now for the components
            var components = CreateElement("components");

            switch (project.projectType)
            {
                case ProjectType.module:
                    package.AddAttribute("type", "Module");
                    package.SetChildElement("azureCompatible", project.module.azureCompatible);
                    components.AppendChild(project.module.ToXml(components));
                    if (project.config != null)
                    {
                        components.AppendChild(project.config.ToXml(components));
                    }

                    components.AddScripts(project, Solution.version, Solution.dnn.pathsAndFiles.packageScriptsFolder);
                    components.AddAssemblies(project, Solution.dnn.pathsAndFiles.pathToAssemblies, Solution.dnn.pathsAndFiles.packageAssembliesFolder);
                    components.AddCleanupFiles(project, Solution.dnn.pathsAndFiles.packageCleanupFolder);
                    components.AddResourceComponent(project);
                    package.AppendChild(components);
                    break;
                case ProjectType.library:
                    package.AddAttribute("type", "Library");
                    if (project.config != null)
                    {
                        components.AppendChild(project.config.ToXml(components));
                    }

                    components.AddScripts(project, Solution.version, Solution.dnn.pathsAndFiles.packageScriptsFolder);
                    components.AddAssemblies(project, Solution.dnn.pathsAndFiles.pathToAssemblies, Solution.dnn.pathsAndFiles.packageAssembliesFolder);
                    components.AddCleanupFiles(project, Solution.dnn.pathsAndFiles.packageCleanupFolder);
                    package.AppendChild(components);
                    break;
                case ProjectType.skin:
                    package.AddAttribute("type", "Skin");
                    components.AddSkinComponent(project);
                    package.AppendChild(components);
                    break;
                case ProjectType.container:
                    package.AddAttribute("type", "Container");
                    components.AddContainerComponent(project);
                    package.AppendChild(components);
                    break;
                case ProjectType.provider:
                    package.AddAttribute("type", "Provider");
                    if (project.config != null)
                    {
                        components.AppendChild(project.config.ToXml(components));
                    }

                    components.AddScripts(project, Solution.version, Solution.dnn.pathsAndFiles.packageScriptsFolder);
                    components.AddAssemblies(project, Solution.dnn.pathsAndFiles.pathToAssemblies, Solution.dnn.pathsAndFiles.packageAssembliesFolder);
                    components.AddCleanupFiles(project, Solution.dnn.pathsAndFiles.packageCleanupFolder);
                    package.AppendChild(components);
                    break;
                case ProjectType.auth_system:
                    package.AddAttribute("type", "Auth_System");
                    components.AddAuthSystemComponent(project);
                    if (project.config != null)
                    {
                        components.AppendChild(project.config.ToXml(components));
                    }

                    components.AddScripts(project, Solution.version, Solution.dnn.pathsAndFiles.packageScriptsFolder);
                    components.AddAssemblies(project, Solution.dnn.pathsAndFiles.pathToAssemblies, Solution.dnn.pathsAndFiles.packageAssembliesFolder);
                    components.AddCleanupFiles(project, Solution.dnn.pathsAndFiles.packageCleanupFolder);
                    components.AddResourceComponent(project);
                    package.AppendChild(components);
                    break;
                case ProjectType.personabar:
                    package.AddAttribute("type", "PersonaBar");
                    components.AddPersonabarComponent(project);
                    if (project.config != null)
                    {
                        components.AppendChild(project.config.ToXml(components));
                    }

                    components.AddScripts(project, Solution.version, Solution.dnn.pathsAndFiles.packageScriptsFolder);
                    components.AddAssemblies(project, Solution.dnn.pathsAndFiles.pathToAssemblies, Solution.dnn.pathsAndFiles.packageAssembliesFolder);
                    components.AddCleanupFiles(project, Solution.dnn.pathsAndFiles.packageCleanupFolder);
                    components.AddResourceComponent(project);
                    package.AppendChild(components);
                    break;
            }
        }
    }
}
