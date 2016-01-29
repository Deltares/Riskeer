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

using System;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for keeping and notifying changes to the application selection.
    /// </summary>
    public interface IApplicationSelection
    {
        /// <summary>
        /// Gets or sets current selected object(s). Visibility of the menus, toolbars and 
        /// other controls should be updated when selected object is changed. Default 
        /// implementation will also show it in the PropertyGrid.
        /// </summary>
        object Selection { get; set; }

        /// <summary>
        /// Fired when user changes selection by clicking on it or by setting it using <see cref="Selection"/> property.
        /// </summary>
        event EventHandler<SelectedItemChangedEventArgs> SelectionChanged; 
    }
}