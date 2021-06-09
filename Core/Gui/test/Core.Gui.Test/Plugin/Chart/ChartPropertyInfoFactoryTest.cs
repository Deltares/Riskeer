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
using Core.Components.Chart.Data;
using Core.Gui.Plugin;
using Core.Gui.Plugin.Chart;
using Core.Gui.PropertyClasses.Chart;
using Core.Gui.TestUtil;
using NUnit.Framework;

namespace Core.Gui.Test.Plugin.Chart
{
    public class ChartPropertyInfoFactoryTest
    {
        [Test]
        public void Create_Always_ReturnsPropertyInfos()
        {
            // Call
            PropertyInfo[] propertyInfos = ChartPropertyInfoFactory.Create().ToArray();

            // Assert
            Assert.AreEqual(6, propertyInfos.Length);

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartDataCollection),
                typeof(ChartDataCollectionProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartLineData),
                typeof(ChartLineDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartAreaData),
                typeof(ChartAreaDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartMultipleAreaData),
                typeof(ChartMultipleAreaDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartMultipleLineData),
                typeof(ChartMultipleLineDataProperties));

            PluginTestHelper.AssertPropertyInfoDefined(
                propertyInfos, typeof(ChartPointData),
                typeof(ChartPointDataProperties));
        }
    }
}