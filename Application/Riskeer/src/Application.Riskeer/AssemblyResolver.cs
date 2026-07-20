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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Application.Riskeer.Integration.Test")]
namespace Application.Riskeer
{
    internal static class AssemblyResolver
    {
        internal static readonly string Root = Path.Combine(
            AppContext.BaseDirectory,
            @"Application\Built-in\Managed");

        /// <summary>
        /// On startup, we build a dictionary of assembly names to their paths for quick lookup.
        /// </summary>
        private static readonly Dictionary<string, string> assemblyPaths = CreateAssemblyPaths();

        private static Dictionary<string, string> CreateAssemblyPaths()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ap in Directory
                               .EnumerateFiles(Root, "*.dll", SearchOption.AllDirectories)
                               .Where(file => !file.EndsWith(
                                                  ".resources.dll",
                                                  StringComparison.OrdinalIgnoreCase))
                               .Select(TryCreateAssemblyPath)
                               .Where(ap => ap != null))
            {
                if (!result.ContainsKey(ap.AssemblyName.FullName))
                {
                    result.Add(ap.AssemblyName.FullName, ap.Path);
                }
                else
                {
                    Console.WriteLine($"Duplicate assembly path: {ap.AssemblyName.FullName}");
                }
            }

            return result;
        }

        internal static AssemblyPath TryCreateAssemblyPath(string file)
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

        internal sealed class AssemblyPath
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

        internal static Assembly ResolveAssembly(
            object sender,
            ResolveEventArgs args)
        {
            return assemblyPaths.TryGetValue(args.Name, out var assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }
    }
}