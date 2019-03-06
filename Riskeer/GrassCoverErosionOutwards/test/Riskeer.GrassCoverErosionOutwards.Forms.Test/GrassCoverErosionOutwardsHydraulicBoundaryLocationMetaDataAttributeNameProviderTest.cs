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

using NUnit.Framework;
using Riskeer.GrassCoverErosionOutwards.Util;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var nameProvider = new GrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider();

            // Assert
            Assert.IsInstanceOf<IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider>(nameProvider);
            Assert.AreEqual("h_Iv", nameProvider.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("h_IIv", nameProvider.WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("h_IIIv", nameProvider.WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("h_IVv", nameProvider.WaterLevelCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("h_Vv", nameProvider.WaterLevelCalculationForFactorizedLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_Iv", nameProvider.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("Hs_IIv", nameProvider.WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("Hs_IIIv", nameProvider.WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_IVv", nameProvider.WaveHeightCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_Vv", nameProvider.WaveHeightCalculationForFactorizedLowerLimitNormAttributeName);
        }
    }
}