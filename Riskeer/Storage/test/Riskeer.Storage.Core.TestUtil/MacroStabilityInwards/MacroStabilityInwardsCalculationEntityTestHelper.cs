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
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil.MacroStabilityInwards
{
    /// <summary>
    /// Class to assert the properties of macro stability inwards calculation entities.
    /// </summary>
    public static class MacroStabilityInwardsCalculationEntityTestHelper
    {
        /// <summary>
        /// Determines for each property of <paramref name="scenario"/> 
        /// whether the matching <paramref name="entity"/> property have the expected value.
        /// </summary>
        /// <param name="scenario">The <see cref="MacroStabilityInwardsCalculationScenario"/> to compare.</param>
        /// <param name="entity">The <see cref="MacroStabilityInwardsCalculationEntity"/>
        /// to compare.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the arguments is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when any of the values of the 
        /// <see cref="MacroStabilityInwardsCalculationScenario"/> and its nested elements do not match.
        /// </exception>
        /// <remarks>This method does not assert the values of:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsInput.StochasticSoilModel"/> 
        /// and <see cref="MacroStabilityInwardsInput.StochasticSoilProfile"/>.</item>
        /// <item><see cref="MacroStabilityInwardsInput.HydraulicBoundaryLocation"/>.</item>
        /// <item><see cref="MacroStabilityInwardsInput.SurfaceLine"/>.</item>
        /// <item>The string related properties <see cref="MacroStabilityInwardsCalculationScenario.Name"/> and comments.</item>
        /// </list></remarks>
        public static void AssertCalculationScenarioPropertyValues(MacroStabilityInwardsCalculationScenario scenario, MacroStabilityInwardsCalculationEntity entity)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException(nameof(scenario));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Assert.AreEqual(Convert.ToByte(scenario.IsRelevant), entity.RelevantForScenario);
            AssertAreEqual(scenario.Contribution, entity.ScenarioContribution);

            AssertInputParameters(scenario.InputParameters, entity);
        }

        private static void AssertInputParameters(MacroStabilityInwardsInput input, MacroStabilityInwardsCalculationEntity entity)
        {
            AssertAreEqual(input.AssessmentLevel, entity.AssessmentLevel);
            Assert.AreEqual(Convert.ToByte(input.UseAssessmentLevelManualInput), entity.UseAssessmentLevelManualInput);
            AssertAreEqual(input.SlipPlaneMinimumDepth, entity.SlipPlaneMinimumDepth);
            AssertAreEqual(input.SlipPlaneMinimumLength, entity.SlipPlaneMinimumLength);
            AssertAreEqual(input.MaximumSliceWidth, entity.MaximumSliceWidth);
            Assert.AreEqual(Convert.ToByte(input.MoveGrid), entity.MoveGrid);
            Assert.AreEqual(Convert.ToByte(input.DikeSoilScenario), entity.DikeSoilScenario);
            AssertAreEqual(input.WaterLevelRiverAverage, entity.WaterLevelRiverAverage);
            Assert.AreEqual(Convert.ToByte(input.DrainageConstructionPresent), entity.DrainageConstructionPresent);
            AssertAreEqual(input.XCoordinateDrainageConstruction, entity.DrainageConstructionCoordinateX);
            AssertAreEqual(input.ZCoordinateDrainageConstruction, entity.DrainageConstructionCoordinateZ);
            AssertAreEqual(input.MinimumLevelPhreaticLineAtDikeTopRiver, entity.MinimumLevelPhreaticLineAtDikeTopRiver);
            AssertAreEqual(input.MinimumLevelPhreaticLineAtDikeTopPolder, entity.MinimumLevelPhreaticLineAtDikeTopPolder);

            AssertLocationInputs(input.LocationInputExtreme, input.LocationInputDaily, entity);

            Assert.AreEqual(Convert.ToByte(input.AdjustPhreaticLine3And4ForUplift), entity.AdjustPhreaticLine3And4ForUplift);
            AssertAreEqual(input.LeakageLengthOutwardsPhreaticLine3, entity.LeakageLengthOutwardsPhreaticLine3);
            AssertAreEqual(input.LeakageLengthInwardsPhreaticLine3, entity.LeakageLengthInwardsPhreaticLine3);
            AssertAreEqual(input.LeakageLengthOutwardsPhreaticLine4, entity.LeakageLengthOutwardsPhreaticLine4);
            AssertAreEqual(input.LeakageLengthInwardsPhreaticLine4, entity.LeakageLengthInwardsPhreaticLine4);
            AssertAreEqual(input.PiezometricHeadPhreaticLine2Outwards, entity.PiezometricHeadPhreaticLine2Outwards);
            AssertAreEqual(input.PiezometricHeadPhreaticLine2Inwards, entity.PiezometricHeadPhreaticLine2Inwards);

            Assert.AreEqual(Convert.ToByte(input.GridDeterminationType), entity.GridDeterminationType);
            Assert.AreEqual(Convert.ToByte(input.TangentLineDeterminationType), entity.TangentLineDeterminationType);

            AssertAreEqual(input.TangentLineZTop, entity.TangentLineZTop);
            AssertAreEqual(input.TangentLineZBottom, entity.TangentLineZBottom);
            Assert.AreEqual(input.TangentLineNumber, entity.TangentLineNumber);

            AssertGridInputs(input.LeftGrid, input.RightGrid, entity);

            Assert.AreEqual(Convert.ToByte(input.CreateZones), entity.CreateZones);
            Assert.AreEqual(Convert.ToByte(input.ZoningBoundariesDeterminationType), entity.ZoningBoundariesDeterminationType);
            AssertAreEqual(input.ZoneBoundaryLeft, entity.ZoneBoundaryLeft);
            AssertAreEqual(input.ZoneBoundaryRight, entity.ZoneBoundaryRight);
        }

        private static void AssertGridInputs(MacroStabilityInwardsGrid leftGrid,
                                             MacroStabilityInwardsGrid rightGrid,
                                             MacroStabilityInwardsCalculationEntity entity)
        {
            AssertAreEqual(leftGrid.XLeft, entity.LeftGridXLeft);
            AssertAreEqual(leftGrid.XRight, entity.LeftGridXRight);
            Assert.AreEqual(leftGrid.NumberOfHorizontalPoints, entity.LeftGridNrOfHorizontalPoints);
            AssertAreEqual(leftGrid.ZTop, entity.LeftGridZTop);
            AssertAreEqual(leftGrid.ZBottom, entity.LeftGridZBottom);
            Assert.AreEqual(leftGrid.NumberOfVerticalPoints, entity.LeftGridNrOfVerticalPoints);

            AssertAreEqual(rightGrid.XLeft, entity.RightGridXLeft);
            AssertAreEqual(rightGrid.XRight, entity.RightGridXRight);
            Assert.AreEqual(rightGrid.NumberOfHorizontalPoints, entity.RightGridNrOfHorizontalPoints);
            AssertAreEqual(rightGrid.ZTop, entity.RightGridZTop);
            AssertAreEqual(rightGrid.ZBottom, entity.RightGridZBottom);
            Assert.AreEqual(rightGrid.NumberOfVerticalPoints, entity.RightGridNrOfVerticalPoints);
        }

        private static void AssertLocationInputs(IMacroStabilityInwardsLocationInputExtreme locationInputExtreme,
                                                 IMacroStabilityInwardsLocationInputDaily locationInputDaily,
                                                 MacroStabilityInwardsCalculationEntity entity)
        {
            AssertAreEqual(locationInputExtreme.WaterLevelPolder, entity.LocationInputExtremeWaterLevelPolder);
            Assert.AreEqual(Convert.ToByte(locationInputExtreme.UseDefaultOffsets), entity.LocationInputExtremeUseDefaultOffsets);
            AssertAreEqual(locationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver, entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver);
            AssertAreEqual(locationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder, entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder);
            AssertAreEqual(locationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside, entity.LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside);
            AssertAreEqual(locationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder, entity.LocationInputExtremePhreaticLineOffsetDikeToeAtPolder);
            AssertAreEqual(locationInputExtreme.PenetrationLength, entity.LocationInputExtremePenetrationLength);

            AssertAreEqual(locationInputDaily.WaterLevelPolder, entity.LocationInputDailyWaterLevelPolder);
            Assert.AreEqual(Convert.ToByte(locationInputDaily.UseDefaultOffsets), entity.LocationInputDailyUseDefaultOffsets);
            AssertAreEqual(locationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver, entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver);
            AssertAreEqual(locationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder, entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder);
            AssertAreEqual(locationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside, entity.LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside);
            AssertAreEqual(locationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder, entity.LocationInputDailyPhreaticLineOffsetDikeToeAtPolder);
        }

        private static void AssertAreEqual(RoundedDouble expectedDouble, double? actualDouble)
        {
            if (double.IsNaN(expectedDouble))
            {
                Assert.IsNull(actualDouble);
            }
            else
            {
                Assert.AreEqual(expectedDouble, actualDouble, expectedDouble.GetAccuracy());
            }
        }
    }
}