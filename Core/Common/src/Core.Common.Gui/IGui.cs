// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Storage;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.Selection;
using Core.Common.Gui.Settings;

namespace Core.Common.Gui
{
    /// <summary>
    /// Provides graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui : ICommandsOwner, ISettingsOwner, IProjectOwner,
                            IApplicationSelection, IViewController, IContextMenuBuilderProvider,
                            IMainWindowController, IGuiPluginsHost, IDisposable
    {
        /// <summary>
        /// Gets the object responsible for retrieving the <see cref="ObjectProperties{T}"/>
        /// instance for a given data object for the application to use.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        /// Gets the current project storage.
        /// </summary>
        IStoreProject Storage { get; }

        /// <summary>
        /// Runs the user interface, causing all user interface components to initialize, 
        /// loading plugins and displaying the main window.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs the user interface, causing all user interface components to initialize, 
        /// loading plugins, opening a saved project and displaying the main window.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened.</param>
        void Run(string projectPath);

        /// <summary>
        /// Terminates the application.
        /// </summary>
        void Exit();
    }
}