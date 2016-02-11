using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Common.Gui.Test.Forms.ViewManager;
using Core.Common.Gui.Theme;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        private MessageWindowLogAppender originalAppender;
        private IViewCommands originalViewPropertyEditor;

        [SetUp]
        public void SetUp()
        {
            originalAppender = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();

            originalViewPropertyEditor = ViewPropertyEditor.ViewCommands;
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalAppender;
            ViewPropertyEditor.ViewCommands = originalViewPropertyEditor;
        }

        [Test]
        [STAThread]
        public void ParameteredConstructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new RingtoetsGui(mainWindow, projectStore, applicationCore, guiCoreSettings))
            {
                // Assert
                Assert.AreSame(applicationCore, gui.ApplicationCore);
                Assert.AreEqual(null, gui.PropertyResolver);
                Assert.AreSame(projectStore, gui.Storage);

                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.AreEqual(null, gui.Project);

                Assert.AreEqual(null, gui.Selection);

                Assert.IsInstanceOf<ProjectCommandsHandler>(gui.ProjectCommands);
                Assert.IsInstanceOf<StorageCommandHandler>(gui.StorageCommands);
                Assert.IsInstanceOf<ViewCommandHandler>(gui.ViewCommands);
                Assert.AreEqual(null, gui.ApplicationCommands);

                Assert.AreEqual(null, gui.ActiveView);
                Assert.AreEqual(null, gui.DocumentViews);
                Assert.AreEqual(null, gui.DocumentViewsResolver);

                AssertDefaultUserSettings(gui.UserSettings);
                Assert.AreSame(guiCoreSettings, gui.FixedSettings);

                CollectionAssert.IsEmpty(gui.Plugins);

                Assert.AreEqual(mainWindow, gui.MainWindow);

                Assert.AreEqual(null, gui.ToolWindowViews);

                Assert.AreSame(ViewPropertyEditor.ViewCommands, gui.ViewCommands);

                Assert.IsTrue(Application.RenderWithVisualStyles);
            }
            mocks.VerifyAll();
        }

        private static void AssertDefaultUserSettings(SettingsBase settings)
        {
            Assert.IsNotNull(settings);
            Assert.AreEqual(15, settings.Properties.Count);
            var mruList = (StringCollection)settings["mruList"];
            CollectionAssert.IsEmpty(mruList);
            var defaultViewDataTypes = (StringCollection)settings["defaultViewDataTypes"];
            CollectionAssert.IsEmpty(defaultViewDataTypes);
            var defaultViews = (StringCollection)settings["defaultViews"];
            CollectionAssert.IsEmpty(defaultViews);
            var lastVisitedPath = (string)settings["lastVisitedPath"];
            Assert.AreEqual(string.Empty, lastVisitedPath);
            var isMainWindowFullScreen = (bool)settings["MainWindow_FullScreen"];
            Assert.IsFalse(isMainWindowFullScreen);
            var x = (int)settings["MainWindow_X"];
            Assert.AreEqual(50, x);
            var y = (int)settings["MainWindow_Y"];
            Assert.AreEqual(50, y);
            var width = (int)settings["MainWindow_Width"];
            Assert.AreEqual(1024, width);
            var height = (int)settings["MainWindow_Height"];
            Assert.AreEqual(768, height);
            var startPageName = (string)settings["startPageName"];
            Assert.AreEqual("Startpagina", startPageName);
            var showStartPage = (bool)settings["showStartPage"];
            Assert.IsTrue(showStartPage);
            var showSplashScreen = (bool)settings["showSplashScreen"];
            Assert.IsTrue(showSplashScreen);
            var showHiddenDataItems = (bool)settings["showHiddenDataItems"];
            Assert.IsFalse(showHiddenDataItems);
            var colorTheme = (ColorTheme)settings["colorTheme"];
            Assert.AreEqual(ColorTheme.Generic, colorTheme);
        }

        [Test]
        [STAThread]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ParameteredConstructor_SomeArgumentIsNull_ThrowArgumentNullException(int nullArgumentIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IStoreProject projectStore = nullArgumentIndex == 1 ? null : mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            ApplicationCore applicationCore = nullArgumentIndex == 2 ? null : new ApplicationCore();
            GuiCoreSettings guiCoreSettings = nullArgumentIndex == 3 ? null : new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            
            {
                // Call
                TestDelegate call = () => new RingtoetsGui(nullArgumentIndex == 0 ? null : mainWindow, projectStore, applicationCore, guiCoreSettings);

                // Assert
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Value cannot be null.");
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_ConstuctedAfterAnotherInstanceHasBeenCreated_ThrowInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new RingtoetsGui(mainWindow, projectStore, applicationCore, guiCoreSettings))
            {
                // Call
                using (var gui2 = new RingtoetsGui(mainWindow, projectStore, applicationCore, guiCoreSettings))
                {
                    // Assert
                    Assert.Fail("Expected an InvalidOperationException to be thrown.");
                }
            }
            mocks.VerifyAll();
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

            var gui = new RingtoetsGui(new MainWindow(), projectStore, applicationCore, new GuiCoreSettings());

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

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var ringtoetsGui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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