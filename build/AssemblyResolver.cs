// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Application
{
    internal static class AssemblyResolver
    {
        private const string resourcesDllPattern = ".resources.dll";
        private const string dllPattern = "*.dll";

        /// <summary>
        /// On startup, we build a dictionary of assembly names to their paths for quick lookup.
        /// </summary>
        private static readonly Dictionary<string, string> assemblyPaths = CreateAssemblyPaths();

        /// <summary>
        /// Resolves assemblies from the prebuilt assembly-name-to-path lookup. 
        /// </summary>
        /// <remarks>
        /// Invoked by the AppDomain assembly resolution mechanism when normal probing fails due to dll's not being next to the dll.
        /// Loads the matching assembly from disk using its full assembly name
        /// </remarks>
        internal static Assembly ResolveAssembly(
            object sender,
            ResolveEventArgs args)
        {
            return assemblyPaths.TryGetValue(args.Name, out var assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }

        /// <summary>
        /// Gets the root folder of the managed assemblies.
        /// </summary>
        private static string AssembliesDirectory()
        {
            return Path.Combine(GetApplicationDirectory(), "Built-in", "Managed");
        }

        /// <summary>
        /// Gets the root folder of the application, test environment aware. 
        /// </summary>
        /// <remarks>
        /// This code is a duplication of the AssemblyHelper.GetApplicationDirectory();
        /// </remarks>
        private static string GetApplicationDirectory()
        {
            DirectoryInfo rootDirectoryInfo = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            while (rootDirectoryInfo.GetDirectories().All(di => di.Name != "Application"))
            {
                rootDirectoryInfo = Directory.GetParent(rootDirectoryInfo.FullName);
            }

            return Path.Combine(rootDirectoryInfo.FullName, "Application");
        }

        /// <summary>
        /// Creates an AssemblyPath from a DLL if it contains a valid managed assembly.
        /// </summary>
        /// <remarks>
        /// Returns null for native DLLs or assemblies whose metadata cannot be inspected.
        /// This allows mixed managed and unmanaged binaries to coexist in the search path.
        /// </remarks>
        private static AssemblyPath TryCreateAssemblyPath(string file)
        {
            try
            {
                return new AssemblyPath(
                    AssemblyName.GetAssemblyName(file),
                    file);
            }
            catch (BadImageFormatException)
            {
                // Native DLL, for example SQLite.Interop.dll.
                // These do not contain a managed assembly manifest.
                return null;
            }
            catch (FileLoadException)
            {
                // DLL exists, but cannot be inspected as a managed assembly.
                return null;
            }
        }

        /// <summary>
        /// Builds a lookup of managed assemblies discovered under the configured root directory.
        /// </summary>
        /// <remarks>
        /// Recursively scans for DLLs, ignores resource assemblies, and indexes assemblies
        /// by their full assembly identity. Throws if multiple files expose the same assembly
        /// name to prevent ambiguous resolution.
        /// </remarks>
        private static Dictionary<string, string> CreateAssemblyPaths()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ap in Directory
                               .EnumerateFiles(AssembliesDirectory(), dllPattern, SearchOption.AllDirectories)
                               .Where(file => !file.EndsWith(
                                                  resourcesDllPattern,
                                                  StringComparison.OrdinalIgnoreCase))
                               .Select(TryCreateAssemblyPath)
                               .Where(ap => ap != null))
            {
                if (!result.TryGetValue(ap.AssemblyName.FullName, out string existingPath))
                {
                    result.Add(ap.AssemblyName.FullName, ap.Path);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Duplicate assembly name '{ap.AssemblyName.FullName}' found at '{ap.Path}' and '{existingPath}'.");
                }
            }

            return result;
        }

        /// <summary>
        /// Associates an assembly identity with its physical file location.
        /// </summary>
        private sealed class AssemblyPath
        {
            public AssemblyPath(
                AssemblyName assemblyName,
                string path)
            {
                AssemblyName = assemblyName;
                Path = path;
            }

            public AssemblyName AssemblyName { get; }

            public string Path { get; }
        }
    }
}