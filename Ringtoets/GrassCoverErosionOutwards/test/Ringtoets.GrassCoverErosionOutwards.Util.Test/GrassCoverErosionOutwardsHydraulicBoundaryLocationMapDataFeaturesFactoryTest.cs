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

using System;
using System.Linq;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Util.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateHydraulicBoundaryLocationFeature_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var nameProvider = mocks.Stub<IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(null, nameProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeature_NameProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location = GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper.Create();
            
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(location, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("nameProvider", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeature_WithAllData_ReturnFeature()
        {
            // Setup
            const string waterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName = "h1";
            const string waterLevelCalculationForMechanismSpecificSignalingNormAttributeName = "h2";
            const string waterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName = "h3";
            const string waterLevelCalculationForLowerLimitNormAttributeName = "h4";
            const string waterLevelCalculationForFactorizedLowerLimitNormAttributeName = "h5";
            const string waveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName = "hs1";
            const string waveHeightCalculationForMechanismSpecificSignalingNormAttributeName = "hs2";
            const string waveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName = "hs3";
            const string waveHeightCalculationForLowerLimitNormAttributeName = "hs4";
            const string waveHeightCalculationForFactorizedLowerLimitNormAttributeName = "hs5";

            var mocks = new MockRepository();
            var nameProvider = mocks.StrictMock<IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            nameProvider.Expect(provider => provider.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName)
                        .Return(waterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            nameProvider.Expect(provider => provider.WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName)
                        .Return(waterLevelCalculationForMechanismSpecificSignalingNormAttributeName);
            nameProvider.Expect(provider => provider.WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName)
                        .Return(waterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName);
            nameProvider.Expect(provider => provider.WaterLevelCalculationForLowerLimitNormAttributeName)
                        .Return(waterLevelCalculationForLowerLimitNormAttributeName);
            nameProvider.Expect(provider => provider.WaterLevelCalculationForFactorizedLowerLimitNormAttributeName)
                        .Return(waterLevelCalculationForFactorizedLowerLimitNormAttributeName);
            nameProvider.Expect(provider => provider.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName)
                        .Return(waveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            nameProvider.Expect(provider => provider.WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName)
                        .Return(waveHeightCalculationForMechanismSpecificSignalingNormAttributeName);
            nameProvider.Expect(provider => provider.WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName)
                        .Return(waveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName);
            nameProvider.Expect(provider => provider.WaveHeightCalculationForLowerLimitNormAttributeName)
                        .Return(waveHeightCalculationForLowerLimitNormAttributeName);
            nameProvider.Expect(provider => provider.WaveHeightCalculationForFactorizedLowerLimitNormAttributeName)
                        .Return(waveHeightCalculationForFactorizedLowerLimitNormAttributeName);
            mocks.ReplayAll();

            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location = GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper.Create();

            // Call
            MapFeature feature = GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(location, nameProvider);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                location.Location
            }, feature.MapGeometries.First().PointCollections.First());
            Assert.AreEqual(location.Id, feature.MetaData["ID"]);
            Assert.AreEqual(location.Name, feature.MetaData["Naam"]);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm,
                feature, waterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForMechanismSpecificSignalingNorm,
                feature, waterLevelCalculationForMechanismSpecificSignalingNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForMechanismSpecificLowerLimitNorm,
                feature, waterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForLowerLimitNorm,
                feature, waterLevelCalculationForLowerLimitNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForFactorizedLowerLimitNorm,
                feature, waterLevelCalculationForFactorizedLowerLimitNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
                feature, waveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForMechanismSpecificSignalingNorm,
                feature, waveHeightCalculationForMechanismSpecificSignalingNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForMechanismSpecificLowerLimitNorm,
                feature, waveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForLowerLimitNorm,
                feature, waveHeightCalculationForLowerLimitNormAttributeName);
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForFactorizedLowerLimitNorm,
                feature, waveHeightCalculationForFactorizedLowerLimitNormAttributeName);
            mocks.VerifyAll();
        }
    }
}