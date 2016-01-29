﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// All rights preserved.

using Core.Common.Controls.Views;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for controller that controls Tool Views in the application.
    /// </summary>
    public interface IToolViewController
    {
        /// <summary>
        /// Gets view manager used to handle tool windows.
        /// </summary>
        IViewList ToolWindowViews { get; }

        /// <summary>
        /// Open the tool view and make it visible in the interface.
        /// </summary>
        /// <param name="toolView">The tool view to open.</param>
        void OpenToolView(IView toolView);

        /// <summary>
        /// Close the tool view removing it from the interface.
        /// </summary>
        /// <param name="toolView">The tool view to close.</param>
        void CloseToolView(IView toolView);
    }
}