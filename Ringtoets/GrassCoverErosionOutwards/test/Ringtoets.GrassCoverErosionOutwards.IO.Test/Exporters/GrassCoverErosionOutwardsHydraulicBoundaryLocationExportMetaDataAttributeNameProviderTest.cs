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

using NUnit.Framework;
using Ringtoets.GrassCoverErosionOutwards.IO.Exporters;
using Ringtoets.GrassCoverErosionOutwards.Util;

namespace Ringtoets.GrassCoverErosionOutwards.IO.Test.Exporters
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationExportMetaDataAttributeNameProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var nameProvider = new GrassCoverErosionOutwardsHydraulicBoundaryLocationExportMetaDataAttributeNameProvider();

            // Assert
            Assert.IsInstanceOf<IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider>(nameProvider);
            Assert.AreEqual("h_I", nameProvider.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("h_II", nameProvider.WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("h_III", nameProvider.WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("h_IV", nameProvider.WaterLevelCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("h_V", nameProvider.WaterLevelCalculationForFactorizedLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_I", nameProvider.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("Hs_II", nameProvider.WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("Hs_III", nameProvider.WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_IV", nameProvider.WaveHeightCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("Hs_V", nameProvider.WaveHeightCalculationForFactorizedLowerLimitNormAttributeName);
        }
    }
}