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

using System.Windows.Forms;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Commands
{
    [TestFixture]
    public class ToggleMapLegendViewCommandTest
    {
        [Test]
        public void Constructor_Always_CreatesICommand()
        {
            // Call
            var command = new ToggleMapLegendViewCommand(null);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Call
            var command = new ToggleMapLegendViewCommand(null);

            // Assert
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
            var parentWindow = mocks.StrictMock<IWin32Window>();
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);
            var command = new ToggleMapLegendViewCommand(controller);

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
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            // Open first
            using (mocks.Ordered())
            {
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(v => true)));

                // Then close
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(true);
                plugin.Expect(p => p.CloseToolView(Arg<MapLegendView>.Matches(v => true)));
            }
            mocks.ReplayAll();

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);
            var command = new ToggleMapLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}