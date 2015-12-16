using System.Linq;

using Core.Common.Base.Plugin;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Test.Gui
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        [Test]
        public void DisposingGuiDisposesApplication()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = mocks.Stub<ApplicationCore>();

            applicationCore.Expect(ac => ac.Dispose());

            mocks.ReplayAll();

            var gui = new RingtoetsGui(applicationCore);

            // Call
            gui.Dispose();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CheckViewPropertyEditorIsInitialized()
        {
            using (var gui = new RingtoetsGui())
            {
                Assert.NotNull(ViewPropertyEditor.Gui);
            }
        }

        [Test]
        public void GetAllDataWithViewDefinitionsRecursively_DataHasNoViewDefinitions_ReturnEmpty()
        {
            // Setup
            using (var ringtoetsGui = new RingtoetsGui())
            {
                var rootData = new object();

                // Call
                var dataInstancesWithViewDefinitions = ringtoetsGui.GetAllDataWithViewDefinitionsRecursively(rootData);

                // Assert
                CollectionAssert.IsEmpty(dataInstancesWithViewDefinitions);
            }
        }

        [Test]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRoot_ReturnRootObject()
        {
            // Setup
            var rootData = new object();

            var mocks = new MockRepository();
            var plugin1 = mocks.StrictMock<GuiPlugin>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData
            });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<GuiPlugin>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData
            });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            mocks.ReplayAll();

            using (var ringtoetsGui = new RingtoetsGui())
            {
                ringtoetsGui.Plugins.Add(plugin1);
                ringtoetsGui.Plugins.Add(plugin2);

                // Call
                var dataInstancesWithViewDefinitions = ringtoetsGui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

                // Assert
                var expectedDataDefinitions = new[]
                {
                    rootData
                };
                CollectionAssert.AreEquivalent(expectedDataDefinitions, dataInstancesWithViewDefinitions);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRootAndChild_ReturnRootAndChild()
        {
            // Setup
            object rootData = 1;
            object rootChild = 2;

            var mocks = new MockRepository();
            var plugin1 = mocks.StrictMock<GuiPlugin>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData, rootChild
            });
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootChild)).Return(new[]
            {
                rootChild
            });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<GuiPlugin>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootChild, rootData
            });
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootChild)).Return(new[]
            {
                rootChild
            });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            mocks.ReplayAll();

            using (var ringtoetsGui = new RingtoetsGui())
            {
                ringtoetsGui.Plugins.Add(plugin1);
                ringtoetsGui.Plugins.Add(plugin2);

                // Call
                var dataInstancesWithViewDefinitions = ringtoetsGui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

                // Assert
                var expectedDataDefinitions = new[]
                {
                    rootData, rootChild
                };
                CollectionAssert.AreEquivalent(expectedDataDefinitions, dataInstancesWithViewDefinitions);
            }
            mocks.VerifyAll();
        }
    }
}