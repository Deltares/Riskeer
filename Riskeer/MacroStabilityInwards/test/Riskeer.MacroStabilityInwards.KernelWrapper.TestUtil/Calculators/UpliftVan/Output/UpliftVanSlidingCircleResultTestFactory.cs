// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output
{
    /// <summary>
    /// Factory to create simple <see cref="UpliftVanSlidingCircleResult"/> instances that can be used for testing.
    /// </summary>
    public static class UpliftVanSlidingCircleResultTestFactory
    {
        /// <summary>
        /// Creates a new <see cref="UpliftVanSlidingCircleResult"/>.
        /// </summary>
        /// <returns>The created <see cref="UpliftVanSlidingCircleResult"/>.</returns>
        public static UpliftVanSlidingCircleResult Create()
        {
            return new UpliftVanSlidingCircleResult(new Point2D(0, 0), 0.1, true, 0.2, 0.3, 0.4, 0.5);
        }
    }
}