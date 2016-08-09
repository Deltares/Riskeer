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
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                TestDelegate call = () => plugin.Activate();

                // Assert
                Assert.DoesNotThrow(call);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Deactivate_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                TestDelegate call = () => plugin.Deactivate();

                // Assert
                Assert.DoesNotThrow(call);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetPropertyInfos_ReturnsEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetPropertyInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetViewInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_ReturnsEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetChildDataWithViewDefinitions(null);

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetFileImporters_ReturnsEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetFileImporters();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetExportInfos_ReturnsEmptyEnumerable()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                var infos = plugin.GetExportInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_SetGuiToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimplePlugin
            {
                Gui = gui
            })
            {
                // Call
                plugin.Dispose();

                // Assert
                Assert.IsNull(plugin.Gui);
            }
            mocks.VerifyAll();
        }

        private class SimplePlugin : PluginBase {}
    }
}