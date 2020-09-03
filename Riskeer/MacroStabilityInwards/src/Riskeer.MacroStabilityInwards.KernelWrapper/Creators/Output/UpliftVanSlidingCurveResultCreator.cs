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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output
{
    /// <summary>
    /// Creates <see cref="UpliftVanSlidingCurveResult"/> instances.
    /// </summary>
    internal static class UpliftVanSlidingCurveResultCreator
    {
        /// <summary>
        /// Creates a <see cref="UpliftVanSlidingCurveResult"/> based on the information
        /// given in the <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The output to create the result for.</param>
        /// <returns>A new <see cref="UpliftVanSlidingCurveResult"/> with information
        /// taken from the <paramref name="slidingCurve"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slidingCurve"/>
        /// is <c>null</c>.</exception>
        public static UpliftVanSlidingCurveResult Create(DualSlidingCircleMinimumSafetyCurve slidingCurve)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }

            bool leftCircleIsActive = slidingCurve.ActiveCircleCenter.X <= slidingCurve.PassiveCircleCenter.X;
            UpliftVanSlidingCircleResult leftCircle = leftCircleIsActive ? CreateActiveCircle(slidingCurve) : CreatePassiveCircle(slidingCurve);
            UpliftVanSlidingCircleResult rightCircle = leftCircleIsActive ? CreatePassiveCircle(slidingCurve) : CreateActiveCircle(slidingCurve);

            return new UpliftVanSlidingCurveResult(leftCircle, rightCircle, CreateSlices(slidingCurve.Slices),
                                                   slidingCurve.NonIteratedHorizontalForce, slidingCurve.IteratedHorizontalForce);
        }

        private static UpliftVanSlidingCircleResult CreateActiveCircle(DualSlidingCircleMinimumSafetyCurve slidingCurve)
        {
            return new UpliftVanSlidingCircleResult(new Point2D(slidingCurve.ActiveCircleCenter.X, slidingCurve.ActiveCircleCenter.Z),
                                                    slidingCurve.ActiveCircleRadius, true, slidingCurve.NonIteratedActiveForce, slidingCurve.IteratedActiveForce,
                                                    slidingCurve.DrivingActiveMoment, slidingCurve.ResistingActiveMoment);
        }

        private static UpliftVanSlidingCircleResult CreatePassiveCircle(DualSlidingCircleMinimumSafetyCurve slidingCurve)
        {
            return new UpliftVanSlidingCircleResult(new Point2D(slidingCurve.PassiveCircleCenter.X, slidingCurve.PassiveCircleCenter.Z),
                                                    slidingCurve.PassiveCircleRadius, false, slidingCurve.NonIteratedPassiveForce, slidingCurve.IteratedPassiveForce,
                                                    slidingCurve.DrivingPassiveMoment, slidingCurve.ResistingPassiveMoment);
        }

        private static IEnumerable<UpliftVanSliceResult> CreateSlices(IEnumerable<Slice> slidingCurveSlices)
        {
            return slidingCurveSlices.Select(
                slice =>
                    new UpliftVanSliceResult(
                        ToRiskeerPoint2D(slice.TopLeftPoint),
                        ToRiskeerPoint2D(slice.TopRightPoint),
                        ToRiskeerPoint2D(slice.BottomLeftPoint),
                        ToRiskeerPoint2D(slice.BottomRightPoint),
                        new UpliftVanSliceResult.ConstructionProperties
                        {
                            Cohesion = slice.Cohesion,
                            FrictionAngle = slice.FrictionAngleInput,
                            CriticalPressure = slice.YieldStress,
                            OverConsolidationRatio = slice.OCR,
                            Pop = slice.POP,
                            DegreeOfConsolidationPorePressureSoil = slice.DegreeOfConsolidationPorePressure,
                            DegreeOfConsolidationPorePressureLoad = slice.PorePressureDueToDegreeOfConsolidationLoad,
                            Dilatancy = slice.Dilatancy,
                            ExternalLoad = slice.ExternalLoad,
                            HydrostaticPorePressure = slice.HydrostaticPorePressure,
                            LeftForce = slice.LeftForce,
                            LeftForceAngle = slice.LeftForceAngle,
                            LeftForceY = slice.LeftForceY,
                            RightForce = slice.RightForce,
                            RightForceAngle = slice.RightForceAngle,
                            RightForceY = slice.RightForceY,
                            LoadStress = slice.LoadStress,
                            NormalStress = slice.NormalStress,
                            PorePressure = slice.PorePressure,
                            HorizontalPorePressure = slice.HorizontalPorePressure,
                            VerticalPorePressure = slice.VerticalPorePressure,
                            PiezometricPorePressure = slice.PiezometricPorePressure,
                            EffectiveStress = slice.EffectiveStress,
                            ExcessPorePressure = slice.ExcessPorePressure,
                            ShearStress = slice.ShearStress,
                            SoilStress = slice.SoilStress,
                            TotalPorePressure = slice.TotalPorePressure,
                            TotalStress = slice.TotalStress,
                            Weight = slice.Weight
                        })).ToArray();
        }

        private static Point2D ToRiskeerPoint2D(CSharpWrapperPoint2D point)
        {
            return new Point2D(point.X, point.Z);
        }
    }
}