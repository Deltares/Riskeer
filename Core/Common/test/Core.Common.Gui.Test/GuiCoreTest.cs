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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class GuiCoreTests
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
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [STAThread]
        public void ParameteredConstructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectFactory, guiCoreSettings))
            {
                // Assert
                Assert.AreEqual(null, gui.PropertyResolver);

                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.AreEqual(null, gui.Project);

                Assert.AreEqual(null, gui.Selection);

                Assert.IsInstanceOf<StorageCommandHandler>(gui.StorageCommands);
                Assert.IsInstanceOf<ViewCommandHandler>(gui.ViewCommands);
                Assert.AreEqual(null, gui.ApplicationCommands);

                Assert.AreEqual(null, gui.ViewHost);
                Assert.AreEqual(null, gui.DocumentViewController);

                Assert.AreSame(guiCoreSettings, gui.FixedSettings);

                CollectionAssert.IsEmpty(gui.Plugins);

                Assert.AreEqual(mainWindow, gui.MainWindow);

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
        public void ParameteredConstructor_SomeArgumentIsNull_ThrowsArgumentNullException(int nullArgumentIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IStoreProject projectStore = nullArgumentIndex == 1 ? null : mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            GuiCoreSettings guiCoreSettings = nullArgumentIndex == 2 ? null : new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())

            {
                // Call
                TestDelegate call = () => new GuiCore(nullArgumentIndex == 0 ? null : mainWindow, projectStore, projectFactory, guiCoreSettings);

                // Assert
                const string expectedMessage = "Value cannot be null.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_ConstuctedAfterAnotherInstanceHasBeenCreated_ThrowsInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            using (new GuiCore(mainWindow, projectStore, projectFactory, guiCoreSettings))
            {
                // Call
                using (new GuiCore(mainWindow, projectStore, projectFactory, guiCoreSettings)) {}
            }

            // Assert
            mocks.VerifyAll();
            Assert.Fail("Expected an InvalidOperationException to be thrown.");
        }

        [Test]
        [STAThread]
        public void Dispose_PluginsAdded_PluginsDisabledAndRemovedAndDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var pluginMock = mocks.Stub<PluginBase>();
            pluginMock.Expect(p => p.Deactivate());
            pluginMock.Expect(p => p.Dispose());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings());
            gui.Plugins.Add(pluginMock);

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Dispose_PluginAddedButThrowsExceptionDuringDeactivation_LogErrorAndStillDisposeAndRemove()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var pluginMock = mocks.Stub<PluginBase>();
            pluginMock.Expect(p => p.Deactivate()).Throw(new Exception("Bad stuff happening!"));
            pluginMock.Expect(p => p.Dispose());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings());
            gui.Plugins.Add(pluginMock);

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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings())
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
            var projectMock = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings())
            {
                Project = projectMock
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                var gui = new GuiCore(mainWindow, projectStore, projectFactory, new GuiCoreSettings());

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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var messageWindowLogAppender = new MessageWindowLogAppender();

            var rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            rootLogger.AddAppender(messageWindowLogAppender);

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var toolView = new TestView())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.ViewHost.AddToolView(toolView, ToolViewLocation.Left);

                // Call
                gui.Dispose();

                // Assert
                Assert.IsEmpty(gui.ViewHost.ToolViews);
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var documentView = new TestView())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.ViewHost.AddDocumentView(documentView);

                // Call
                gui.Dispose();

                // Assert
                Assert.IsEmpty(gui.ViewHost.DocumentViews);
                Assert.IsNull(gui.DocumentViewController);
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
                {
                    // Call
                    gui.Run();

                    // Assert
                    Assert.AreEqual(1, rootLogger.Appenders.Count);
                    IAppender appender = rootLogger.Appenders[0];
                    Assert.IsInstanceOf<MessageWindowLogAppender>(appender);
                    Assert.AreSame(appender, MessageWindowLogAppender.Instance);
                    Assert.IsTrue(rootLogger.Repository.Configured);

                    Assert.IsTrue(MessageWindowLogAppender.Instance.Enabled);
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender();

            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();
            rootLogger.AddAppender(appender);
            var rootloggerConfigured = rootLogger.Repository.Configured;

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
                {
                    // Call
                    gui.Run();

                    // Assert
                    Assert.AreEqual(1, rootLogger.Appenders.Count);
                    Assert.AreSame(appender, rootLogger.Appenders[0]);
                    Assert.AreSame(appender, MessageWindowLogAppender.Instance);
                    Assert.AreEqual(rootloggerConfigured, rootLogger.Repository.Configured);

                    Assert.IsTrue(MessageWindowLogAppender.Instance.Enabled);
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

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var deserializedProject = mocks.Stub<IProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile)).Return(deserializedProject);
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectFactory, fixedSettings))
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
            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Expect(ph => ph.CreateNewProject()).Return(project);
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectFactory, fixedSettings))
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

            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Expect(ph => ph.CreateNewProject()).Return(project);
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectFactory, fixedSettings))
            {
                // Call
                Action call = () => gui.Run(path);

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, "Nieuw project aanmaken...");
                Assert.IsNull(gui.ProjectFilePath);
                var expectedTitle = string.Format("{0} - {1} {2}",
                                                  expectedProjectName, fixedSettings.MainWindowTitle, SettingsHelper.ApplicationVersion);
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_WithPlugins_SetGuiAndActivatePlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.Deactivate());
            plugin.Stub(p => p.Dispose());
            plugin.Expect(p => p.Activate());
            plugin.Expect(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Expect(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                gui.Run();

                // Assert
                Assert.AreSame(gui, plugin.Gui);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_WithPluginThatThrowsExceptionWhenActivated_DeactivateAndDisposePlugin()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            plugin.Expect(p => p.Deactivate());
            plugin.Expect(p => p.Dispose());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                gui.Run();
            }
            // Assert
            mocks.VerifyAll(); // Expect calls on plugin
        }

        [Test]
        [STAThread]
        public void Run_WithPluginThatThrowsExceptionWhenActivatedAndDeactivated_LogErrorForDeactivatingThenDispose()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            plugin.Stub(p => p.Deactivate()).Throw(new Exception("MORE ERROR!"));
            plugin.Expect(p => p.Dispose());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                Action call = () => gui.Run();

                // Assert
                var expectedMessage = "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            }
            mocks.VerifyAll(); // Expect Dispose call on plugin
        }

        [Test]
        [STAThread]
        public void Run_InitializesViewController()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                // Call
                gui.Run();

                // Assert
                CollectionAssert.IsEmpty(gui.ViewHost.DocumentViews);
                Assert.IsNull(gui.ViewHost.ActiveDocumentView);

                Assert.AreEqual(2, gui.ViewHost.ToolViews.Count());
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is PropertyGridView));
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is MessageWindow));

                Assert.IsNotNull(gui.DocumentViewController);
                CollectionAssert.IsEmpty(gui.DocumentViewController.DefaultViewTypes);
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                var rootData = new object();

                // Call
                var dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData);

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

            var plugin1 = mocks.StrictMock<PluginBase>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData
            });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<PluginBase>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData
            });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin1);
                gui.Plugins.Add(plugin2);

                // Call
                var dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

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
            var plugin1 = mocks.StrictMock<PluginBase>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootData,
                rootChild
            });
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootChild)).Return(new[]
            {
                rootChild
            });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<PluginBase>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData)).Return(new[]
            {
                rootChild,
                rootData
            });
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootChild)).Return(new[]
            {
                rootChild
            });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin1);
                gui.Plugins.Add(plugin2);

                // Call
                var dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

                // Assert
                var expectedDataDefinitions = new[]
                {
                    rootData,
                    rootChild
                };
                CollectionAssert.AreEquivalent(expectedDataDefinitions, dataInstancesWithViewDefinitions);
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
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                // Call
                var result = gui.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(result);
            }

            mocks.VerifyAll();
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
                new TreeNodeInfo()
            };
            var nodesPluginC = new[]
            {
                new TreeNodeInfo(),
                new TreeNodeInfo(),
                new TreeNodeInfo()
            };

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var pluginA = mocks.Stub<PluginBase>();
            pluginA.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginA);
            pluginA.Stub(p => p.Dispose());
            pluginA.Stub(p => p.Deactivate());
            var pluginB = mocks.Stub<PluginBase>();
            pluginB.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginB);
            pluginB.Stub(p => p.Dispose());
            pluginB.Stub(p => p.Deactivate());
            var pluginC = mocks.Stub<PluginBase>();
            pluginC.Stub(p => p.GetTreeNodeInfos()).Return(nodesPluginC);
            pluginC.Stub(p => p.Dispose());
            pluginC.Stub(p => p.Deactivate());
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
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

        [Test]
        [STAThread]
        public void Get_GuiHasNotRunYet_ThrowsInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                // Call
                TestDelegate call = () => gui.Get(new object(), treeView);

                // Assert
                var message = Assert.Throws<InvalidOperationException>(call).Message;
                Assert.AreEqual("Call IGui.Run in order to initialize dependencies before getting the ContextMenuBuilder.", message);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Get_GuiIsRunning_ReturnsContextMenuBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                // Call
                IContextMenuBuilder builder = gui.Get(new object(), treeView);

                // Assert
                ContextMenuStrip contextMenu = builder.AddRenameItem()
                                                      .AddCollapseAllItem()
                                                      .AddDeleteItem()
                                                      .AddExpandAllItem()
                                                      .AddImportItem()
                                                      .AddExportItem()
                                                      .AddOpenItem()
                                                      .AddSeparator()
                                                      .AddPropertiesItem()
                                                      .Build();
                Assert.AreEqual(9, contextMenu.Items.Count);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Project_SetNewValue_FireProjectOpenedEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var oldProjectMock = mocks.Stub<IProject>();
            var newProjectMock = mocks.Stub<IProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), storeProject, projectFactory, new GuiCoreSettings()))
            {
                gui.Project = oldProjectMock;

                int openedCallCount = 0;
                gui.ProjectOpened += project =>
                {
                    Assert.AreSame(newProjectMock, project);
                    openedCallCount++;
                };

                // Call
                gui.Project = newProjectMock;

                // Assert
                Assert.AreEqual(1, openedCallCount);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithoutSelection_WhenSelectionProviderAdded_ThenSelectionSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                // Precondition
                Assert.IsNull(gui.Selection);

                // Call
                gui.ViewHost.AddDocumentView(selectionProvider);

                // Assert
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenSelectionProviderAdded_ThenSelectionSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.Selection = new object();

                // Call
                gui.ViewHost.AddDocumentView(selectionProvider);

                // Assert
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenNonSelectionProviderAdded_ThenSelectionPreserved()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selection = new object();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.Selection = selection;

                // Call
                gui.ViewHost.AddDocumentView(new TestView());

                // Assert
                Assert.AreSame(selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenSelectionChangedOnAddedSelectionProvider_ThenSelectionSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);

                gui.Selection = new object();

                // Call
                selectionProvider.ChangeSelection();

                // Assert
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenSelectionChangedOnRemovedSelectionProvider_ThenSelectionNoLongerSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selection = new object();
            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                gui.ViewHost.Remove(selectionProvider);

                gui.Selection = selection;

                // Call
                selectionProvider.ChangeSelection();

                // Assert
                Assert.AreSame(selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenSelectionProviderBecomesActiveView_ThenSelectionSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                gui.ViewHost.AddDocumentView(new TestView());

                gui.Selection = new object();

                // Call
                gui.ViewHost.SetFocusToView(selectionProvider);

                // Assert
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenNonSelectionProviderBecomesActiveView_ThenSelectionPreserved()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var testView = new TestView();
            var selection = new object();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(testView);

                gui.Selection = selection;

                // Call
                gui.ViewHost.SetFocusToView(testView);

                // Assert
                Assert.AreSame(selection, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void GivenGuiWithRandomSelection_WhenGuiDisposed_ThenSelectionNoLongerSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);

                gui.Dispose();

                // Precondition
                Assert.IsNull(gui.Selection);

                // Call
                selectionProvider.ChangeSelection();

                // Assert
                Assert.IsNull(gui.Selection);
            }
            mocks.VerifyAll();
        }

        private class TestSelectionProvider : Control, ISelectionProvider
        {
            private object selection;

            public event EventHandler<EventArgs> SelectionChanged;

            public TestSelectionProvider()
            {
                selection = new object();
            }

            public object Data { get; set; }

            public object Selection
            {
                get
                {
                    return selection;
                }
            }

            public void ChangeSelection()
            {
                selection = new object();

                if (SelectionChanged != null)
                {
                    SelectionChanged(this, new EventArgs());
                }
            }
        }

        private class TestView : Control, IView
        {
            public object Data { get; set; }
        }
    }
}