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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class BackgroundDataPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(BackgroundDataProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(BackgroundData), info.DataType);
            Assert.AreEqual(typeof(BackgroundDataProperties), info.PropertyObjectType);
        }

        [Test]
        [TestCaseSource(nameof(ValidBackgroundDatas))]
        public void CreateInstance_ValidBackgroundData_ReturnBackgroundDataProperties(BackgroundData backgroundData)
        {
            // Call
            IObjectProperties objectProperties = info.CreateInstance(backgroundData);

            // Assert
            Assert.IsInstanceOf<BackgroundDataProperties>(objectProperties);
            Assert.AreSame(backgroundData, objectProperties.Data);
        }

        private static IEnumerable<TestCaseData> ValidBackgroundDatas()
        {
            var wellKnownMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingRoads);
            WmtsMapData wmtsMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            yield return new TestCaseData(
                    new BackgroundData(new TestBackgroundDataConfiguration()))
                .SetName("Arbitrary BackgroundData Configuration");
            yield return new TestCaseData(
                new BackgroundData(new WellKnownBackgroundDataConfiguration((RingtoetsWellKnownTileSource) wellKnownMapData.TileSource))
                {
                    Name = wellKnownMapData.Name
                }).SetName("WellKnown BingRoads BackgroundData");
            yield return new TestCaseData(new BackgroundData(new WmtsBackgroundDataConfiguration(wmtsMapData.IsConfigured,
                                                                                                 wmtsMapData.SourceCapabilitiesUrl,
                                                                                                 wmtsMapData.SelectedCapabilityIdentifier,
                                                                                                 wmtsMapData.PreferredFormat))
            {
                Name = wmtsMapData.Name
            }).SetName("Wmts DefaultPdok BackgroundData");
        }
    }
}