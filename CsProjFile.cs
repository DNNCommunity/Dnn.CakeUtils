using System.Xml;

namespace Dnn.CakeUtils
{
    public class CsProjFile : XmlDocument
    {
        private string FilePath { get; set; }

        public bool IsNetCore { get; set; } = false;

        public CsProjFile(string filePath)
        {
            FilePath = filePath;
            base.Load(filePath);
            if (this.DocumentElement.HasAttribute("Sdk"))
            {
                IsNetCore = true;
            }
        }

        public void SetProperty(string propName, string propValue)
        {
            var found = false;
            foreach (XmlElement propGrp in DocumentElement.ChildNodes)
            {
                if (propGrp.Name == "PropertyGroup" && !propGrp.HasAttribute("Condition"))
                {
                    foreach (XmlElement prop in propGrp.ChildNodes)
                    {
                        if (prop.Name == propName)
                        {
                            prop.InnerText = propValue;
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                var newProp = CreateElement(propName);
                newProp.InnerText = propValue;
                foreach (XmlElement propGrp in DocumentElement.ChildNodes)
                {
                    if (propGrp.Name == "PropertyGroup" && !propGrp.HasAttribute("Condition"))
                    {
                        propGrp.AppendChild(newProp);
                        found = true;
                    }
                }
                if (!found)
                {
                    var newGrp = CreateElement("PropertyGroup");
                    newGrp.AppendChild(newProp);
                    DocumentElement.AppendChild(newGrp);
                }
            }
        }

        public void Write()
        {
            Write(FilePath);
        }
        public void Write(string filePath)
        {
            Save(filePath);
        }

    }
}
