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

namespace AssemblyResolver
{
    /// <summary>
    /// Resolves assemblies from a prebuilt lookup of assembly names to their paths. Can be invoked by the AppDomain assembly
    /// resolution mechanism when normal probing fails due to assemblies not being next to the executable. Loads the matching
    /// assembly from disk using its full assembly name.
    /// </summary>
    internal static class AssemblyResolver
    {
        private const string resourcesDllPattern = ".resources.dll";
        private const string dllPattern = "*.dll";

        private static readonly Dictionary<string, string> assemblyPaths = CreateAssemblyPaths();

        /// <summary>
        /// Resolves an assembly.
        /// </summary>
        /// <param name="args">The arguments containing the assembly name to resolve.</param>
        /// <returns>The resolved assembly, or <c>null</c> if not found.</returns>
        internal static Assembly ResolveAssembly(ResolveEventArgs args)
        {
            return assemblyPaths.TryGetValue(args.Name, out string assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }

        /// <summary>
        /// Builds a lookup of managed assemblies discovered under the configured root directory.
        /// </summary>
        /// <remarks>
        /// Recursively scans for assemblies, ignores resource assemblies, and indexes assemblies by their full assembly identity.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if multiple files expose the same assembly name.</exception>
        private static Dictionary<string, string> CreateAssemblyPaths()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (AssemblyPath ap in Directory
                                        .EnumerateFiles(GetAssembliesDirectory(), dllPattern, SearchOption.AllDirectories)
                                        .Where(file => !file.EndsWith(resourcesDllPattern, StringComparison.OrdinalIgnoreCase))
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

        private static AssemblyPath TryCreateAssemblyPath(string file)
        {
            try
            {
                return new AssemblyPath(AssemblyName.GetAssemblyName(file), file);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
            catch (FileLoadException)
            {
                return null;
            }
        }

        private static string GetAssembliesDirectory()
        {
            return Path.Combine(GetApplicationDirectory(), "Built-in", "Managed");
        }

        private static string GetApplicationDirectory()
        {
            DirectoryInfo rootDirectoryInfo = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            while (rootDirectoryInfo.GetDirectories().All(di => di.Name != "Application"))
            {
                rootDirectoryInfo = Directory.GetParent(rootDirectoryInfo.FullName);
            }

            return Path.Combine(rootDirectoryInfo.FullName, "Application");
        }

        private sealed class AssemblyPath
        {
            public AssemblyPath(AssemblyName assemblyName, string path)
            {
                AssemblyName = assemblyName;
                Path = path;
            }

            public AssemblyName AssemblyName { get; }

            public string Path { get; }
        }
    }
}