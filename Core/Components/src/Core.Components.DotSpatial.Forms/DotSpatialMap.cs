// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using DotSpatial.Controls;
using DotSpatial.Data;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// The DotSpatial Map Control for 2D applications.
    /// </summary>
    /// <remarks>This class was introduced to prevent a <see cref="StackOverflowException"/> when zooming in on 
    /// an extent smaller than 1e-3 and should be removed when DotSpatial solved the issue.</remarks>
    public class DotSpatialMap : Map
    {
        private const double minExt = 1e-3;

        /// <summary>
        /// Fires the ViewExtentsChanged event. Corrects the ViewExtent if it is smaller than 1e-3. If ZoomOutFartherThanMaxExtent is set, it corrects the 
        /// ViewExtent if it is bigger then 1e+9. Otherwise it corrects the ViewExtent if it is bigger than the Maps extent + 10%.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">An object that contains extent data.</param>
        /// <remarks>Corrects the <see cref="Map.OnViewExtentsChanged"/> with a minimum extent of 1e-3.</remarks>
        protected override void OnViewExtentsChanged(object sender, ExtentArgs args)
        {
            if (ViewExtents.Width < minExt || ViewExtents.Height < minExt) // the current height or width is smaller than minExt
            {
                double x = ViewExtents.Center.X;
                double y = ViewExtents.Center.Y;
                ViewExtents.SetCenter(x, y, minExt, minExt);
                return;
            }

            base.OnViewExtentsChanged(sender, args);
        }
    }
}