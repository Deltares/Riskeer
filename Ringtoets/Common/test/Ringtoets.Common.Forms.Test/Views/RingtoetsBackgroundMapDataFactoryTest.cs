// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Utils.TypeConverters;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsBackgroundMapDataFactoryTest
    {
        private static IEnumerable<TestCaseData> WmtsMapDatas
        {
            get
            {
                yield return new TestCaseData(WmtsMapData.CreateDefaultPdokMapData())
                    .SetName("Configured WMTS map data.");
                yield return new TestCaseData(WmtsMapData.CreateUnconnectedMapData())
                    .SetName("Not configured WMTS map data.");
            }
        }

        [Test]
        public void CreateBackgroundMapData_BackgroundMapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundData", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(WmtsMapDatas))]
        public void CreateBackgroundMapData_WmtsConfiguredBackgroundData_ReturnWmtsMapData(WmtsMapData mapData)
        {
            // Setup
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            // Call
            ImageBasedMapData backgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(backgroundData, backgroundMapData);
        }

        [Test]
        public void CreateBackgroundMapData_WellKnownConfiguredBackgroundData_ReturnWellKnownMapData()
        {
            // Setup
            var random = new Random(21);
            var wellKnownTileSource = random.NextEnumValue<WellKnownTileSource>();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(wellKnownTileSource));

            // Call
            ImageBasedMapData backgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(backgroundData, backgroundMapData);
        }

        [Test]
        public void CreateBackgroundMapData_InvalidWellKnownConfiguredBackgroundData_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var backgroundData = new BackgroundData(new WellKnownBackgroundDataConfiguration((RingtoetsWellKnownTileSource) 1337));

            // Call
            TestDelegate call = () => RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }
    }
}