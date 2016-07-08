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

using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Commands
{
    [TestFixture]
    public class ToggleLegendViewCommandTest
    {
        [Test]
        public void Constructor_Always_CreatesICommand()
        {
            // Call
            var command = new ToggleLegendViewCommand(null);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Setup
            var command = new ToggleLegendViewCommand(null);

            // Call & Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checked_LegendViewOpenOrClosed_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(plugin);
            var command = new ToggleLegendViewCommand(controller);

            // Call
            var result = command.Checked;

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_Always_TogglesLegend()
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();

            // Open first
            using (mocks.Ordered())
            {
                plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(v => true)));

                // Then close
                plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(true);
                plugin.Expect(p => p.CloseToolView(Arg<ChartLegendView>.Matches(v => true)));
            }
            mocks.ReplayAll();

            var controller = new ChartLegendController(plugin);
            var command = new ToggleLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}