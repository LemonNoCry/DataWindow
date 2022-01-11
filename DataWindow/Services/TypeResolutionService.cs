using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace DataWindow.Services
{
    public class TypeResolutionService : ITypeResolutionService, ITypeDiscoveryService
    {
        private readonly List<Assembly> assemblies;

        public TypeResolutionService()
        {
            assemblies = new List<Assembly>();
            assemblies.AddRange(new[]
            {
                typeof(Size).Assembly,
                typeof(Control).Assembly,
                typeof(DataSet).Assembly,
                typeof(XmlElement).Assembly
            });
        }

        public ICollection GetTypes(Type baseType, bool excludeGlobalTypes)
        {
            var list = new List<Type>();
            if (baseType == null) baseType = typeof(object);

            Func<Type, bool> func = null;
            foreach (var assembly in assemblies)
                if (!excludeGlobalTypes || !assembly.GlobalAssemblyCache)
                {
                    var list2 = list;
                    IEnumerable<Type> types = assembly.GetTypes();
                    Func<Type, bool> predicate;
                    if ((predicate = func) == null) predicate = func = t => t.IsSubclassOf(baseType);

                    list2.AddRange(types.Where(predicate));
                }

            return list;
        }

        public Assembly GetAssembly(AssemblyName name, bool throwOnError)
        {
            var assembly = assemblies.Find(a => a.GetName().FullName.CompareTo(name.FullName) == 0);
            if (assembly != null) return assembly;

            try
            {
                assembly = Assembly.Load(name);
            }
            catch (Exception ex)
            {
                if (throwOnError) throw ex;
            }

            if (assembly != null)
            {
                assemblies.Add(assembly);
                return assembly;
            }

            return null;
        }

        public Assembly GetAssembly(AssemblyName name)
        {
            return GetAssembly(name, false);
        }

        public string GetPathOfAssembly(AssemblyName name)
        {
            var assembly = assemblies.Find(a => a.GetName().FullName.CompareTo(name.FullName) == 0);
            if (assembly == null) return null;

            return assembly.Location;
        }

        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            var type = Type.GetType(name, throwOnError, ignoreCase);
            if (type == null)
                assemblies.Exists(delegate(Assembly assembly)
                {
                    type = assembly.GetType(name, false, ignoreCase);
                    return type != null;
                });

            if (type == null && throwOnError) throw new TypeLoadException("未找到类型 " + name);

            return type;
        }

        public Type GetType(string name, bool throwOnError)
        {
            return GetType(name, throwOnError, false);
        }

        public Type GetType(string name)
        {
            return GetType(name, false, false);
        }

        public void ReferenceAssembly(AssemblyName name)
        {
            GetAssembly(name, false);
        }
    }
}