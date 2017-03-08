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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class BackgroundDataPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        private static readonly WellKnownTileSourceMapData wellKnownMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingRoads);
        private static readonly WmtsMapData wmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

        private static IEnumerable<TestCaseData> ValidBackgroundMapDatas
        {
            get
            {
                yield return new TestCaseData(new BackgroundData());
                yield return new TestCaseData(new BackgroundData
                {
                    Name = wellKnownMapData.Name,
                    BackgroundMapDataType = BackgroundMapDataType.WellKnown,
                    IsConfigured = wellKnownMapData.IsConfigured
                });
                yield return new TestCaseData(new BackgroundData
                {
                    Name = wmtsMapData.Name,
                    BackgroundMapDataType = BackgroundMapDataType.Wmts,
                    IsConfigured = wmtsMapData.IsConfigured
                });
            }
        }

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
        [TestCaseSource(nameof(ValidBackgroundMapDatas))]
        public void CreateInstance_ValidBackgroundMapData_ReturnBackgroundMapDataProperties(BackgroundData backgroundData)
        {
            // Call
            IObjectProperties objectProperties = info.CreateInstance(backgroundData);

            // Assert
            Assert.IsInstanceOf<BackgroundDataProperties>(objectProperties);
            Assert.AreSame(backgroundData, objectProperties.Data);
        }
    }
}