// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Windows.Input;

namespace Core.Gui.Commands
{
    /// <summary>
    /// Provides a set of commands for the main window.
    /// </summary>
    public static class MainWindowCommands
    {
        /// <summary>
        /// The command for creating a new project.
        /// </summary>
        public static readonly ICommand NewProjectCommand = new RoutedCommand();

        /// <summary>
        /// The command for saving the project.
        /// </summary>
        public static readonly ICommand SaveProjectCommand = new RoutedCommand();

        /// <summary>
        /// The command for saving the project at a new location.
        /// </summary>
        public static readonly ICommand SaveProjectAsCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening a project.
        /// </summary>
        public static readonly ICommand OpenProjectCommand = new RoutedCommand();

        /// <summary>
        /// The command for closing the application.
        /// </summary>
        public static readonly ICommand CloseApplicationCommand = new RoutedCommand();

        /// <summary>
        /// The command for toggling the backstage.
        /// </summary>
        public static readonly ICommand ToggleBackstageCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening the log file.
        /// </summary>
        public static readonly ICommand OpenLogFileCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening the user manual.
        /// </summary>
        public static readonly ICommand OpenUserManualCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening the support desk website.
        /// </summary>
        public static readonly ICommand OpenSupportDeskWebsiteCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening the support desk call information website.
        /// </summary>
        public static readonly ICommand OpenCallSupportDeskWebsiteCommand = new RoutedCommand();

        /// <summary>
        /// The command for opening the support desk email information website.
        /// </summary>
        public static readonly ICommand OpenEmailSupportDeskWebsiteCommand = new RoutedCommand();
    }
}