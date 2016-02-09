using System;
using System.Linq;

using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Test.Forms.ViewManager;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        private MessageWindowLogAppender originalAppender;

        [SetUp]
        public void SetUp()
        {
            originalAppender = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalAppender;
        }

        [Test]
        [STAThread]
        public void DisposingGuiDisposesApplication()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var applicationCore = mocks.Stub<ApplicationCore>();
            applicationCore.Expect(ac => ac.Dispose());
            mocks.ReplayAll();

            var gui = new RingtoetsGui(new MainWindow(), projectStore, applicationCore);

            // Call
            gui.Dispose();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_WithFile_LoadProjectFromFile()
        {
            // Setup
            var testFile = "SomeFile";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile));
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                // Call
                gui.Run(testFile);

                // Assert
                Assert.AreEqual(testFile, gui.ProjectFilePath);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void CheckViewPropertyEditorIsInitialized()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (new RingtoetsGui(new MainWindow(), projectStore))
            {
                Assert.NotNull(ViewPropertyEditor.ViewCommands);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GetAllDataWithViewDefinitionsRecursively_DataHasNoViewDefinitions_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                var rootData = new object();

                // Call
                var dataInstancesWithViewDefinitions = ringtoetsGui.GetAllDataWithViewDefinitionsRecursively(rootData);

                // Assert
                CollectionAssert.IsEmpty(dataInstancesWithViewDefinitions);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRoot_ReturnRootObject()
        {
            // Setup
            var rootData = new object();

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

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

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore))
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
        [STAThread]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRootAndChild_ReturnRootAndChild()
        {
            // Setup
            object rootData = 1;
            object rootChild = 2;

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
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

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore))
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

        [Test]
        [RequiresSTA]
        public void ActiveViewChanged_LastDocumentViewClosed_EventFired()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                gui.Run();

                var view = new TestView();

                // Precondition
                Assert.AreEqual(0, gui.DocumentViews.Count);

                gui.DocumentViews.Add(view);

                var hitCount = 0;
                gui.ActiveViewChanged += (s, e) => hitCount++;

                // Call
                gui.DocumentViews.RemoveAt(0);

                // Assert
                Assert.AreEqual(0, gui.DocumentViews.Count);
                Assert.AreEqual(1, hitCount);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GetTreeNodeInfos_NoPluginsConfigured_EmptyList()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                // Call
                var result = gui.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(result);
            }
        }

        [Test]
        [RequiresSTA]
        public void GetTreeNodeInfos_MultiplePluginsConfigured_RetrievesTreeNodeInfosFromPlugins()
        {
            // Setup
            var nodesPluginA = new[]
            {
                new TreeNodeInfo(),
                new TreeNodeInfo()
            };
            var nodesPluginB = new[]
            {
                new TreeNodeInfo(),
            };
            var nodesPluginC = new[]
            {
                new TreeNodeInfo(),
                new TreeNodeInfo(),
                new TreeNodeInfo(),
            };

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var pluginA = mocks.Stub<GuiPlugin>();
            pluginA.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginA);
            pluginA.Stub(p => p.Dispose());
            pluginA.Stub(p => p.Deactivate());
            var pluginB = mocks.Stub<GuiPlugin>();
            pluginB.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginB);
            pluginB.Stub(p => p.Dispose());
            pluginB.Stub(p => p.Deactivate());
            var pluginC = mocks.Stub<GuiPlugin>();
            pluginC.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginC);
            pluginC.Stub(p => p.Dispose());
            pluginC.Stub(p => p.Deactivate());
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore))
            {
                gui.Plugins.Add(pluginA);
                gui.Plugins.Add(pluginB);
                gui.Plugins.Add(pluginC);

                // Call
                var result = gui.GetTreeNodeInfos();

                // Assert
                var expected = nodesPluginA.Concat(nodesPluginB).Concat(nodesPluginC);
                CollectionAssert.AreEquivalent(expected, result);
            }

            mocks.VerifyAll();
        }
    }
}