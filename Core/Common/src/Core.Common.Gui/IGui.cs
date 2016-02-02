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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using Core.Common.Base.Plugin;
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
    public interface IGui : ICommandsOwner, ISettingsOwner, IToolViewController, IProjectOwner,
                            IApplicationSelection, IDocumentViewController, IContextMenuBuilderProvider,
                            IMainWindowController, IGuiPluginsHost, IDisposable
    {
        /// <summary>
        /// Object responsible for retrieving the <see cref="ObjectProperties{T}"/> instance 
        /// for a given data object and wrapping that in a <see cref="DynamicPropertyBag"/> 
        /// for the application to be used.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationCore"/> of the <see cref="IGui"/>.
        /// </summary>
        ApplicationCore ApplicationCore { get; }

        /// <summary>
        /// Gets or sets the current storage.
        /// </summary>
        IStoreProject Storage { get; }

        /// <summary>
        /// Runs gui. Internally it runs <see cref="ApplicationCore"/>, initializes all user interface components, including 
        /// those loaded from plugins. After that it creates and shows main window.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs gui and opens a given project in gui.ApplicationCore.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened.</param>
        void Run(string projectPath);

        /// <summary>
        /// Exits gui by user request.
        /// </summary>
        void Exit();
    }
}