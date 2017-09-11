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
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real upliftVan sub calculator of macrostability inwards.
    /// </summary>
    public class UpliftVanCalculatorStub : IUpliftVanCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        public SoilModel SoilModel { get; set; }

        public SoilProfile2D SoilProfile { get; set; }

        public StabilityLocation Location { get; set; }

        public bool MoveGrid { get; set; }

        public double MaximumSliceWidth { get; set; }

        public SurfaceLine2 SurfaceLine { get; set; }

        public SlipPlaneUpliftVan SlipPlaneUpliftVan { get; set; }

        public bool GridAutomaticDetermined { get; set; }

        public void Calculate()
        {
            Calculated = true;
        }
    }
}