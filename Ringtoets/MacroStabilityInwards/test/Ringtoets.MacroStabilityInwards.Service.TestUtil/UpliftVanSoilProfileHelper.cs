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
    /// Helper that can be used in tests.
    /// </summary>
    public static class UpliftVanSoilProfileHelper
    {
        /// <summary>
        /// Asserts the <paramref name="actualSoilProfile"/>.
        /// </summary>
        /// <param name="originSoilProfile">The original soil profile.</param>
        /// <param name="actualSoilProfile">The profile to assert.</param>
        /// <exception cref="AssertionException"></exception>
        public static void AssertSoilProfile(MacroStabilityInwardsSoilProfileUnderSurfaceLine originSoilProfile, UpliftVanSoilProfile actualSoilProfile)
        {
            MacroStabilityInwardsSoilLayerUnderSurfaceLine[] expectedLayers = originSoilProfile.Layers.ToArray();
            UpliftVanSoilLayer[] actualLayers = actualSoilProfile.Layers.ToArray();

            MacroStabilityInwardsPreconsolidationStress[] expectedPreconsolidationStresses = originSoilProfile.PreconsolidationStresses.ToArray();
            UpliftVanPreconsolidationStress[] actualPreconsolidationStresses = actualSoilProfile.PreconsolidationStresses.ToArray();

            AssertLayers(expectedLayers, actualLayers);
            AssertPreconsolidationStresses(expectedPreconsolidationStresses, actualPreconsolidationStresses);
        }

        private static void AssertPreconsolidationStresses(MacroStabilityInwardsPreconsolidationStress[] expectedPreconsolidationStresses,
                                                           UpliftVanPreconsolidationStress[] actualPreconsolidationStresses)
        {
            Assert.AreEqual(expectedPreconsolidationStresses.Length, actualPreconsolidationStresses.Length);
            for (var i = 0; i < expectedPreconsolidationStresses.Length; i++)
            {
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(expectedPreconsolidationStresses[i]).GetDesignValue(), actualPreconsolidationStresses[i].Stress);
                Assert.AreEqual(expectedPreconsolidationStresses[i].Location, actualPreconsolidationStresses[i].Coordinate);
            }
        }

        private static void AssertLayers(MacroStabilityInwardsSoilLayerUnderSurfaceLine[] expectedLayers, UpliftVanSoilLayer[] actualLayers)
        {
            Assert.AreEqual(expectedLayers.Length, actualLayers.Length);

            for (var i = 0; i < expectedLayers.Length; i++)
            {
                Assert.AreEqual(expectedLayers[i].OuterRing, actualLayers[i].OuterRing);
                CollectionAssert.AreEqual(expectedLayers[i].Holes, actualLayers[i].Holes);

                MacroStabilityInwardsSoilLayerProperties expectedProperties = expectedLayers[i].Properties;
                Assert.AreEqual(expectedProperties.MaterialName, actualLayers[i].MaterialName);
                Assert.AreEqual(expectedProperties.UsePop, actualLayers[i].UsePop);
                Assert.AreEqual(expectedProperties.IsAquifer, actualLayers[i].IsAquifer);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(expectedProperties).GetDesignValue(), actualLayers[i].AbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(expectedProperties).GetDesignValue(), actualLayers[i].BelowPhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(expectedProperties).GetDesignValue(), actualLayers[i].Cohesion);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(expectedProperties).GetDesignValue(), actualLayers[i].FrictionAngle);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(expectedProperties).GetDesignValue(), actualLayers[i].StrengthIncreaseExponent);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(expectedProperties).GetDesignValue(), actualLayers[i].ShearStrengthRatio);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(expectedProperties).GetDesignValue(), actualLayers[i].Pop);
            }
        }
    }
}