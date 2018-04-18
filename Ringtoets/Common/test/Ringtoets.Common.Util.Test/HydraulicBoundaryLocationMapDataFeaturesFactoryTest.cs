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
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.Common.Util.TestUtil;

namespace Ringtoets.Common.Util.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateHydraulicBoundaryLocationFeature_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeature_WithLocation_ReturnFeature()
        {
            // Setup
            AggregatedHydraulicBoundaryLocation location = AggregatedHydraulicBoundaryLocationTestHelper.Create();

            // Call
            MapFeature feature = HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(location);

            // Assert
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForFactorizedSignalingNorm.ToString(),
                feature, "h(A+->A)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForSignalingNorm.ToString(),
                feature, "h(A->B)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForLowerLimitNorm.ToString(),
                feature, "h(B->C)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForFactorizedLowerLimitNorm.ToString(),
                feature, "h(C->D)");

            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForFactorizedSignalingNorm.ToString(),
                feature, "Hs(A+->A)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForSignalingNorm.ToString(),
                feature, "Hs(A->B)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForLowerLimitNorm.ToString(),
                feature, "Hs(B->C)");
            MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForFactorizedLowerLimitNorm.ToString(),
                feature, "Hs(C->D)");
        }
    }
}