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
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.MapFunctions
{
    /// <summary>
    /// A <see cref="MapFunction"/> that can pan the map and handles events to update <see cref="Cursor"/>.
    /// </summary>
    public class MapFunctionPan : global::DotSpatial.Controls.MapFunctionPan
    {
        private readonly Cursor defaultCursor = Cursors.Default;

        /// <summary>
        /// Initializes a new instance of the MapFunctionPan class.
        /// </summary>
        /// <param name="map">Any valid <see cref="IMap"/> interface.</param>
        public MapFunctionPan(IMap map) : base(map)
        {
            FunctionActivated += ActivateFunction;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
        }

        private void ActivateFunction(object sender, EventArgs e)
        {
            Map.Cursor = defaultCursor;
        }

        private void OnMouseUp(object sender, GeoMouseArgs e)
        {
            Map.Cursor = defaultCursor;
        }

        private void OnMouseDown(object sender, GeoMouseArgs geoMouseArgs)
        {
            Map.Cursor = Cursors.Hand;
        }
    }
}