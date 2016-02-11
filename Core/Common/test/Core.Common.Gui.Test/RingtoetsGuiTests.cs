using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using Core.Common.Base.Data;
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

using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class RingtoetsGuiTests
    {
        private MessageWindowLogAppender originalMessageWindowLogAppender;
        private IViewCommands originalViewPropertyEditor;

        [SetUp]
        public void SetUp()
        {
            originalMessageWindowLogAppender = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();

            originalViewPropertyEditor = ViewPropertyEditor.ViewCommands;
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalMessageWindowLogAppender;
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

                // Check for OS settings that allow visual styles to be rendered in the first place:
                if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser &&
                    (Application.VisualStyleState == VisualStyleState.ClientAreaEnabled ||
                     Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled))
                {
                    Assert.IsTrue(Application.RenderWithVisualStyles,
                        "OS configured to support visual styles, therefore GUI should enable this rendering style.");
                }
                else
                {
                    // 
                    Assert.IsFalse(Application.RenderWithVisualStyles,
                        "OS not supporting visual styles, therefore application shouldn't be render with visual styles.");
                }
            }
            mocks.VerifyAll();
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
        public void Dispose_ApplicationCoreSet_DisposesOfApplicationCore()
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
        public void Dispose_GuipluginsAdded_PluginsDisabledAndRemovedAndDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var guiPluginMock = mocks.Stub<GuiPlugin>();
            guiPluginMock.Expect(p => p.Deactivate());
            guiPluginMock.Expect(p => p.Dispose());
            mocks.ReplayAll();

            var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings());
            gui.Plugins.Add(guiPluginMock);

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_GuiPluginAddedButThrowsExceptionDuringDeactivation_LogErrorAndStillDisposeAndRemove()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var guiPluginMock = mocks.Stub<GuiPlugin>();
            guiPluginMock.Expect(p => p.Deactivate()).Throw(new Exception("Bad stuff happening!"));
            guiPluginMock.Expect(p => p.Dispose());
            mocks.ReplayAll();

            var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings());
            gui.Plugins.Add(guiPluginMock);

            // Call
            Action call = () => gui.Dispose();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.", 1);
            Assert.IsNull(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasSelection_ClearSelection()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings())
            {
                Selection = new object()
            };

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(gui.Selection);
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasProject_ClearProject()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings())
            {
                Project = new Project()
            };

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(gui.Project);
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasMainWindow_DiposeOfMainWindow()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                var gui = new RingtoetsGui(mainWindow, projectStore, new ApplicationCore(), new GuiCoreSettings());

                // Call
                gui.Dispose();

                // Assert
                Assert.IsTrue(mainWindow.IsWindowDisposed);
                Assert.IsNull(gui.MainWindow);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasInitializedMessageWindowForLogAppender_ClearMessageWindow()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var messageWindowLogAppender = new MessageWindowLogAppender();

            var rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            rootLogger.AddAppender(messageWindowLogAppender);

            try
            {
                using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
                {
                    gui.Run();

                    // Precondition:
                    Assert.IsNotNull(MessageWindowLogAppender.Instance.MessageWindow);
                    Assert.IsNotNull(messageWindowLogAppender.MessageWindow);

                    // Call
                    gui.Dispose();

                    // Assert
                    Assert.IsNull(MessageWindowLogAppender.Instance.MessageWindow);
                    Assert.IsNull(messageWindowLogAppender.MessageWindow);
                    CollectionAssert.DoesNotContain(rootLogger.Appenders, messageWindowLogAppender);
                }
            }
            finally
            {
                rootLogger.RemoveAppender(messageWindowLogAppender);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasOpenedToolView_ToolViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using(var toolView = new TestView())
            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Run();

                gui.OpenToolView(toolView);

                // Call
                gui.Dispose();

                // Assert
                Assert.IsNull(gui.ToolWindowViews);
                Assert.IsTrue(toolView.IsDisposed);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_HasOpenedDocumentView_DocumentViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var documentView = new TestView())
            using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Run();

                gui.DocumentViews.Add(documentView);

                // Call
                gui.Dispose();

                // Assert
                Assert.IsNull(gui.DocumentViews);
                Assert.IsNull(gui.DocumentViewsResolver);
                Assert.IsTrue(documentView.IsDisposed);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_NoMessageWindowLogAppender_AddNewLogAppender()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            Logger rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();

            try
            {
                using(var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
                {
                    // Call
                    gui.Run();

                    // Assert
                    Assert.AreEqual(1, rootLogger.Appenders.Count);
                    IAppender appender = rootLogger.Appenders[0];
                    Assert.IsInstanceOf<MessageWindowLogAppender>(appender);
                    Assert.AreSame(appender, MessageWindowLogAppender.Instance);
                    Assert.IsTrue(rootLogger.Repository.Configured);
                }
            }
            finally
            {
                rootLogger.RemoveAllAppenders();
                foreach (var appender in originalAppenders)
                {
                    rootLogger.AddAppender(appender);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_AlreadyHasMessageWindowLogAppender_NoChangesToLogAppenders()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender();

            Logger rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();
            rootLogger.AddAppender(appender);
            var rootloggerConfigured = rootLogger.Repository.Configured;

            try
            {
                using (var gui = new RingtoetsGui(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
                {
                    // Call
                    gui.Run();

                    // Assert
                    Assert.AreEqual(1, rootLogger.Appenders.Count);
                    Assert.AreSame(appender, rootLogger.Appenders[0]);
                    Assert.AreSame(appender, MessageWindowLogAppender.Instance);
                    Assert.AreEqual(rootloggerConfigured, rootLogger.Repository.Configured);
                }
            }
            finally
            {
                rootLogger.RemoveAllAppenders();
                foreach (var originalAppender in originalAppenders)
                {
                    rootLogger.AddAppender(originalAppender);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_WithFile_LoadProjectFromFile()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = string.Format("{0}.rtd", fileName);

            var deserializedProject = new Project();

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile)).Return(deserializedProject);
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new RingtoetsGui(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
                Assert.AreEqual(testFile, gui.ProjectFilePath);
                Assert.AreSame(deserializedProject, gui.Project);
                Assert.AreEqual(fileName, gui.Project.Name,
                    "Project name should be updated to the name of the file.");

                Assert.AreSame(gui.Selection, gui.Project);
                var expectedTitle = string.Format("{0} - {1} {2}",
                                                  fileName, fixedSettings.MainWindowTitle, SettingsHelper.ApplicationVersion);
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_LoadingFromFileThrowsStorageException_LogErrorAndLoadDefaultProjectInstead()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = string.Format("{0}.rtd", fileName);

            const string storageExceptionText = "<Some error preventing the project from being opened>";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile)).Throw(new StorageException(storageExceptionText));
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new RingtoetsGui(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    storageExceptionText,
                    "Het is niet gelukt om het Ringtoetsproject te laden.",
                    "Nieuw project aanmaken..."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);

                Assert.IsNull(gui.ProjectFilePath);
                const string expectedProjectName = "Project";
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(string.Empty, gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
                Assert.AreEqual(0, gui.Project.StorageId);

                Assert.AreSame(gui.Selection, gui.Project);
                var expectedTitle = string.Format("{0} - {1} {2}",
                                                  expectedProjectName, fixedSettings.MainWindowTitle, SettingsHelper.ApplicationVersion);
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        [STAThread]
        public void Run_WithoutFile_CreateNewDefaultProject(string path)
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.StrictMock<IStoreProject>();
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<title part>"
            };

            using(var mainWindow = new MainWindow())
            using (var gui = new RingtoetsGui(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
            {
                // Call
                Action call = () => gui.Run(path);

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, "Nieuw project aanmaken...");

                Assert.IsNull(gui.ProjectFilePath);
                const string expectedProjectName = "Project";
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual(string.Empty, gui.Project.Description);
                CollectionAssert.IsEmpty(gui.Project.Items);
                Assert.AreEqual(0, gui.Project.StorageId);

                Assert.AreSame(gui.Selection, gui.Project);
                var expectedTitle = string.Format("{0} - {1} {2}",
                                                  expectedProjectName, fixedSettings.MainWindowTitle, SettingsHelper.ApplicationVersion);
                Assert.AreEqual(expectedTitle, mainWindow.Title);
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
    }
}