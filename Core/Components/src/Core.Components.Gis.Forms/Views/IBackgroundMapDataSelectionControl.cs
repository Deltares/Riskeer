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
using System.Windows.Forms;
using Core.Components.Gis.Data;

namespace Core.Components.Gis.Forms.Views
{
    /// <summary>
    /// Interface for a user control that has a background MapData.
    /// </summary>
    public interface IBackgroundMapDataSelectionControl
    {
        /// <summary>
        /// Fired when the <see cref="SelectedMapData"/> has been changed. 
        /// </summary>
        event EventHandler<EventArgs> SelectedMapDataChanged;

        /// <summary>
        /// Gets the display name of the user control;
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the selected <see cref="ImageBasedMapData"/> or <c>null</c> if none selected.
        /// </summary>
        ImageBasedMapData SelectedMapData { get; }

        /// <summary>
        /// Gets the user control.
        /// </summary>
        UserControl UserControl { get; }
    }
}