// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Common.Assembly;
using Core.Common.Util;

namespace Application.Riskeer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        // Start application after this process will exit (used during restart)
        private const string argumentWaitForProcess = "--wait-for-process=";

        private static string fileToOpen = string.Empty;

        private static RiskeerRunner runner;

        public App()
        {
            SetupAssemblyResolver();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            runner.OnExit();
            base.OnExit(e);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            ParseArguments(e.Args);

            Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);

            runner = new RiskeerRunner(fileToOpen, this);
        }

        private static void SetupAssemblyResolver()
        {
            string assemblyDirectory = Path.Combine(
                Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName,
                "Application", "Built-in", "Managed");

            Assembly GetAssemblyResolver(object sender, ResolveEventArgs args)
            {
                return Assembly.LoadFile(Path.Combine(assemblyDirectory, "Core", "Core.Common.Assembly.dll"));
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

        private static bool ParseFileArgument(string potentialPath)
        {
            if (potentialPath.Length > 0)
            {
                try
                {
                    IOUtils.ValidateFilePath(potentialPath);
                    fileToOpen = potentialPath;
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Parses the process' start-up parameters.
        /// </summary>
        /// <param name="arguments">List of start-up parameters.</param>
        private static void ParseArguments(IEnumerable<string> arguments)
        {
            var argumentWaitForProcessRegex = new Regex("^" + argumentWaitForProcess + @"(?<processId>\d+)$", RegexOptions.IgnoreCase);
            foreach (string arg in arguments)
            {
                Match match = argumentWaitForProcessRegex.Match(arg);
                if (match.Success)
                {
                    int pid = int.Parse(match.Groups["processId"].Value);
                    if (pid > 0)
                    {
                        RiskeerRunner.WaitForProcessId = pid;
                        break;
                    }
                }

                if (ParseFileArgument(arg))
                {
                    break;
                }
            }
        }
    }
}