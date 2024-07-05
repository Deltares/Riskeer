// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.Primitives.TestUtil
{
    /// <summary>
    /// Factory for creating macro stability inwards related test data instances.
    /// </summary>
    public static class MacroStabilityInwardsTestDataFactory
    {
        /// <summary>
        /// Creates a test instance of <see cref="MacroStabilityInwardsWaternetLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MacroStabilityInwardsWaternetLine"/>.</returns>
        public static MacroStabilityInwardsWaternetLine CreateMacroStabilityInwardsWaternetLine()
        {
            return new MacroStabilityInwardsWaternetLine("Test Waternet Line", Enumerable.Empty<Point2D>(), CreateMacroStabilityInwardsPhreaticLine());
        }

        /// <summary>
        /// Creates a test instance of <see cref="MacroStabilityInwardsPhreaticLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MacroStabilityInwardsPhreaticLine"/>.</returns>
        public static MacroStabilityInwardsPhreaticLine CreateMacroStabilityInwardsPhreaticLine()
        {
            return new MacroStabilityInwardsPhreaticLine("Test Phreatic Line", Enumerable.Empty<Point2D>());
        }
    }
}