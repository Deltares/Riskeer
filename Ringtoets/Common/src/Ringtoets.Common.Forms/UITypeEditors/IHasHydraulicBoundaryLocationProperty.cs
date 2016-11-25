// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.UITypeEditors
{
    /// <summary>
    /// Interface for <see cref="IObjectProperties"/> with a hydraulic boundary location property.
    /// </summary>
    public interface IHasHydraulicBoundaryLocationProperty : IObjectProperties
    {
        /// <summary>
        /// Gets the hydraulic boundary location that is selected.
        /// </summary>
        HydraulicBoundaryLocation SelectedHydraulicBoundaryLocation { get; }

        /// <summary>
        /// Returns the collection of selectable hydraulic boundary locations.
        /// </summary>
        /// <returns>A collection of selectable hydraulic boundary locations.</returns>
        IEnumerable<HydraulicBoundaryLocation> GetHydraulicBoundaryLocations();

        /// <summary>
        /// Returns the reference location from which the hydraulic boundary location needs to be calculated.
        /// </summary>
        Point2D GetReferenceLocation();
    }
}