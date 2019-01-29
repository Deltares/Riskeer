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

using Core.Common.Base.TestUtil.Geometry;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class MacroStabilityInwardsCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsCalculationScenario original,
                                     MacroStabilityInwardsCalculationScenario clone)
        {
            Assert.AreEqual(original.Contribution, clone.Contribution);
            Assert.AreEqual(original.IsRelevant, clone.IsRelevant);
            AreClones((MacroStabilityInwardsCalculation) original, clone);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsCalculation original,
                                     MacroStabilityInwardsCalculation clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, CommonCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, AreClones);
            CoreCloneAssert.AreObjectClones(original.Output, clone.Output, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsInput original,
                                     MacroStabilityInwardsInput clone)
        {
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
            Assert.AreSame(original.StochasticSoilModel, clone.StochasticSoilModel);
            Assert.AreSame(original.StochasticSoilProfile, clone.StochasticSoilProfile);
            Assert.AreSame(original.SurfaceLine, clone.SurfaceLine);

            Assert.AreEqual(original.AssessmentLevel, clone.AssessmentLevel);
            Assert.AreEqual(original.UseAssessmentLevelManualInput, clone.UseAssessmentLevelManualInput);

            Assert.AreEqual(original.SlipPlaneMinimumDepth, clone.SlipPlaneMinimumDepth);
            Assert.AreEqual(original.SlipPlaneMinimumLength, clone.SlipPlaneMinimumLength);
            Assert.AreEqual(original.MaximumSliceWidth, clone.MaximumSliceWidth);

            Assert.AreEqual(original.MoveGrid, clone.MoveGrid);
            Assert.AreEqual(original.DikeSoilScenario, clone.DikeSoilScenario);

            Assert.AreEqual(original.WaterLevelRiverAverage, clone.WaterLevelRiverAverage);

            Assert.AreEqual(original.DrainageConstructionPresent, clone.DrainageConstructionPresent);
            Assert.AreEqual(original.XCoordinateDrainageConstruction, clone.XCoordinateDrainageConstruction);
            Assert.AreEqual(original.ZCoordinateDrainageConstruction, clone.ZCoordinateDrainageConstruction);

            Assert.AreEqual(original.MinimumLevelPhreaticLineAtDikeTopRiver, clone.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(original.MinimumLevelPhreaticLineAtDikeTopPolder, clone.MinimumLevelPhreaticLineAtDikeTopPolder);

            CoreCloneAssert.AreObjectClones((MacroStabilityInwardsLocationInputExtreme) original.LocationInputExtreme,
                                            clone.LocationInputExtreme,
                                            AreClones);
            CoreCloneAssert.AreObjectClones((MacroStabilityInwardsLocationInputDaily) original.LocationInputDaily,
                                            clone.LocationInputDaily,
                                            AreClones);

            Assert.AreEqual(original.AdjustPhreaticLine3And4ForUplift, clone.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(original.LeakageLengthOutwardsPhreaticLine3, clone.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(original.LeakageLengthInwardsPhreaticLine3, clone.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(original.LeakageLengthOutwardsPhreaticLine4, clone.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(original.LeakageLengthInwardsPhreaticLine4, clone.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(original.PiezometricHeadPhreaticLine2Outwards, clone.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(original.PiezometricHeadPhreaticLine2Inwards, clone.PiezometricHeadPhreaticLine2Inwards);

            Assert.AreEqual(original.GridDeterminationType, clone.GridDeterminationType);
            Assert.AreEqual(original.TangentLineDeterminationType, clone.TangentLineDeterminationType);

            Assert.AreEqual(original.TangentLineZTop, clone.TangentLineZTop);
            Assert.AreEqual(original.TangentLineZBottom, clone.TangentLineZBottom);
            Assert.AreEqual(original.TangentLineNumber, clone.TangentLineNumber);

            CoreCloneAssert.AreObjectClones(original.LeftGrid, clone.LeftGrid, AreClones);
            CoreCloneAssert.AreObjectClones(original.RightGrid, clone.RightGrid, AreClones);

            Assert.AreEqual(original.CreateZones, clone.CreateZones);
            Assert.AreEqual(original.ZoningBoundariesDeterminationType, clone.ZoningBoundariesDeterminationType);
            Assert.AreEqual(original.ZoneBoundaryLeft, clone.ZoneBoundaryLeft);
            Assert.AreEqual(original.ZoneBoundaryRight, clone.ZoneBoundaryRight);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsLocationInputBase original,
                                     MacroStabilityInwardsLocationInputBase clone)
        {
            Assert.AreEqual(original.WaterLevelPolder, clone.WaterLevelPolder);
            Assert.AreEqual(original.UseDefaultOffsets, clone.UseDefaultOffsets);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeTopAtPolder, clone.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeTopAtRiver, clone.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(original.PhreaticLineOffsetBelowShoulderBaseInside, clone.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeToeAtPolder, clone.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsLocationInputExtreme original,
                                     MacroStabilityInwardsLocationInputExtreme clone)
        {
            AreClones((MacroStabilityInwardsLocationInputBase) original, clone);
            Assert.AreEqual(original.PenetrationLength, clone.PenetrationLength);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsLocationInputDaily original,
                                     MacroStabilityInwardsLocationInputDaily clone)
        {
            AreClones((MacroStabilityInwardsLocationInputBase) original, clone);
            Assert.AreEqual(original.PenetrationLength, clone.PenetrationLength);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsGrid original,
                                     MacroStabilityInwardsGrid clone)
        {
            Assert.AreEqual(original.XLeft, clone.XLeft);
            Assert.AreEqual(original.XRight, clone.XRight);
            Assert.AreEqual(original.NumberOfHorizontalPoints, clone.NumberOfHorizontalPoints);
            Assert.AreEqual(original.ZTop, clone.ZTop);
            Assert.AreEqual(original.ZBottom, clone.ZBottom);
            Assert.AreEqual(original.NumberOfVerticalPoints, clone.NumberOfVerticalPoints);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsOutput original,
                                     MacroStabilityInwardsOutput clone)
        {
            CoreCloneAssert.AreObjectClones(original.SlidingCurve, clone.SlidingCurve, AreClones);
            CoreCloneAssert.AreObjectClones(original.SlipPlane, clone.SlipPlane, AreClones);

            Assert.AreEqual(original.FactorOfStability, clone.FactorOfStability);
            Assert.AreEqual(original.ZValue, clone.ZValue);
            Assert.AreEqual(original.ForbiddenZonesXEntryMin, clone.ForbiddenZonesXEntryMin);
            Assert.AreEqual(original.ForbiddenZonesXEntryMax, clone.ForbiddenZonesXEntryMax);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsSlipPlaneUpliftVan original,
                                     MacroStabilityInwardsSlipPlaneUpliftVan clone)
        {
            CoreCloneAssert.AreObjectClones(original.LeftGrid, clone.LeftGrid, AreClones);
            CoreCloneAssert.AreObjectClones(original.RightGrid, clone.RightGrid, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.TangentLines, clone.TangentLines,
                                                 (originalTangentLine, clonedTangentLine) =>
                                                     Assert.AreEqual(originalTangentLine, clonedTangentLine));
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsSlidingCurve original,
                                     MacroStabilityInwardsSlidingCurve clone)
        {
            CoreCloneAssert.AreObjectClones(original.LeftCircle, clone.LeftCircle, AreClones);
            CoreCloneAssert.AreObjectClones(original.RightCircle, clone.RightCircle, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.Slices, clone.Slices, AreClones);

            Assert.AreEqual(original.NonIteratedHorizontalForce, clone.NonIteratedHorizontalForce);
            Assert.AreEqual(original.IteratedHorizontalForce, clone.IteratedHorizontalForce);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsSlice original,
                                     MacroStabilityInwardsSlice clone)
        {
            CoreCloneAssert.AreObjectClones(original.TopLeftPoint, clone.TopLeftPoint, GeometryCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.TopRightPoint, clone.TopRightPoint, GeometryCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.BottomLeftPoint, clone.BottomLeftPoint, GeometryCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.BottomRightPoint, clone.BottomRightPoint, GeometryCloneAssert.AreClones);

            Assert.AreEqual(original.XCenter, clone.XCenter);
            Assert.AreEqual(original.ZCenterBottom, clone.ZCenterBottom);
            Assert.AreEqual(original.Width, clone.Width);
            Assert.AreEqual(original.ArcLength, clone.ArcLength);
            Assert.AreEqual(original.BottomAngle, clone.BottomAngle);
            Assert.AreEqual(original.TopAngle, clone.TopAngle);
            Assert.AreEqual(original.Cohesion, clone.Cohesion);
            Assert.AreEqual(original.FrictionAngle, clone.FrictionAngle);
            Assert.AreEqual(original.CriticalPressure, clone.CriticalPressure);
            Assert.AreEqual(original.OverConsolidationRatio, clone.OverConsolidationRatio);
            Assert.AreEqual(original.Pop, clone.Pop);
            Assert.AreEqual(original.DegreeOfConsolidationPorePressureSoil, clone.DegreeOfConsolidationPorePressureSoil);
            Assert.AreEqual(original.DegreeOfConsolidationPorePressureLoad, clone.DegreeOfConsolidationPorePressureLoad);
            Assert.AreEqual(original.Dilatancy, clone.Dilatancy);
            Assert.AreEqual(original.ExternalLoad, clone.ExternalLoad);
            Assert.AreEqual(original.HydrostaticPorePressure, clone.HydrostaticPorePressure);
            Assert.AreEqual(original.LeftForce, clone.LeftForce);
            Assert.AreEqual(original.LeftForceAngle, clone.LeftForceAngle);
            Assert.AreEqual(original.LeftForceY, clone.LeftForceY);
            Assert.AreEqual(original.RightForce, clone.RightForce);
            Assert.AreEqual(original.RightForceAngle, clone.RightForceAngle);
            Assert.AreEqual(original.RightForceY, clone.RightForceY);
            Assert.AreEqual(original.LoadStress, clone.LoadStress);
            Assert.AreEqual(original.NormalStress, clone.NormalStress);
            Assert.AreEqual(original.PorePressure, clone.PorePressure);
            Assert.AreEqual(original.HorizontalPorePressure, clone.HorizontalPorePressure);
            Assert.AreEqual(original.VerticalPorePressure, clone.VerticalPorePressure);
            Assert.AreEqual(original.PiezometricPorePressure, clone.PiezometricPorePressure);
            Assert.AreEqual(original.EffectiveStress, clone.EffectiveStress);
            Assert.AreEqual(original.EffectiveStressDaily, clone.EffectiveStressDaily);
            Assert.AreEqual(original.ExcessPorePressure, clone.ExcessPorePressure);
            Assert.AreEqual(original.ShearStress, clone.ShearStress);
            Assert.AreEqual(original.SoilStress, clone.SoilStress);
            Assert.AreEqual(original.TotalPorePressure, clone.TotalPorePressure);
            Assert.AreEqual(original.TotalStress, clone.TotalStress);
            Assert.AreEqual(original.Weight, clone.Weight);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(MacroStabilityInwardsSlidingCircle original,
                                     MacroStabilityInwardsSlidingCircle clone)
        {
            CoreCloneAssert.AreObjectClones(original.Center, clone.Center, GeometryCloneAssert.AreClones);
            Assert.AreEqual(original.Radius, clone.Radius);
            Assert.AreEqual(original.IsActive, clone.IsActive);
            Assert.AreEqual(original.NonIteratedForce, clone.NonIteratedForce);
            Assert.AreEqual(original.IteratedForce, clone.IteratedForce);
            Assert.AreEqual(original.DrivingMoment, clone.DrivingMoment);
            Assert.AreEqual(original.ResistingMoment, clone.ResistingMoment);
        }
    }
}