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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.Service.Converters
{
    /// <summary>
    /// Converter to convert <see cref="UpliftVanSlidingCurveResult"/>
    /// into <see cref="MacroStabilityInwardsSlidingCurve"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSlidingCurveConverter
    {
        /// <summary>
        /// Converts <see cref="UpliftVanSlidingCurveResult"/>
        /// into <see cref="MacroStabilityInwardsSlidingCurve"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>The converted <see cref="MacroStabilityInwardsSlidingCurve"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsSlidingCurve Convert(UpliftVanSlidingCurveResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            MacroStabilityInwardsSlidingCircle leftCircle = ConvertCircle(result.LeftCircle);
            MacroStabilityInwardsSlidingCircle rightCircle = ConvertCircle(result.RightCircle);
            IEnumerable<MacroStabilityInwardsSlice> slices = ConvertSlices(result.Slices);

            return new MacroStabilityInwardsSlidingCurve(leftCircle, rightCircle, slices, result.NonIteratedHorizontalForce, result.IteratedHorizontalForce);
        }

        private static MacroStabilityInwardsSlidingCircle ConvertCircle(UpliftVanSlidingCircleResult circle)
        {
            return new MacroStabilityInwardsSlidingCircle(circle.Center,
                                                          circle.Radius,
                                                          circle.IsActive,
                                                          circle.NonIteratedForce,
                                                          circle.IteratedForce,
                                                          circle.DrivingMoment,
                                                          circle.ResistingMoment);
        }

        private static IEnumerable<MacroStabilityInwardsSlice> ConvertSlices(IEnumerable<UpliftVanSliceResult> slices)
        {
            return slices.Select(s => new MacroStabilityInwardsSlice(
                                     s.TopLeftPoint,
                                     s.TopRightPoint,
                                     s.BottomLeftPoint,
                                     s.BottomRightPoint,
                                     new MacroStabilityInwardsSlice.ConstructionProperties
                                     {
                                         Cohesion = s.Cohesion,
                                         FrictionAngle = s.FrictionAngle,
                                         CriticalPressure = s.CriticalPressure,
                                         OverConsolidationRatio = s.OverConsolidationRatio,
                                         Pop = s.Pop,
                                         DegreeOfConsolidationPorePressureSoil = s.DegreeOfConsolidationPorePressureSoil,
                                         DegreeOfConsolidationPorePressureLoad = s.DegreeOfConsolidationPorePressureLoad,
                                         Dilatancy = s.Dilatancy,
                                         ExternalLoad = s.ExternalLoad,
                                         HydrostaticPorePressure = s.HydrostaticPorePressure,
                                         LeftForce = s.LeftForce,
                                         LeftForceAngle = s.LeftForceAngle,
                                         LeftForceY = s.LeftForceY,
                                         RightForce = s.RightForce,
                                         RightForceAngle = s.RightForceAngle,
                                         RightForceY = s.RightForceY,
                                         LoadStress = s.LoadStress,
                                         NormalStress = s.NormalStress,
                                         PorePressure = s.PorePressure,
                                         HorizontalPorePressure = s.HorizontalPorePressure,
                                         VerticalPorePressure = s.VerticalPorePressure,
                                         PiezometricPorePressure = s.PiezometricPorePressure,
                                         EffectiveStress = s.EffectiveStress,
                                         EffectiveStressDaily = s.EffectiveStressDaily,
                                         ExcessPorePressure = s.ExcessPorePressure,
                                         ShearStress = s.ShearStress,
                                         SoilStress = s.SoilStress,
                                         TotalPorePressure = s.TotalPorePressure,
                                         TotalStress = s.TotalStress,
                                         Weight = s.Weight
                                     })).ToArray();
        }
    }
}