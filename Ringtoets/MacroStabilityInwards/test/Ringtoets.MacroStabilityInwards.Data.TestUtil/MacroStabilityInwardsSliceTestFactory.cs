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

using System;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates instances of <see cref="MacroStabilityInwardsSlice"/>
    /// that can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsSliceTestFactory
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSlice"/> with initialized values 
        /// for all properties.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsSlice"/>.</returns>
        public static MacroStabilityInwardsSlice CreateSlice()
        {
            var random = new Random(21);
            return new MacroStabilityInwardsSlice(new Point2D(random.NextDouble(), random.NextDouble()),
                                                  new Point2D(random.NextDouble(), random.NextDouble()),
                                                  new Point2D(random.NextDouble(), random.NextDouble()),
                                                  new Point2D(random.NextDouble(), random.NextDouble()),
                                                  new MacroStabilityInwardsSlice.ConstructionProperties
                                                  {
                                                      Cohesion = random.NextDouble(),
                                                      FrictionAngle = random.NextDouble(),
                                                      CriticalPressure = random.NextDouble(),
                                                      OverConsolidationRatio = random.NextDouble(),
                                                      DegreeOfConsolidationPorePressureSoil = random.NextDouble(),
                                                      DegreeOfConsolidationPorePressureLoad = random.NextDouble(),
                                                      Pop = random.NextDouble(),
                                                      Dilatancy = random.NextDouble(),
                                                      ExternalLoad = random.NextDouble(),
                                                      HydrostaticPorePressure = random.NextDouble(),
                                                      LeftForce = random.NextDouble(),
                                                      LeftForceAngle = random.NextDouble(),
                                                      LeftForceY = random.NextDouble(),
                                                      RightForce = random.NextDouble(),
                                                      RightForceAngle = random.NextDouble(),
                                                      RightForceY = random.NextDouble(),
                                                      LoadStress = random.NextDouble(),
                                                      NormalStress = random.NextDouble(),
                                                      PorePressure = random.NextDouble(),
                                                      HorizontalPorePressure = random.NextDouble(),
                                                      VerticalPorePressure = random.NextDouble(),
                                                      PiezometricPorePressure = random.NextDouble(),
                                                      EffectiveStress = random.NextDouble(),
                                                      EffectiveStressDaily = random.NextDouble(),
                                                      ExcessPorePressure = random.NextDouble(),
                                                      ShearStress = random.NextDouble(),
                                                      SoilStress = random.NextDouble(),
                                                      TotalPorePressure = random.NextDouble(),
                                                      TotalStress = random.NextDouble(),
                                                      Weight = random.NextDouble()
                                                  });
        }
    }
}