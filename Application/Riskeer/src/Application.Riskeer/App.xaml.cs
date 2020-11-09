﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Common.Assembly;

namespace Application.Riskeer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Creates a new instance of <see cref="App"/>.
        /// </summary>
        public App()
        {
            SetupAssemblyResolver();
            Initialize();
        }

        private static void SetupAssemblyResolver()
        {
            string assemblyDirectory = Path.Combine(GetApplicationDirectory(), "Built-in", "Managed");

            Assembly GetAssemblyResolver(object sender, ResolveEventArgs args)
            {
                return Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Core", "Core.Common.Assembly.dll"));
            }

            AppDomain.CurrentDomain.AssemblyResolve += GetAssemblyResolver;

            InitializeAssemblyResolver(assemblyDirectory);

            AppDomain.CurrentDomain.AssemblyResolve -= GetAssemblyResolver;
        }

        private static void InitializeAssemblyResolver(string assemblyDirectory)
        {
            if (AssemblyResolver.RequiresInitialization)
            {
                AssemblyResolver.Initialize(assemblyDirectory);
            }
        }

        private static string GetApplicationDirectory()
        {
            DirectoryInfo executingAssemblyDirectoryInfo = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            while (executingAssemblyDirectoryInfo.GetDirectories().All(di => di.Name != "Application"))
            {
                executingAssemblyDirectoryInfo = Directory.GetParent(executingAssemblyDirectoryInfo.FullName);
            }

            return Path.Combine(executingAssemblyDirectoryInfo.FullName, "Application");
        }
    }
}