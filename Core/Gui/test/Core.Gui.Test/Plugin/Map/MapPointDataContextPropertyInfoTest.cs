// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using Core.Components.Gis.Data;
using Core.Gui.Plugin;
using Core.Gui.Plugin.Map;
using Core.Gui.PresentationObjects.Map;
using Core.Gui.PropertyBag;
using Core.Gui.PropertyClasses.Map;
using NUnit.Framework;

namespace Core.Gui.Test.Plugin.Map
{
    [TestFixture]
    public class MapPointDataContextPropertyInfoTest
    {
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            info = MapPropertyInfoFactory.Create().Single(pi => pi.PropertyObjectType == typeof(MapPointDataProperties));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MapPointDataContext), info.DataType);
            Assert.AreEqual(typeof(MapPointDataProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithValidArguments_NewPropertiesWithMapPointDataAsData()
        {
            // Setup
            var mapData = new MapPointData("Test");
            var context = new MapPointDataContext(mapData, new MapDataCollectionContext(new MapDataCollection("test"), null));

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MapPointDataProperties>(objectProperties);
            Assert.AreSame(mapData, objectProperties.Data);
        }
    }
}