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
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.TestUtil
{
    /// <summary>
    /// Class for asserting calculator input.
    /// </summary>
    public static class CalculatorInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsWaternetInput"/>.</param>
        /// <param name="actual">The actual <see cref="WaternetCalculatorInput"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertDailyInput(IMacroStabilityInwardsWaternetInput original, WaternetCalculatorInput actual)
        {
            AssertPhreaticLineOffsets(original.LocationInputDaily, actual.PhreaticLineOffsets);
            Assert.AreEqual(original.LocationInputDaily.WaterLevelPolder, actual.WaterLevelPolder);
            Assert.AreEqual(original.LocationInputDaily.PenetrationLength, actual.PenetrationLength);
            Assert.AreEqual(original.WaterLevelRiverAverage, actual.AssessmentLevel);

            AssertGenericInput(original, actual);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsWaternetInput"/>.</param>
        /// <param name="actual">The actual <see cref="WaternetCalculatorInput"/>.</param>
        /// <param name="assessmentLevel">The assessment level to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/> or <paramref name="assessmentLevel"/>.</exception>
        public static void AssertExtremeInput(IMacroStabilityInwardsWaternetInput original, WaternetCalculatorInput actual, double assessmentLevel)
        {
            AssertPhreaticLineOffsets(original.LocationInputExtreme, actual.PhreaticLineOffsets);
            Assert.AreEqual(original.LocationInputExtreme.WaterLevelPolder, actual.WaterLevelPolder);
            Assert.AreEqual(original.LocationInputExtreme.PenetrationLength, actual.PenetrationLength);
            Assert.AreEqual(assessmentLevel, actual.AssessmentLevel);

            AssertGenericInput(original, actual);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/>.</param>
        /// <param name="actual">The actual <see cref="SoilProfile"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="original"/>
        /// contains an invalid value of the enum <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="original"/>
        /// contains an unsupported value of <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        public static void AssertSoilProfile(IMacroStabilityInwardsSoilProfileUnderSurfaceLine original, SoilProfile actual)
        {
            MacroStabilityInwardsSoilLayer2D[] expectedLayers = original.Layers.ToArray();
            SoilLayer[] actualLayers = actual.Layers.ToArray();

            IMacroStabilityInwardsPreconsolidationStress[] expectedPreconsolidationStresses = original.PreconsolidationStresses.ToArray();
            PreconsolidationStress[] actualPreconsolidationStresses = actual.PreconsolidationStresses.ToArray();

            AssertLayers(expectedLayers, actualLayers);
            AssertPreconsolidationStresses(expectedPreconsolidationStresses, actualPreconsolidationStresses);
        }

        /// <summary>
        /// Asserts whether <paramref name="original"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsWaternetInput"/>.</param>
        /// <param name="actual">The actual <see cref="DrainageConstruction"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertDrainageConstruction(IMacroStabilityInwardsWaternetInput original, DrainageConstruction actual)
        {
            Assert.AreEqual(original.DrainageConstructionPresent, actual.IsPresent);
            Assert.AreEqual(original.XCoordinateDrainageConstruction, actual.XCoordinate);
            Assert.AreEqual(original.ZCoordinateDrainageConstruction, actual.ZCoordinate);
        }

        /// <summary>
        /// Asserts whether <paramref name="original"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsLocationInput"/>.</param>
        /// <param name="actual">The actual <see cref="PhreaticLineOffsets"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertPhreaticLineOffsets(IMacroStabilityInwardsLocationInput original, PhreaticLineOffsets actual)
        {
            Assert.AreEqual(original.UseDefaultOffsets, actual.UseDefaults);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeTopAtRiver, actual.BelowDikeTopAtRiver);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeTopAtPolder, actual.BelowDikeTopAtPolder);
            Assert.AreEqual(original.PhreaticLineOffsetBelowDikeToeAtPolder, actual.BelowDikeToeAtPolder);
            Assert.AreEqual(original.PhreaticLineOffsetBelowShoulderBaseInside, actual.BelowShoulderBaseInside);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsWaternetInput"/>.</param>
        /// <param name="actual">The actual <see cref="WaternetCalculatorInput"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertGenericInput(IMacroStabilityInwardsWaternetInput original, WaternetCalculatorInput actual)
        {
            AssertSoilProfile(original.SoilProfileUnderSurfaceLine, actual.SoilProfile);
            AssertDrainageConstruction(original, actual.DrainageConstruction);
            Assert.AreSame(original.SurfaceLine, actual.SurfaceLine);
            Assert.AreEqual(original.DikeSoilScenario, actual.DikeSoilScenario);
            Assert.AreEqual(original.WaterLevelRiverAverage, actual.WaterLevelRiverAverage);
            Assert.AreEqual(original.MinimumLevelPhreaticLineAtDikeTopRiver, actual.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(original.MinimumLevelPhreaticLineAtDikeTopPolder, actual.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(original.LeakageLengthOutwardsPhreaticLine3, actual.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(original.LeakageLengthInwardsPhreaticLine3, actual.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(original.LeakageLengthOutwardsPhreaticLine4, actual.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(original.LeakageLengthInwardsPhreaticLine4, actual.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(original.PiezometricHeadPhreaticLine2Outwards, actual.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(original.PiezometricHeadPhreaticLine2Inwards, actual.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(original.AdjustPhreaticLine3And4ForUplift, actual.AdjustPhreaticLine3And4ForUplift);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="IMacroStabilityInwardsPreconsolidationStress"/> array.</param>
        /// <param name="actual">The actual <see cref="PreconsolidationStress"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertPreconsolidationStresses(IMacroStabilityInwardsPreconsolidationStress[] original,
                                                           PreconsolidationStress[] actual)
        {
            Assert.AreEqual(original.Length, actual.Length);
            for (var i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(original[i]).GetDesignValue(), actual[i].Stress);
                Assert.AreSame(original[i].Location, actual[i].Coordinate);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSoilLayer2D"/> array.</param>
        /// <param name="actual">The actual <see cref="SoilLayer"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="original"/>
        /// contains an item with an invalid value of the enum <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="original"/>
        /// contains an item with an unsupported value of <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        private static void AssertLayers(MacroStabilityInwardsSoilLayer2D[] original, SoilLayer[] actual)
        {
            Assert.AreEqual(original.Length, actual.Length);

            for (var i = 0; i < original.Length; i++)
            {
                CollectionAssert.AreEqual(original[i].OuterRing.Points, actual[i].OuterRing);

                AssertLayers(original[i].NestedLayers.ToArray(), actual[i].NestedLayers.ToArray());

                MacroStabilityInwardsSoilLayerData expectedData = original[i].Data;
                Assert.AreEqual(expectedData.MaterialName, actual[i].MaterialName);
                Assert.AreEqual(expectedData.UsePop, actual[i].UsePop);
                Assert.AreEqual(expectedData.IsAquifer, actual[i].IsAquifer);
                Assert.AreEqual(0.0, actual[i].Dilatancy);
                Assert.AreEqual(WaterPressureInterpolationModel.Automatic, actual[i].WaterPressureInterpolationModel);
                Assert.AreEqual(ConvertShearStrengthModel(expectedData.ShearStrengthModel), actual[i].ShearStrengthModel);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(expectedData).GetDesignValue(), actual[i].AbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(expectedData).GetDesignValue(), actual[i].BelowPhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(expectedData).GetDesignValue(), actual[i].Cohesion);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(expectedData).GetDesignValue(), actual[i].FrictionAngle);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(expectedData).GetDesignValue(), actual[i].StrengthIncreaseExponent);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(expectedData).GetDesignValue(), actual[i].ShearStrengthRatio);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(expectedData).GetDesignValue(), actual[i].Pop);
            }
        }

        /// <summary>
        /// Converts <paramref name="shearStrengthModel"/> to a <see cref="ShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The original shear strength model</param>
        /// <returns>A converted shear strength model</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value of the enum <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an unsupported value.</exception>
        private static ShearStrengthModel ConvertShearStrengthModel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int) shearStrengthModel,
                                                       typeof(MacroStabilityInwardsShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                    return ShearStrengthModel.SuCalculated;
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return ShearStrengthModel.CPhi;
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return ShearStrengthModel.CPhiOrSuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}