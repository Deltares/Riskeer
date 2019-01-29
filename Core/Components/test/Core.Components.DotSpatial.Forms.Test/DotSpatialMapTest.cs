// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using Core.Components.Gis.TestUtil;
using DotSpatial.Controls;
using DotSpatial.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class DotSpatialMapTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(1e-2)]
        [TestCase(1e-3)]
        [TestCase(1e-4)]
        [TestCase(1e+8)]
        [TestCase(1e+9)]
        [TestCase(1e+10)]
        public void OnViewExtentsChanged_EdgeCases_DoesNotThrowException(double minExt)
        {
            // Setup
            using (var form = new Form())
            {
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (DotSpatialMap) new ControlTester("Map").TheObject;

                // Call
                TestDelegate test = () => map.ViewExtents = new Extent(1 + minExt, 2 + minExt, 1 + minExt, 2 + minExt);

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [IgnoreOnNoInternet]
        public void GivenMapControl_WhenPdokDataSetAsBackground_ThenMapControlShowsBackgroundLayer()
        {
            // Given
            using (var form = new Form())
            {
                var mapControl = new MapControl();
                Map mapView = mapControl.Controls.OfType<Map>().First();
                form.Controls.Add(mapControl);
                form.Show();

                // When
                mapControl.BackgroundMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

                // Then
                Assert.AreEqual(1, mapView.Layers.Count);
            }
        }

        private class IgnoreOnNoInternetAttribute : Attribute, ITestAction
        {
            public ActionTargets Targets { get; }

            public void BeforeTest(ITest test)
            {
                bool ignore;
                try
                {
                    var ping = new Ping();
                    PingReply reply = ping.Send("deltares.nl", 100);
                    ignore = reply == null || reply.Status != IPStatus.Success;
                }
                catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
                {
                    ignore = true;
                }

                if (ignore)
                {
                    Assert.Ignore("Test is Ignored, because there currently is no internet connection.");
                }
            }

            public void AfterTest(ITest test) {}
        }
    }
}