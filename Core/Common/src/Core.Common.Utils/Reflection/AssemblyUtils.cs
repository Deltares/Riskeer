﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.Common.Utils.Reflection
{
    /// <summary>
    /// Utility class containing functions for retrieving specific information about a .net assembly.
    /// </summary>
    public static class AssemblyUtils
    {
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
                        throw new AmbiguousMatchException(string.Format("Type '{0}' found in multiple assemblies", typeName));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> to an embedded resource of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the embedded resource.</param>
        /// <param name="fileName">Name of the embedded file.</param>
        /// <returns>The byte-stream to the embedded resource.</returns>
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