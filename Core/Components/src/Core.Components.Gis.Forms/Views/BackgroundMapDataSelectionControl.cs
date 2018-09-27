// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;
using Core.Components.Gis.Data;

namespace Core.Components.Gis.Forms.Views
{
    /// <summary>
    /// Abstract base class for a user control used for selecting background map data.
    /// </summary>
    public abstract class BackgroundMapDataSelectionControl : UserControl
    {
        /// <summary>
        /// Fired when the <see cref="SelectedMapData"/> has been changed.
        /// </summary>
        public abstract event EventHandler<EventArgs> SelectedMapDataChanged;

        /// <summary>
        /// Creates a new instance of <see cref="BackgroundMapDataSelectionControl"/>.
        /// </summary>
        /// <param name="displayName">The display name of the user control.</param>
        protected BackgroundMapDataSelectionControl(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets the display name of the user control.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the selected <see cref="ImageBasedMapData"/> or <c>null</c> if none selected.
        /// </summary>
        public abstract ImageBasedMapData SelectedMapData { get; }
    }
}