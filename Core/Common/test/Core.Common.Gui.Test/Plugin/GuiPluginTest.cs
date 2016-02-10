using Core.Common.Gui.Plugin;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class GuiPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new SimpleGuiPlugin())
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

            using (var plugin = new SimpleGuiPlugin())
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

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
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

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                TestDelegate call = () => plugin.Deactivate();

                // Assert
                Assert.DoesNotThrow(call);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetPropertyInfos_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                var infos = plugin.GetPropertyInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                var infos = plugin.GetViewInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetTreeNodeInfos_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                var infos = plugin.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(infos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            mocks.ReplayAll();

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                var infos = plugin.GetChildDataWithViewDefinitions(null);

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

            using (var plugin = new SimpleGuiPlugin { Gui = gui })
            {
                // Call
                plugin.Dispose();

                // Assert
                Assert.IsNull(plugin.Gui);
            }
            mocks.VerifyAll();
        }

        private class SimpleGuiPlugin : GuiPlugin
        {
            
        }
    }
}