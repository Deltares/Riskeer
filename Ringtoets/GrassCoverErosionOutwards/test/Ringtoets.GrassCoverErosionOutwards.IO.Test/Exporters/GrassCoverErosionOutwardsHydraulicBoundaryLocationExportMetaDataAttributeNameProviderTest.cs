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
            Assert.AreEqual("h gr.I", nameProvider.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("h gr.II", nameProvider.WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("h gr.III", nameProvider.WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("h gr.IV", nameProvider.WaterLevelCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("h gr.V", nameProvider.WaterLevelCalculationForFactorizedLowerLimitNormAttributeName);
            Assert.AreEqual("Hs gr.I", nameProvider.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            Assert.AreEqual("Hs gr.II", nameProvider.WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName);
            Assert.AreEqual("Hs gr.III", nameProvider.WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName);
            Assert.AreEqual("Hs gr.IV", nameProvider.WaveHeightCalculationForLowerLimitNormAttributeName);
            Assert.AreEqual("Hs gr.V", nameProvider.WaveHeightCalculationForFactorizedLowerLimitNormAttributeName);
        }
    }
}