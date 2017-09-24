// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an upliftVan sub calculation.
    /// </summary>
    public interface IUpliftVanCalculator
    {
        /// <summary>
        /// Sets the soil model.
        /// </summary>
        SoilModel SoilModel { set; }

        /// <summary>
        /// Sets the soil profile.
        /// </summary>
        SoilProfile2D SoilProfile { set; }

        /// <summary>
        /// Sets the location.
        /// </summary>
        StabilityLocation Location { set; }

        /// <summary>
        /// Sets the surface line.
        /// </summary>
        SurfaceLine2 SurfaceLine { set; }

        /// <summary>
        /// Sets the move grid property.
        /// </summary>
        bool MoveGrid { set; }

        /// <summary>
        /// Sets the maximum slice width.
        /// </summary>
        double MaximumSliceWidth { set; }

        /// <summary>
        /// Sets the slip plane upliftVan.
        /// </summary>
        SlipPlaneUpliftVan SlipPlaneUpliftVan { set; }

        /// <summary>
        /// Sets whether the grid is automatically determined or not.
        /// </summary>
        bool GridAutomaticDetermined { set; }

        /// <summary>
        /// Sets whether zones should be created.
        /// </summary>
        bool CreateZones { set; }

        /// <summary>
        /// Sets the automatic forbidden zones property.
        /// </summary>
        bool AutomaticForbiddenZones { set; }

        /// <summary>
        /// Sets the minimum depth of the slip plane.
        /// [m]
        /// </summary>
        double SlipPlaneMinimumDepth { set; }

        /// <summary>
        /// Sets the minimum length of the slip plane.
        /// [m]
        /// </summary>
        double SlipPlaneMinimumLength { set; }

        /// <summary>
        /// Gets the factor of stability.
        /// </summary>
        double FactoryOfStability { get; }

        /// <summary>
        /// Gets the z value.
        /// </summary>
        double ZValue { get; }

        /// <summary>
        /// Gets the forbidden zones x entry min.
        /// </summary>
        double ForbiddenZonesXEntryMin { get; }

        /// <summary>
        /// Gets the forbidden zones x entry max.
        /// </summary>
        double ForbiddenZonesXEntryMax { get; }

        /// <summary>
        /// Gets whether the forbidden zones are automatically calculated.
        /// </summary>
        bool ForbiddenZonesAutomaticallyCalculated { get; }

        /// <summary>
        /// Gets whether the grid is automatically calculated.
        /// </summary>
        bool GridAutomaticallyCalculated { get; }

        /// <summary>
        /// Gets the sliding curve result.
        /// </summary>
        SlidingDualCircle SlidingCurveResult { get; }

        /// <summary>
        /// Gets the slip plane result.
        /// </summary>
        SlipPlaneUpliftVan SlipPlaneResult { get; }

        /// <summary>
        /// Performs the upliftVan calculation.
        /// </summary>
        void Calculate();
    }
}