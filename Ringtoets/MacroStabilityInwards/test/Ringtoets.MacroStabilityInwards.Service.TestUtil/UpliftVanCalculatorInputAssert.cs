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

using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Ringtoets.MacroStabilityInwards.Service.TestUtil
{
    /// <summary>
    /// Class for asserting Uplift Van calculator input.
    /// </summary>
    public static class UpliftVanCalculatorInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSoilProfile"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertSoilProfile(MacroStabilityInwardsSoilProfileUnderSurfaceLine original, UpliftVanSoilProfile actual)
        {
            MacroStabilityInwardsSoilLayerUnderSurfaceLine[] expectedLayers = original.Layers.ToArray();
            UpliftVanSoilLayer[] actualLayers = actual.Layers.ToArray();

            MacroStabilityInwardsPreconsolidationStress[] expectedPreconsolidationStresses = original.PreconsolidationStresses.ToArray();
            UpliftVanPreconsolidationStress[] actualPreconsolidationStresses = actual.PreconsolidationStresses.ToArray();

            AssertLayers(expectedLayers, actualLayers);
            AssertPreconsolidationStresses(expectedPreconsolidationStresses, actualPreconsolidationStresses);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsPreconsolidationStress"/> array.</param>
        /// <param name="actual">The actual <see cref="UpliftVanPreconsolidationStress"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertPreconsolidationStresses(MacroStabilityInwardsPreconsolidationStress[] original,
                                                           UpliftVanPreconsolidationStress[] actual)
        {
            Assert.AreEqual(original.Length, actual.Length);
            for (var i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(original[i]).GetDesignValue(), actual[i].Stress);
                Assert.AreEqual(original[i].Location, actual[i].Coordinate);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="MacroStabilityInwardsSoilLayerUnderSurfaceLine"/> array.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSoilLayer"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        private static void AssertLayers(MacroStabilityInwardsSoilLayerUnderSurfaceLine[] original, UpliftVanSoilLayer[] actual)
        {
            Assert.AreEqual(original.Length, actual.Length);

            for (var i = 0; i < original.Length; i++)
            {
                Assert.AreSame(original[i].OuterRing, actual[i].OuterRing);
                Assert.AreSame(original[i].Holes, actual[i].Holes);

                MacroStabilityInwardsSoilLayerData expectedData = original[i].Data;
                Assert.AreEqual(expectedData.MaterialName, actual[i].MaterialName);
                Assert.AreEqual(expectedData.UsePop, actual[i].UsePop);
                Assert.AreEqual(expectedData.IsAquifer, actual[i].IsAquifer);
                Assert.AreEqual(UpliftVanDilatancyType.Zero, actual[i].DilatancyType);
                Assert.AreEqual(UpliftVanWaterPressureInterpolationModel.Automatic, actual[i].WaterPressureInterpolationModel);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(expectedData).GetDesignValue(), actual[i].AbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(expectedData).GetDesignValue(), actual[i].BelowPhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(expectedData).GetDesignValue(), actual[i].Cohesion);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(expectedData).GetDesignValue(), actual[i].FrictionAngle);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(expectedData).GetDesignValue(), actual[i].StrengthIncreaseExponent);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(expectedData).GetDesignValue(), actual[i].ShearStrengthRatio);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(expectedData).GetDesignValue(), actual[i].Pop);
            }
        }
    }
}