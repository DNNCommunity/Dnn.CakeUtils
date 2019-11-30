using System.Xml;

namespace Dnn.CakeUtils.Manifest
{
    public static class Extensions
    {
        public static XmlNode ToXml(this DnnModule control, XmlNode parent)
        {
            var newNode = parent.AddChildElement("component").AddAttribute("type", "Module");
            var dtm = newNode.AddChildElement("desktopModule");
            dtm.AddChildElement("moduleName", control.moduleName);
            dtm.AddChildElement("foldername", control.foldername);
            dtm.AddChildElementIfNotNull("businessControllerClass", control.businessControllerClass);
            if (control.supportedFeatures != null && control.supportedFeatures.Length > 0)
            {
                var supfeats = dtm.AddChildElement("supportedFeatures");
                foreach (var sf in control.supportedFeatures)
                {
                    supfeats.AddChildElement("supportedFeature", "").AddAttribute("type", sf);
                }
                dtm.AppendChild(supfeats);
            }
            if (control.moduleDefinitions != null && control.moduleDefinitions.Length > 0)
            {
                var mdefs = dtm.AddChildElement("moduleDefinitions");
                foreach (var md in control.moduleDefinitions)
                {
                    mdefs.AppendChild(md.ToXml(mdefs));
                }
                dtm.AppendChild(mdefs);
            }
            if (control.eventMessage != null)
            {
                var evtMsg = newNode.AddChildElement("eventMessage");
                evtMsg.AddChildElement("processorType", control.eventMessage.processorType);
                evtMsg.AddChildElement("processorCommand", control.eventMessage.processorCommand);
                if (control.eventMessage.attributes.Count > 0)
                {
                    var atts = evtMsg.AddChildElement("attributes");
                    foreach (var propName in control.eventMessage.attributes.Keys)
                    {
                        atts.AddChildElement(propName, control.eventMessage.attributes[propName]);
                    }
                }
            }
            return newNode;
        }
        public static XmlNode ToXml(this DnnModuleDefinition control, XmlNode parent)
        {
            var newNode = parent.AddChildElement("moduleDefinition");
            newNode.AddChildElement("definitionName", control.definitionName);
            newNode.AddChildElement("friendlyName", control.friendlyName);
            newNode.AddChildElement("defaultCacheTime", control.defaultCacheTime.ToString());
            if (control.moduleControls != null && control.moduleControls.Length > 0)
            {
                var modcs = newNode.AddChildElement("moduleControls");
                foreach (var mc in control.moduleControls)
                {
                    modcs.AppendChild(mc.ToXml(modcs));
                }
                newNode.AppendChild(modcs);
            }
            if (control.permissions != null && control.permissions.Length > 0)
            {
                var perms = newNode.AddChildElement("permissions");
                foreach (var p in control.permissions)
                {
                    perms.AppendChild(p.ToXml(perms));
                }
                newNode.AppendChild(perms);
            }
            return newNode;
        }
        public static XmlNode ToXml(this DnnModuleControl control, XmlNode parent)
        {
            var newNode = parent.AddChildElement("moduleControl");
            newNode.AddChildElement("controlKey", control.controlKey == null ? "" : control.controlKey);
            newNode.AddChildElementIfNotNull("controlTitle", control.controlTitle);
            newNode.AddChildElementIfNotNull("controlSrc", control.controlSrc);
            newNode.AddChildElementIfNotNull("supportsPartialRendering", control.supportsPartialRendering);
            newNode.AddChildElementIfNotNull("controlType", control.controlType);
            newNode.AddChildElementIfNotNull("iconFile", control.iconFile);
            newNode.AddChildElementIfNotNull("helpUrl", control.helpUrl);
            newNode.AddChildElementIfNotNull("viewOrder", control.viewOrder.ToString());
            return newNode;
        }
        public static XmlNode ToXml(this DnnModulePermission permission, XmlNode parent)
        {
            var newNode = parent.AddChildElement("permission");
            newNode.AddAttribute("code", permission.code);
            newNode.AddAttribute("key", permission.key);
            newNode.AddAttribute("name", permission.name);
            return newNode;
        }
        public static XmlNode ToXml(this DnnConfig config, XmlNode parent)
        {
            var newNode = parent.AddChildElement("component").AddAttribute("type", "Config");
            var cfgNode = newNode.AddChildElement("config");
            cfgNode.AddChildElement("configFile", config.configFile);
            var inst = cfgNode.AddChildElement("install");
            if (System.IO.File.Exists(config.install))
            {
                inst.InnerXml = Utilities.ReadFile(config.install);
            }
            else
            {
                inst.InnerXml = config.install;
            }
            var uninst = cfgNode.AddChildElement("uninstall");
            if (System.IO.File.Exists(config.uninstall))
            {
                uninst.InnerXml = Utilities.ReadFile(config.uninstall);
            }
            else
            {
                uninst.InnerXml = config.uninstall;
            }
            return newNode;
        }
    }
}
