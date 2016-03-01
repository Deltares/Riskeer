﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Components.Gis.Data;

namespace Core.Components.Gis
{
    /// <summary>
    /// Interface describing general map interactions.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Gets a value indicating whether or not the map can be panned with the left mouse button.
        /// </summary>
        bool IsPanningEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the map can be zoomed by rectangle with the left mouse button.
        /// </summary>
        bool IsRectangleZoomingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the map coordinates of the mouse should be shown.
        /// </summary>
        bool IsMouseCoordinatesEnabled { get; }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="IMap"/>.
        /// </summary>
        MapData Data { get; set; }

        /// <summary>
        /// Zooms to a level so that everything is in view.
        /// </summary>
        void ZoomToAll();

        /// <summary>
        /// Toggles panning of the <see cref="IMap"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        void TogglePanning();

        /// <summary>
        /// Toggles rectangle zooming of the <see cref="IMap"/>. Rectangle zooming is invoked by clicking the left mouse-button.
        /// </summary>
        void ToggleRectangleZooming();

        /// <summary>
        /// Toggles mouse coordinates of the <see cref="IMap"/>.
        /// </summary>
        void ToggleMouseCoordinates();
    }
}