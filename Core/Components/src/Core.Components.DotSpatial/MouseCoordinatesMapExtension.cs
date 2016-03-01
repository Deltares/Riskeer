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
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial
{
    public class MouseCoordinatesMapExtension : Extension
    {
        private readonly Map map;
        private readonly TextBox textBox;

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

        public override void Activate()
        {
            map.GeoMouseMove += OnMouseMove;
            textBox.Location = new Point(0, map.Height - 16);
            map.Controls.Add(textBox);
            base.Activate();
        }

        public override void Deactivate()
        {
            map.GeoMouseMove -= OnMouseMove;
            map.Controls.Remove(textBox);

            base.Deactivate();
        }

        private void OnMouseMove(object sender, GeoMouseArgs e)
        {
            var location = string.Format("X: {0:.#####} Y: {1:.#####}", e.GeographicLocation.X, e.GeographicLocation.Y);

            textBox.Text = location;
        }
    }
}