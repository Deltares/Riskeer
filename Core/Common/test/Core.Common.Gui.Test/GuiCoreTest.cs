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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.PropertyGridView;
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
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, applicationCore, guiCoreSettings))
            {
                // Assert
                Assert.AreSame(applicationCore, gui.ApplicationCore);
                Assert.AreEqual(null, gui.PropertyResolver);
                Assert.AreSame(projectStore, gui.Storage);

                Assert.AreEqual(null, gui.ProjectFilePath);
                Assert.AreEqual(null, gui.Project);

                Assert.AreEqual(null, gui.Selection);

                Assert.IsInstanceOf<ProjectCommandHandler>(gui.ProjectCommands);
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
                TestDelegate call = () => new GuiCore(nullArgumentIndex == 0 ? null : mainWindow, projectStore, applicationCore, guiCoreSettings);

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
            using (var gui = new GuiCore(mainWindow, projectStore, applicationCore, guiCoreSettings))
            {
                // Call
                using (var gui2 = new GuiCore(mainWindow, projectStore, applicationCore, guiCoreSettings))
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

            var gui = new GuiCore(new MainWindow(), projectStore, applicationCore, new GuiCoreSettings());

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
            var guiPluginMock = mocks.Stub<PluginBase>();
            guiPluginMock.Expect(p => p.Deactivate());
            guiPluginMock.Expect(p => p.Dispose());
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings());
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
            var guiPluginMock = mocks.Stub<PluginBase>();
            guiPluginMock.Expect(p => p.Deactivate()).Throw(new Exception("Bad stuff happening!"));
            guiPluginMock.Expect(p => p.Dispose());
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings());
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

            var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings())
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

            var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings())
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
                var gui = new GuiCore(mainWindow, projectStore, new ApplicationCore(), new GuiCoreSettings());

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
                using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
                using(var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender();

            Logger rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();
            rootLogger.AddAppender(appender);
            var rootloggerConfigured = rootLogger.Repository.Configured;

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            using (var gui = new GuiCore(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
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
            using (var gui = new GuiCore(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
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
            using (var gui = new GuiCore(mainWindow, projectStore, new ApplicationCore(), fixedSettings))
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
        public void Run_WithGuiPlugins_SetGuiAndActivatePlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var guiPlugin = mocks.Stub<PluginBase>();
            guiPlugin.Stub(p => p.Deactivate());
            guiPlugin.Stub(p => p.Dispose());
            guiPlugin.Expect(p => p.Activate());
            guiPlugin.Expect(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            guiPlugin.Expect(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Plugins.Add(guiPlugin);

                // Call
                gui.Run();

                // Assert
                Assert.AreSame(gui, guiPlugin.Gui);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_WithGuiPluginThatThrowsExceptionWhenActivated_DeactivateAndDisposePlugin()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var guiPlugin = mocks.Stub<PluginBase>();
            guiPlugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            guiPlugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            guiPlugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            guiPlugin.Expect(p => p.Deactivate());
            guiPlugin.Expect(p => p.Dispose());
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Plugins.Add(guiPlugin);

                // Call
                gui.Run();
            }
            // Assert
            mocks.VerifyAll(); // Expect calls on plugin
        }

        [Test]
        [STAThread]
        public void Run_WithGuiPluginThatThrowsExceptionWhenActivatedAndDeactivated_LogErrorForDeactivatingThenDispose()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var guiPlugin = mocks.Stub<PluginBase>();
            guiPlugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            guiPlugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            guiPlugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            guiPlugin.Stub(p => p.Deactivate()).Throw(new Exception("MORE ERROR!"));
            guiPlugin.Expect(p => p.Dispose());
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Plugins.Add(guiPlugin);

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
        public void Run_InitializesDocumentViewController()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                // Call
                Action call = () => gui.Run();

                // Assert
                var expectedMessage = "Schermmanager voor documenten aan het maken...";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);

                CollectionAssert.IsEmpty(gui.DocumentViews);
                Assert.IsFalse(gui.DocumentViews.IgnoreActivation);
                Assert.IsNull(gui.DocumentViews.ActiveView);
                
                Assert.IsNotNull(gui.DocumentViewsResolver);
                CollectionAssert.IsEmpty(gui.DocumentViewsResolver.DefaultViewTypes);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Run_InitializesToolViewController()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                // Call
                gui.Run();

                // Assert
                Assert.AreEqual(2, gui.ToolWindowViews.Count);
                Assert.AreEqual(1, gui.ToolWindowViews.Count(v => v is PropertyGridView));
                Assert.AreEqual(1, gui.ToolWindowViews.Count(v => v is MessageWindow));
                Assert.IsFalse(gui.ToolWindowViews.IgnoreActivation);
                Assert.IsNull(gui.ToolWindowViews.ActiveView);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
                rootData, rootChild
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
                rootChild, rootData
            });
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootChild)).Return(new[]
            {
                rootChild
            });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin1);
                gui.Plugins.Add(plugin2);

                // Call
                var dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

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

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
        public void Get_GuiHasntRunYet_ThrowInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
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
        public void GivenGuiRunCalled_WhenMainWindowOpens_EnsurePropertyGridAndMessageWindowAreActivated()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                gui.Run();

                var projectExplorerMock = new ProjectExplorerMock();
                gui.ToolWindowViews.Add(projectExplorerMock);

                var activatedViewsDuringShow = new List<IView>();
                gui.ToolWindowViews.ActiveViewChanged += (sender, args) =>
                {
                    activatedViewsDuringShow.Add(args.View);
                };

                // Call
                mainWindow.Show();

                // Assert
                Assert.AreEqual(3, activatedViewsDuringShow.Count);
                Assert.AreEqual(1, activatedViewsDuringShow.Count(v => v is MessageWindow));
                Assert.AreEqual(1, activatedViewsDuringShow.Count(v => v is PropertyGrid));
                Assert.AreEqual(1, activatedViewsDuringShow.Count(v => ReferenceEquals(v, projectExplorerMock)));
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Project_SetNewValue_FireProjectClosingAndOpenedEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), storeProject, new ApplicationCore(), new GuiCoreSettings()))
            {
                var oldProject = new Project("A");
                var newProject = new Project("B");

                gui.Project = oldProject;

                int closingCallCount = 0;
                gui.ProjectClosing += project =>
                {
                    if (closingCallCount == 0)
                    {
                        Assert.AreSame(oldProject, project);
                    }
                    else
                    {
                        // Dispose causes this one:
                        Assert.AreSame(newProject, project);
                    }
                    closingCallCount++;
                };
                int openedCallCount = 0;
                gui.ProjectOpened += project =>
                {
                    Assert.AreSame(newProject, project);
                    openedCallCount++;
                };

                // Call
                gui.Project = newProject;

                // Assert
                Assert.AreEqual(1, closingCallCount);
                Assert.AreEqual(1, openedCallCount);
            }
            mocks.VerifyAll();
        }

        private static void AssertDefaultUserSettings(SettingsBase settings)
        {
            Assert.IsNotNull(settings);
            Assert.AreEqual(15, settings.Properties.Count);

            // Note: Cannot assert particular values, as they can be changed by user.
            var mruList = (StringCollection)settings["mruList"];
            Assert.IsNotNull(mruList);
            var defaultViewDataTypes = (StringCollection)settings["defaultViewDataTypes"];
            Assert.IsNotNull(defaultViewDataTypes);
            var defaultViews = (StringCollection)settings["defaultViews"];
            Assert.IsNotNull(defaultViews);
            var lastVisitedPath = (string)settings["lastVisitedPath"];
            Assert.IsNotNull(lastVisitedPath);
            var isMainWindowFullScreen = (bool)settings["MainWindow_FullScreen"];
            Assert.IsNotNull(isMainWindowFullScreen);
            var x = (int)settings["MainWindow_X"];
            Assert.IsNotNull(x);
            var y = (int)settings["MainWindow_Y"];
            Assert.IsNotNull(y);
            var width = (int)settings["MainWindow_Width"];
            Assert.IsNotNull(width);
            var height = (int)settings["MainWindow_Height"];
            Assert.IsNotNull(height);
            var startPageName = (string)settings["startPageName"];
            Assert.IsNotNull(startPageName);
            var showStartPage = (bool)settings["showStartPage"];
            Assert.IsNotNull(showStartPage);
            var showSplashScreen = (bool)settings["showSplashScreen"];
            Assert.IsNotNull(showSplashScreen);
            var showHiddenDataItems = (bool)settings["showHiddenDataItems"];
            Assert.IsNotNull(showHiddenDataItems);
            var colorTheme = (ColorTheme)settings["colorTheme"];
            Assert.IsNotNull(colorTheme);
        }

        private class ProjectExplorerMock : UserControl, IProjectExplorer
        {
            public object Data { get; set; }
            public TreeViewControl TreeViewControl { get; private set; }
        }
    }
}