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
using Core.Gui.Plugin;
using Core.Gui.Plugin.Map;
using Core.Gui.PresentationObjects.Map;
using Core.Gui.PropertyClasses.Map;
using Core.Gui.TestUtil;
using NUnit.Framework;

namespace Core.Gui.Test.Plugin.Map
{
    [TestFixture]
    public class MapPropertyInfoFactoryTest
    {
        [Test]
        public void Create_Always_ReturnsPropertyInfos()
        {
            // Call
            PropertyInfo[] propertyInfos = MapPropertyInfoFactory.Create().ToArray();

            // Assert
            Assert.AreEqual(4, propertyInfos.Length);

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos,
                typeof(MapDataCollectionContext),
                typeof(MapDataCollectionProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos,
                typeof(MapPointDataContext),
                typeof(MapPointDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos,
                typeof(MapLineDataContext),
                typeof(MapLineDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos,
                typeof(MapPolygonDataContext),
                typeof(MapPolygonDataProperties));
        }
    }
}