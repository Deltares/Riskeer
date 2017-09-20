﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="MacroStabilityInwardsSlidingCurveResult"/> instances.
    /// </summary>
    public static class MacroStabilityInwardsSlidingCurveResultCreator
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSlidingCurveResult"/> based on the information
        /// given in the <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The output to create the result for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSlidingCurveResult"/> with information
        /// taken from the <paramref name="slidingCurve"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slidingCurve"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsSlidingCurveResult Create(SlidingDualCircle slidingCurve)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }

            MacroStabilityInwardsSlidingCircleResult leftCircle = slidingCurve.LeftCircleIsActive ? CreateActiveCircle(slidingCurve) : CreatePassiveCircle(slidingCurve);
            MacroStabilityInwardsSlidingCircleResult rightCircle = slidingCurve.LeftCircleIsActive ? CreatePassiveCircle(slidingCurve) : CreateActiveCircle(slidingCurve);

            return new MacroStabilityInwardsSlidingCurveResult(leftCircle, rightCircle, CreateSlices(slidingCurve.Slices),
                                                               slidingCurve.HorizontalForce0, slidingCurve.HorizontalForce);
        }

        private static MacroStabilityInwardsSlidingCircleResult CreateActiveCircle(SlidingDualCircle slidingCurve)
        {
            return new MacroStabilityInwardsSlidingCircleResult(new Point2D(slidingCurve.ActiveCircle.X, slidingCurve.ActiveCircle.Z),
                                                                slidingCurve.ActiveRadius, true, slidingCurve.ActiveForce0, slidingCurve.ActiveForce,
                                                                slidingCurve.DrivingMomentActive, slidingCurve.ResistingMomentActive);
        }

        private static MacroStabilityInwardsSlidingCircleResult CreatePassiveCircle(SlidingDualCircle slidingCurve)
        {
            return new MacroStabilityInwardsSlidingCircleResult(new Point2D(slidingCurve.PassiveCircle.X, slidingCurve.PassiveCircle.Z),
                                                                slidingCurve.PassiveRadius, false, slidingCurve.PassiveForce0, slidingCurve.PassiveForce,
                                                                slidingCurve.DrivingMomentPassive, slidingCurve.ResistingMomentPassive);
        }

        private static IEnumerable<MacroStabilityInwardsSliceResult> CreateSlices(IEnumerable<Slice> slidingCurveSlices)
        {
            return slidingCurveSlices.Select(
                slice =>
                    new MacroStabilityInwardsSliceResult(
                        new Point2D(slice.TopLeftX, slice.TopLeftZ),
                        new Point2D(slice.TopRightX, slice.TopRightZ),
                        new Point2D(slice.BottomLeftX, slice.BottomLeftZ),
                        new Point2D(slice.BottomRightX, slice.BottomRightZ),
                        new MacroStabilityInwardsSliceResult.ConstructionProperties
                        {
                            Cohesion = slice.Cohesion,
                            FrictionAngle = slice.Phi,
                            CriticalPressure = slice.PGrens,
                            OverConsolidationRatio = slice.OCR,
                            Pop = slice.POP,
                            DegreeOfConsolidationPorePressureSoil = slice.DegreeofConsolidationPorePressure,
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
                            PorePressure = slice.PoreOnSurface,
                            HorizontalPorePressure = slice.HPoreOnSurface,
                            VerticalPorePressure = slice.VPoreOnSurface,
                            PiezometricPorePressure = slice.PiezometricPorePressure,
                            EffectiveStress = slice.EffectiveStress,
                            EffectiveStressDaily = slice.EffectiveStressDaily,
                            ExcessPorePressure = slice.ExcessPorePressure,
                            ShearStress = slice.ShearStress,
                            SoilStress = slice.SoilStress,
                            TotalPorePressure = slice.TotalPorePressure,
                            TotalStress = slice.TotalStress,
                            Weight = slice.Weight
                        })).ToArray();
        }
    }
}