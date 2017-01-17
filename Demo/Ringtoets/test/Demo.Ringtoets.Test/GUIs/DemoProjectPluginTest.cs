﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Gui;
using System.Threading;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Components.Gis.Data;
using Demo.Ringtoets.GUIs;
using Demo.Ringtoets.Views;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.GUIs
{
    [TestFixture]
    public class DemoProjectPluginTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            mocks.ReplayAll();

            // Call
            using (var plugin = new DemoProjectPlugin
            {
                Gui = gui
            })
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsInstanceOf<IRibbonCommandHandler>(plugin.RibbonCommandHandler);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsChartDataViewInfo()
        {
            // Setup
            using (var plugin = new DemoProjectPlugin())
            {
                // Call
                var views = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, views.Length);

                ViewInfo chartViewInfo = views[0];
                Assert.AreEqual(typeof(ChartDataCollection), chartViewInfo.DataType);
                Assert.AreEqual(typeof(ChartDataView), chartViewInfo.ViewType);
                Assert.AreEqual("Diagram", chartViewInfo.GetViewName(new ChartDataView(), null));

                ViewInfo mapViewInfo = views[1];
                Assert.AreEqual(typeof(MapData), mapViewInfo.DataType);
                Assert.AreEqual(typeof(MapDataView), mapViewInfo.ViewType);
                Assert.AreEqual("Kaart", mapViewInfo.GetViewName(new MapDataView(), null));
            }
        }
    }
}