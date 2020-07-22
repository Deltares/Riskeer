// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile
{
    /// <summary>
    /// Factory to create simple <see cref="MacroStabilityInwardsPreconsolidationStress"/>
    /// instances that can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsPreconsolidationStressTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <returns>The created <see cref="MacroStabilityInwardsPreconsolidationStress"/>.</returns>
        public static MacroStabilityInwardsPreconsolidationStress CreateMacroStabilityInwardsPreconsolidationStress()
        {
            return CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(13, 34));
        }

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="location">The location of the preconsolidation stress.</param>
        /// <returns>The created <see cref="MacroStabilityInwardsPreconsolidationStress"/>.</returns>
        public static MacroStabilityInwardsPreconsolidationStress CreateMacroStabilityInwardsPreconsolidationStress(Point2D location)
        {
            return new MacroStabilityInwardsPreconsolidationStress(location,
                                                                   new VariationCoefficientLogNormalDistribution
                                                                   {
                                                                       Mean = (RoundedDouble) 10.09,
                                                                       CoefficientOfVariation = (RoundedDouble) 20.05
                                                                   });
        }


    }
}