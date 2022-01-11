using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

namespace DataWindow.Toolbox
{
    public class ControlLibraryManager
    {
        private readonly List<ComponentAssembly> assemblies = new List<ComponentAssembly>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public bool Load(string fileName)
        {
            if (!File.Exists(fileName)) return false;
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                if (xmlDocument.DocumentElement.Name != "ControlLibrary") return false;
                foreach (var obj in xmlDocument.DocumentElement["Assemblies"].ChildNodes)
                {
                    var xmlNode = (XmlNode) obj;
                    if (xmlNode.Name == "Assembly")
                    {
                        var innerText = xmlNode.Attributes["assembly"].InnerText;
                        if (xmlNode.Attributes["path"] != null)
                            assemblies.Add(new ComponentAssembly(innerText, xmlNode.Attributes["path"].InnerText));
                        else
                            assemblies.Add(new ComponentAssembly(innerText));
                    }
                }

                foreach (var obj2 in xmlDocument.DocumentElement["Categories"].ChildNodes)
                {
                    var xmlNode2 = (XmlNode) obj2;
                    if (xmlNode2.Name == "Category")
                    {
                        var category = new Category(xmlNode2.Attributes["name"].InnerText);
                        foreach (var obj3 in xmlNode2.ChildNodes)
                        {
                            var xmlNode3 = (XmlNode) obj3;
                            var item = new ToolComponent(xmlNode3.Attributes["class"].InnerText, assemblies[int.Parse(xmlNode3.Attributes["assembly"].InnerText)], IsEnabled(xmlNode3.Attributes["enabled"]));
                            category.ToolComponents.Add(item);
                        }

                        Categories.Add(category);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void RemoveCategory(string name)
        {
            foreach (var category in Categories)
                if (category.Name == name)
                {
                    Categories.Remove(category);
                    break;
                }
        }

        public void DisableToolComponent(string categoryName, string fullName)
        {
            foreach (var category in Categories)
                if (category.Name == categoryName)
                    foreach (var toolComponent in category.ToolComponents)
                        if (toolComponent.FullName == fullName)
                        {
                            toolComponent.IsEnabled = false;
                            return;
                        }
        }

        public void ExchangeToolComponents(string categoryName, string fullName1, string fullName2)
        {
            foreach (var category in Categories)
                if (category.Name == categoryName)
                {
                    var num = -1;
                    var num2 = -1;
                    for (var i = 0; i < category.ToolComponents.Count; i++)
                    {
                        var toolComponent = category.ToolComponents[i];
                        if (toolComponent.FullName == fullName1)
                            num = i;
                        else if (toolComponent.FullName == fullName2) num2 = i;
                        if (num != -1 && num2 != -1)
                        {
                            var value = category.ToolComponents[num];
                            category.ToolComponents[num] = category.ToolComponents[num2];
                            category.ToolComponents[num2] = value;
                            return;
                        }
                    }
                }
        }

        public Bitmap GetIcon(ToolComponent component)
        {
            var assembly = component.LoadAssembly();
            var type = assembly.GetType(component.FullName);
            Bitmap bitmap = null;
            if (type != null)
                foreach (var obj in type.GetCustomAttributes(false))
                    if (obj is ToolboxBitmapAttribute)
                    {
                        bitmap = new Bitmap(((ToolboxBitmapAttribute) obj).GetImage(type));
                        bitmap.MakeTransparent();
                        break;
                    }

            if (bitmap == null)
                try
                {
                    var manifestResourceStream = assembly.GetManifestResourceStream(component.FullName + ".bmp");
                    if (manifestResourceStream != null)
                    {
                        bitmap = new Bitmap(Image.FromStream(manifestResourceStream));
                        bitmap.MakeTransparent();
                    }
                }
                catch (Exception)
                {
                }

            return bitmap;
        }

        public ToolComponent GetToolComponent(string assemblyName)
        {
            foreach (var category in Categories)
            {
                foreach (var toolComponent in category.ToolComponents)
                    if (toolComponent.AssemblyName == assemblyName)
                        return toolComponent;
            }

            return null;
        }

        public void Save(string fileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<ControlLibrary />");
            var hashtable = new Hashtable();
            var xmlElement = xmlDocument.CreateElement("Assemblies");
            xmlDocument.DocumentElement.AppendChild(xmlElement);
            for (var i = 0; i < assemblies.Count; i++)
            {
                var componentAssembly = assemblies[i];
                hashtable[componentAssembly.Name] = i;
                var xmlElement2 = xmlDocument.CreateElement("Assembly");
                xmlElement2.SetAttribute("assembly", componentAssembly.Name);
                if (componentAssembly.HintPath != null) xmlElement2.SetAttribute("path", componentAssembly.HintPath);
                xmlElement.AppendChild(xmlElement2);
            }

            var xmlElement3 = xmlDocument.CreateElement("Categories");
            xmlDocument.DocumentElement.AppendChild(xmlElement3);
            foreach (var category in Categories)
            {
                var xmlElement4 = xmlDocument.CreateElement("Category");
                xmlElement4.SetAttribute("name", category.Name);
                xmlElement4.SetAttribute("enabled", category.IsEnabled.ToString());
                xmlElement3.AppendChild(xmlElement4);
                foreach (var toolComponent in category.ToolComponents)
                {
                    var xmlElement5 = xmlDocument.CreateElement("ToolComponent");
                    xmlElement5.SetAttribute("class", toolComponent.FullName);
                    if (hashtable[toolComponent.AssemblyName] == null)
                    {
                        var xmlElement6 = xmlDocument.CreateElement("Assembly");
                        xmlElement6.SetAttribute("assembly", toolComponent.AssemblyName);
                        if (toolComponent.HintPath != null) xmlElement6.SetAttribute("path", toolComponent.HintPath);
                        xmlElement.AppendChild(xmlElement6);
                        hashtable[toolComponent.AssemblyName] = hashtable.Values.Count;
                    }

                    xmlElement5.SetAttribute("assembly", hashtable[toolComponent.AssemblyName].ToString());
                    xmlElement5.SetAttribute("enabled", toolComponent.IsEnabled.ToString());
                    xmlElement4.AppendChild(xmlElement5);
                }
            }

            xmlDocument.Save(fileName);
        }

        private bool IsEnabled(XmlAttribute attribute)
        {
            if (attribute != null && attribute.InnerText != null)
            {
                var result = true;
                if (bool.TryParse(attribute.InnerText, out result)) return result;
            }

            return true;
        }
    }
}