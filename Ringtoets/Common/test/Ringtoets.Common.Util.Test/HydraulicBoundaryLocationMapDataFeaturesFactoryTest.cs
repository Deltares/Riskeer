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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.Common.Forms.TestUtil;

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
            var location = new AggregatedHydraulicBoundaryLocation(1, "test", new Point2D(0, 0),
                                                                   (RoundedDouble) 0, (RoundedDouble) 0,
                                                                   (RoundedDouble) 0, (RoundedDouble) 0,
                                                                   (RoundedDouble) 0, (RoundedDouble) 0,
                                                                   (RoundedDouble) 0, (RoundedDouble) 0);

            // Call
            MapFeature feature = HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(location);

            // Assert
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForFactorizedSignalingNorm,
                feature, "h(A+->A)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForSignalingNorm,
                feature, "h(A->B)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForLowerLimitNorm,
                feature, "h(B->C)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaterLevelCalculationForFactorizedLowerLimitNorm,
                feature, "h(C->D)");

            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForFactorizedSignalingNorm,
                feature, "Hs(A+->A)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForSignalingNorm,
                feature, "Hs(A->B)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForLowerLimitNorm,
                feature, "Hs(B->C)");
            MapFeaturesTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                location.WaveHeightCalculationForFactorizedLowerLimitNorm,
                feature, "Hs(C->D)");
        }
    }
}