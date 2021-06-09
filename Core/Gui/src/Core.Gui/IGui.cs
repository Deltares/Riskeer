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

using System;
using Core.Common.Base.Storage;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.PropertyView;
using Core.Gui.PropertyBag;
using Core.Gui.Selection;
using Core.Gui.Settings;

namespace Core.Gui
{
    /// <summary>
    /// Provides the graphical user interface logic required to work with an application.
    /// </summary>
    public interface IGui : ICommandsOwner, ISettingsOwner, IProjectOwner,
                            IApplicationSelection, IViewController, IContextMenuBuilderProvider,
                            IMainWindowController, IPluginsHost, IDisposable
    {
        /// <summary>
        /// Gets the object responsible for retrieving the <see cref="ObjectProperties{T}"/>
        /// instance for a given data object.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        /// Gets the object responsible for performing storage actions.
        /// </summary>
        IStoreProject ProjectStore { get; }

        /// <summary>
        /// Terminates the application.
        /// </summary>
        void ExitApplication();
    }
}