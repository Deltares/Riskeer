// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutPlugin_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLegendController(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithToolViewController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartLegendController(toolViewController);

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsLegendViewOpen_LegendViewOpenAndClosedState_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(toolViewController);

            // Call
            var result = controller.IsLegendViewOpen();

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleLegendView_LegendViewOpenAndClosedState_TogglesStateOfLegendView(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            if (open)
            {
                toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(false);
                toolViewController.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(c => true)));
                toolViewController.Expect(p => p.CloseToolView(Arg<ChartLegendView>.Matches(c => true)));
            }
            else
            {
                toolViewController.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(c => true)));
            }
            toolViewController.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(toolViewController);

            if (open)
            {
                controller.ToggleLegend();
            }

            // Call
            controller.ToggleLegend();

            // Assert
            mocks.VerifyAll();
        }
    }
}