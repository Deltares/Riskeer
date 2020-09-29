// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Assembly;
using NUnit.Framework.Internal;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Helper that contains methods which can be used within <see cref="SetUpFixture"/> classes.
    /// </summary>
    public static class SetUpFixtureHelper
    {
#if DEBUG
        private const string configuration = "Debug";
#elif RELEASE
        private const string configuration = "Release";
#elif CREATEINSTALLER
        private const string configuration = "CreateInstaller";
#elif CREATEINSTALLERWITHDEMOPLUGIN
        private const string configuration = "CreateInstallerWithDemoPlugin";
#elif RELEASEFORCODECOVERAGE
        private const string configuration = "ReleaseForCodeCoverage";
#endif

        /// <summary>
        /// Sets up the <see cref="AssemblyResolver"/>.
        /// </summary>
        public static void SetupAssemblyResolver()
        {
            string executingAssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string solutionRoot = GetSolutionRoot(Directory.GetParent(executingAssemblyLocation));
            string executableDirectory = Path.Combine(solutionRoot, "bin", configuration);

            System.Reflection.Assembly GetAssemblyResolver(object sender, ResolveEventArgs args)
            {
                return System.Reflection.Assembly.LoadFile(
                    Path.Combine(executableDirectory, "Application", "Built-in",
                                 "Managed", "Core", "Common.Assembly.dll"));
            }

            AppDomain.CurrentDomain.AssemblyResolve += GetAssemblyResolver;

            InitializeAssemblyResolver(executableDirectory);

            AppDomain.CurrentDomain.AssemblyResolve -= GetAssemblyResolver;
        }

        private static string GetSolutionRoot(DirectoryInfo directory)
        {
            return directory.GetFiles().Any(f => f.Extension.Equals(".sln"))
                       ? directory.FullName
                       : GetSolutionRoot(directory.Parent);
        }

        private static void InitializeAssemblyResolver(string assemblyDirectory)
        {
            if (AssemblyResolver.RequiresInitialization)
            {
                AssemblyResolver.Initialize(assemblyDirectory);
            }
        }
    }
}