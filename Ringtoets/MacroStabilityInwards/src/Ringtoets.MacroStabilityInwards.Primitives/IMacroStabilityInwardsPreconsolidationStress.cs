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

using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Interface that represents a preconsolidation stress definition that was imported
    /// from D-Soil model.
    /// </summary>
    public interface IMacroStabilityInwardsPreconsolidationStress
    {
        /// <summary>
        /// Gets the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        VariationCoefficientLogNormalDistribution Stress { get; }

        /// <summary>
        /// Gets the location of the preconsolidation stress
        /// [m]
        /// </summary>
        /// <remarks>The <see cref="Point2D.Y"/> has the unit [m+NAP]</remarks>
        Point2D Location { get; }
    }
}