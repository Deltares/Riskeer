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
        /// Performs the upliftVan calculation.
        /// </summary>
        void Calculate();

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
        /// Sets the slip plane uplift van.
        /// </summary>
        SlipPlaneUpliftVan SlipPlaneUpliftVan { set; }

        /// <summary>
        /// sets whether the grid is automatic determined or not.
        /// </summary>
        bool GridAutomaticDetermined { set; }

        /// <summary>
        /// Sets the create zones property.
        /// </summary>
        bool CreateZones { set; }

        /// <summary>
        /// Sets the automatic forbidden zones property.
        /// </summary>
        bool AutomaticForbidenZones { set; }

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
    }
}