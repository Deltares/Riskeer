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

using System;
using System.Windows.Forms;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial
{
    /// <summary>
    /// An extension for the <see cref="Map"/> which shows the map coordinates of the mouse.
    /// </summary>
    public class MouseCoordinatesMapExtension : Extension
    {
        private readonly Map map;
        private readonly TextBox textBox;

        /// <summary>
        /// Creates a new instance of <see cref="MouseCoordinatesMapExtension"/>.
        /// </summary>
        /// <param name="map">The <see cref="Map"/> wich the extension applies to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is <c>null</c>.</exception>
        public MouseCoordinatesMapExtension(Map map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map", "An extension cannot be initialized without map.");
            }
            this.map = map;

            textBox = new TextBox
            {
                AutoSize = true,
                BorderStyle = BorderStyle.None,
                Width = 160
            };
        }

        /// <summary>
        /// Actives the extension by keeping track of the mouse and showing the coordinates on the <see cref="Map"/>.
        /// </summary>
        public override void Activate()
        {
            map.GeoMouseMove += OnMouseMove;
            map.Controls.Add(textBox);
            base.Activate();
        }

        /// <summary>
        /// Deactives the extension so it won't keep track of the mouse and doesn't show the coordinates on the <see cref="Map"/>.
        /// </summary>
        public override void Deactivate()
        {
            map.GeoMouseMove -= OnMouseMove;
            map.Controls.Remove(textBox);

            base.Deactivate();
        }

        private void OnMouseMove(object sender, GeoMouseArgs e)
        {
            textBox.Text = string.Format("X: {0:.#####} Y: {1:.#####}", e.GeographicLocation.X, e.GeographicLocation.Y);
        }
    }
}