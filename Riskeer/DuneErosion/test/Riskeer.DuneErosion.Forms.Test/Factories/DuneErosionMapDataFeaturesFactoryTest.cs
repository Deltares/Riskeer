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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.Factories;
using Ringtoets.DuneErosion.Forms.TestUtil;

namespace Riskeer.DuneErosion.Forms.Test.Factories
{
    [TestFixture]
    public class DuneErosionMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateDuneLocationsFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateDuneLocationFeatures_FailureMechanismWithoutDuneLocations_ReturnsEmptyFeaturesArray()
        {
            // Call
            IEnumerable<MapFeature> features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(new DuneErosionFailureMechanism());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDuneLocationsFeatures_WithFailureMechanism_ReturnFeatures(bool withOutput)
        {
            // Setup
            DuneLocation[] duneLocations =
            {
                CreateDuneLocation(1),
                CreateDuneLocation(2)
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);

            if (withOutput)
            {
                DuneLocationsTestHelper.SetDuneLocationCalculationOutput(failureMechanism);
            }

            // Call
            IEnumerable<MapFeature> features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(failureMechanism);

            // Assert
            DuneErosionMapFeaturesTestHelper.AssertDuneLocationFeaturesData(failureMechanism, features);
        }

        private static DuneLocation CreateDuneLocation(int seed)
        {
            var random = new Random(seed);

            int id = random.Next();
            return new DuneLocation(id, $"Location_{id}", new Point2D(random.NextDouble(), random.NextDouble()), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = random.Next(),
                D50 = random.NextDouble(),
                Offset = random.NextDouble()
            });
        }
    }
}