using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Common.Utils.Properties;
using log4net;

namespace Core.Common.Utils.Reflection
{
    /// <summary>
    /// Utility class containing functions for retrieving specific information about a .net assembly.
    /// </summary>
    public static class AssemblyUtils
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssemblyUtils));

        /// <summary>
        /// Return attributes for a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly to read.</param>
        /// <returns>A structure containing all the assembly info provided.</returns>
        public static AssemblyInfo GetAssemblyInfo(Assembly assembly)
        {
            AssemblyInfo info = new AssemblyInfo();

            if (assembly.Location == "")
            {
                return info;
            }

            info.Version = assembly.GetName().Version.ToString();

            var assemblyTitleAttribute = GetAssemblyAttributeValue<AssemblyTitleAttribute>(assembly);
            if (assemblyTitleAttribute != null)
            {
                info.Title = assemblyTitleAttribute.Title;
            }

            var assemblyDescriptionAttribute = GetAssemblyAttributeValue<AssemblyDescriptionAttribute>(assembly);
            if (assemblyDescriptionAttribute != null)
            {
                info.Description = assemblyDescriptionAttribute.Description;
            }

            var assemblyProductAttribute = GetAssemblyAttributeValue<AssemblyProductAttribute>(assembly);
            if (assemblyProductAttribute != null)
            {
                info.Product = assemblyProductAttribute.Product;
            }

            var assemblyCopyrightAttribute = GetAssemblyAttributeValue<AssemblyCopyrightAttribute>(assembly);
            if (assemblyCopyrightAttribute != null)
            {
                info.Copyright = assemblyCopyrightAttribute.Copyright;
            }

            var assemblyCompanyAttribute = GetAssemblyAttributeValue<AssemblyCompanyAttribute>(assembly);
            if (assemblyCompanyAttribute != null)
            {
                info.Company = assemblyCompanyAttribute.Company;
            }

            return info;
        }

        /// <summary>
        /// Return attributes for the current executing assembly.
        /// </summary>
        /// <returns>A structure containing all the assembly info provided.</returns>
        public static AssemblyInfo GetExecutingAssemblyInfo()
        {
            return GetAssemblyInfo(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Returns the type based on the full type name.
        /// </summary>
        /// <param name="typeName">Full type name.</param>
        /// <returns>The <see cref="Type"/> matching the string name, null otherwise.</returns>
        /// <exception cref="Exception">Specified type string is found in multiple assemblies.</exception>
        public static Type GetTypeByName(string typeName)
        {
            Type result = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type foundType = a.GetType(typeName);
                if (foundType != null)
                {
                    if (result == null)
                    {
                        result = foundType;
                    }
                    else
                    {
                        throw new Exception("Type found in multiple assemblies");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> to a embedded resource of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the embedded resource.</param>
        /// <param name="fileName">Name of the embedded file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Embedded resource file with name <paramref name="fileName"/>
        /// cannot be found in <paramref name="assembly"/>.</exception>
        public static Stream GetAssemblyResourceStream(Assembly assembly, string fileName)
        {
            try
            {
                return assembly.GetManifestResourceNames()
                               .Where(resourceName => resourceName.EndsWith(fileName))
                               .Select(assembly.GetManifestResourceStream)
                               .First();
            }
            catch (InvalidOperationException e)
            {
                var message = string.Format("Cannot find embedded resource file '{0}' in '{1}.",
                    fileName, assembly.FullName);
                throw new ArgumentException(message, "fileName", e);
            }
        }

        /// <summary>
        /// Loads all assemblies from a given directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>The assemblies that have been loaded.</returns>
        /// <exception cref="IOException"><paramref name="path"/> is a file name or a network error occurred.</exception>
        /// <exception cref="UnauthorizedAccessException">The called does not have the required permission.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains
        /// only white space or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both 
        /// exceed the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid.</exception>
        public static IEnumerable<Assembly> LoadAllAssembliesFromDirectory(string path)
        {
            foreach (string filename in Directory.GetFiles(path).Where(name => name.EndsWith(".dll")))
            {
                if (!IsManagedDll(filename))
                {
                    continue;
                }

                Assembly a;
                try
                {
                    a = Assembly.LoadFrom(filename);
                }
                catch (Exception exception)
                {
                    var assemblyName = Path.GetFileNameWithoutExtension(filename);
                    log.ErrorFormat(Resources.AssemblyUtils_LoadAllAssembliesFromDirectory_Could_not_read_assembly_information_for_0_1_,
                                    assemblyName, exception.Message);
                    continue;
                }

                yield return a;
            }
        }

        private static bool IsManagedDll(string path)
        {
            // Implementation based on http://www.darwinsys.com/file/

            if (!path.EndsWith(".dll", StringComparison.Ordinal) && !path.EndsWith(".exe", StringComparison.Ordinal))
            {
                return false;
            }

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Seek(0x3C, SeekOrigin.Begin); // PE, SizeOfHeaders starts at 0x3B, second byte is 0x80 for .NET
            int i1 = fs.ReadByte();

            fs.Seek(0x86, SeekOrigin.Begin); // 0x03 for managed code
            int i2 = fs.ReadByte();

            fs.Close();

            var isManagedDll = i1 == 0x80 && i2 == 0x03;

            return isManagedDll;
        }

        private static T GetAssemblyAttributeValue<T>(Assembly assembly) where T : class
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            return (T) attributes[0];
        }

        #region Nested type: AssemblyInfo

        /// <summary>
        /// structure containing assembly attributes as strings.
        /// </summary>
        /// <remarks>Values will be null if they were not specified.</remarks>
        [Serializable]
        public struct AssemblyInfo
        {
            /// <summary>
            /// The company specified in the assembly.
            /// </summary>
            public string Company;
            /// <summary>
            /// The copyright text specified in the assembly.
            /// </summary>
            public string Copyright;
            /// <summary>
            /// The description text specified in the assembly.
            /// </summary>
            public string Description;
            /// <summary>
            /// The product text specified in the assembly.
            /// </summary>
            public string Product;
            /// <summary>
            /// The title text specified in the assembly.
            /// </summary>
            public string Title;
            /// <summary>
            /// The version specified in the assembly.
            /// </summary>
            public string Version;
        }

        #endregion
    }
}