// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class PluginBaseTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new SimplePlugin())
            {
                // Assert
                Assert.IsNull(plugin.Gui);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void Gui_SetValue_GetNewlySetValue()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin())
            {
                // Call
                plugin.Gui = gui;

                // Assert
                Assert.AreEqual(gui, plugin.Gui);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Activate_DoesNotThrow()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                TestDelegate call = () => plugin.Activate();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        [Test]
        public void Deactivate_DoesNotThrow()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                TestDelegate call = () => plugin.Deactivate();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<PropertyInfo> infos = plugin.GetPropertyInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<ViewInfo> infos = plugin.GetViewInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<TreeNodeInfo> infos = plugin.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<object> chidrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(null);

                // Assert
                CollectionAssert.IsEmpty(chidrenWithViewDefinitions);
            }
        }

        [Test]
        public void GetImportInfos_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<ImportInfo> infos = plugin.GetImportInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
        }

        [Test]
        public void GetExportInfos_ReturnsEmpty()
        {
            // Setup
            using (var plugin = new SimplePlugin())
            {
                // Call
                IEnumerable<ExportInfo> infos = plugin.GetExportInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
        }

        [Test]
        public void Dispose_SetGuiToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            mocks.ReplayAll();

            var plugin = new SimplePlugin
            {
                Gui = gui
            };

            // Call
            plugin.Dispose();

            // Assert
            Assert.IsNull(plugin.Gui);
            mocks.VerifyAll();
        }

        private class SimplePlugin : PluginBase {}
    }
}