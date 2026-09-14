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
using System.Runtime.Loader;

namespace AssemblyResolver
{
    /// <summary>
    /// Subscribes <see cref="AssemblyResolver"/> to the assembly resolution events of the running process.
    /// </summary>
    /// <remarks>
    /// This type is compiled into multiple assemblies (the startup hook as well as every test assembly), each with
    /// their own copy of the static state. The installation is therefore guarded by a process-wide flag so that
    /// whichever copy runs first wins and the assembly lookup is only built once.
    /// </remarks>
    internal static class AssemblyResolverInstaller
    {
        private const string installedDataKey = "Riskeer.AssemblyResolverInstalled";

        /// <summary>
        /// Installs the assembly resolver, unless it was already installed in this process.
        /// </summary>
        internal static void Install()
        {
            if (AppDomain.CurrentDomain.GetData(installedDataKey) != null)
            {
                return;
            }

            AppDomain.CurrentDomain.SetData(installedDataKey, bool.TrueString);

            AssemblyLoadContext.Default.Resolving += (context, assemblyName) => AssemblyResolver.ResolveAssembly(assemblyName);
        }
    }
}
