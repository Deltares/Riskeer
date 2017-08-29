﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Threading;
using System.Windows.Forms.Integration;
using Core.Common.Utils.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.Forms.Layout;
using NUnit.Framework;
using WPFExtensions.Controls;

namespace Core.Components.GraphSharp.TestUtil.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class PointedTreeGraphControlHelperTest
    {
        [Test]
        public void GetPointedTreeGraph_ControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PointedTreeGraphControlHelper.GetPointedTreeGraph(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("control", exception.ParamName);
        }

        [Test]
        public void GetPointedTreeGraph_ValidControl_ReturnsPointedTreeGraph()
        {
            // Setup
            using (var control = new PointedTreeGraphControl())
            {
                // Call
                PointedTreeGraph pointedTreeGraph = PointedTreeGraphControlHelper.GetPointedTreeGraph(control);

                // Assert
                var elementHost = TypeUtils.GetField<ElementHost>(control, "wpfElementHost");
                var zoomControl = (ZoomControl) elementHost.Child;
                var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
                PointedTreeGraph expectedGraph = graphLayout.Graph;

                Assert.AreSame(expectedGraph, pointedTreeGraph);
            }
        }

        [Test]
        public void GetZoomControl_ControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PointedTreeGraphControlHelper.GetZoomControl(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("control", exception.ParamName);
        }

        [Test]
        public void GetZoomControl_ValidControl_ReturnsZoomControl()
        {
            // Setup
            using (var control = new PointedTreeGraphControl())
            {
                // Call
                ZoomControl zoomControl = PointedTreeGraphControlHelper.GetZoomControl(control);

                // Assert
                var elementHost = TypeUtils.GetField<ElementHost>(control, "wpfElementHost");
                var expectedZoomControl = (ZoomControl) elementHost.Child;

                Assert.AreSame(expectedZoomControl, zoomControl);
            }
        }
    }
}