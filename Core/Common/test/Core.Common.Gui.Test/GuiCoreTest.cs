// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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
using Core.Common.Util.Settings;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class GuiCoreTest
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
        [Apartment(ApartmentState.STA)]
        public void ParameteredConstructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var project = mocks.Stub<IProject>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                // Assert
                Assert.AreEqual(null, gui.PropertyResolver);

                Assert.IsNull(gui.ProjectFilePath);
                Assert.AreSame(project, gui.Project);

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
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_MainWindowNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            // Call
            TestDelegate call = () => new GuiCore(null, projectStore, projectMigrator, projectFactory, guiCoreSettings);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mainWindow", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ProjectMigratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            {
                // Call
                TestDelegate call = () => new GuiCore(mainWindow, projectStore, null, projectFactory, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(call);
                Assert.AreEqual("projectMigrator", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ProjectStoreNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            {
                // Call
                TestDelegate call = () => new GuiCore(mainWindow, null, projectMigrator, projectFactory, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(call);
                Assert.AreEqual("projectStore", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ProjectFactoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            {
                // Call
                TestDelegate call = () => new GuiCore(mainWindow, projectStore, projectMigrator, null, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(call);
                Assert.AreEqual("projectFactory", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_FixedSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                // Call
                TestDelegate call = () => new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(call);
                Assert.AreEqual("fixedSettings", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ConstructedAfterAnotherInstanceHasBeenCreated_ThrowsInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            using (new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                // Call
                TestDelegate call = () =>
                {
                    using (new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings)) {}
                };

                // Assert
                Assert.Throws<InvalidOperationException>(call);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void SetProject_SetNull_ThrowsArgumentNullException()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Expect(p => p.Deactivate());
            plugin.Expect(p => p.Dispose());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                TestDelegate test = () => gui.SetProject(null, null);

                // Assert
                Assert.Throws<ArgumentNullException>(test);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_PluginsAdded_PluginsDisabledAndRemovedAndDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Expect(p => p.Deactivate());
            plugin.Expect(p => p.Dispose());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
            gui.Plugins.Add(plugin);

            // Call
            gui.Dispose();

            // Assert
            Assert.IsNull(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_PluginAddedButThrowsExceptionDuringDeactivation_LogErrorAndStillDisposeAndRemove()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Expect(p => p.Deactivate()).Throw(new Exception("Bad stuff happening!"));
            plugin.Expect(p => p.Dispose());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
            gui.Plugins.Add(plugin);

            // Call
            Action call = () => gui.Dispose();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.", 1);
            Assert.IsNull(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_HasSelection_ClearSelection()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings())
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
        [Apartment(ApartmentState.STA)]
        public void Dispose_HasMainWindow_DiposeOfMainWindow()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings());

                // Call
                gui.Dispose();

                // Assert
                Assert.IsTrue(mainWindow.IsWindowDisposed);
                Assert.IsNull(gui.MainWindow);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_HasInitializedMessageWindowForLogAppender_ClearMessageWindow()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var messageWindowLogAppender = new MessageWindowLogAppender();

            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            rootLogger.AddAppender(messageWindowLogAppender);

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
        [Apartment(ApartmentState.STA)]
        public void Dispose_HasOpenedToolView_ToolViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var toolView = new TestView())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.ViewHost.AddToolView(toolView, ToolViewLocation.Left);

                // Call
                gui.Dispose();

                // Assert
                CollectionAssert.IsEmpty(gui.ViewHost.ToolViews);
                Assert.IsTrue(toolView.IsDisposed);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_HasOpenedDocumentView_DocumentViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var documentView = new TestView())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                gui.ViewHost.AddDocumentView(documentView);

                // Call
                gui.Dispose();

                // Assert
                CollectionAssert.IsEmpty(gui.ViewHost.DocumentViews);
                Assert.IsNull(gui.DocumentViewController);
                Assert.IsTrue(documentView.IsDisposed);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_NoMessageWindowLogAppender_AddNewLogAppender()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
                foreach (IAppender appender in originalAppenders)
                {
                    rootLogger.AddAppender(appender);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_AlreadyHasMessageWindowLogAppender_NoChangesToLogAppenders()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender();

            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            IAppender[] originalAppenders = rootLogger.Appenders.ToArray();
            rootLogger.RemoveAllAppenders();
            rootLogger.AddAppender(appender);
            bool rootloggerConfigured = rootLogger.Repository.Configured;

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
                foreach (IAppender originalAppender in originalAppenders)
                {
                    rootLogger.AddAppender(originalAppender);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_WithFile_LoadProjectFromFile()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = $"{fileName}.rtd";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(testFile)).Return(MigrationRequired.No);
            var deserializedProject = mocks.Stub<IProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile)).Return(deserializedProject);
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages);
                Assert.AreEqual(testFile, gui.ProjectFilePath);
                Assert.AreSame(deserializedProject, gui.Project);
                Assert.AreEqual(fileName, gui.Project.Name,
                                "Project name should be updated to the name of the file.");

                string expectedTitle = $"{fileName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_LoadingFromOutdatedFileAndMigrationCancelled_LoadDefaultProjectInstead()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = $"{fileName}.rtd";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile)).Return(MigrationRequired.Yes);
            projectMigrator.Stub(pm => pm.DetermineMigrationLocation(testFile)).Return(null);

            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(ph => ph.CreateNewProject()).Return(project);

            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                gui.Run(testFile);

                // Assert
                Assert.IsNull(gui.ProjectFilePath);
                string expectedTitle = $"{expectedProjectName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_LoadingFromOutdatedAndShouldMigrateThrowsArgumentException_LogErrorAndLoadDefaultProjectInstead()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = $"{fileName}.rtd";

            const string expectedErrorMessage = "You shall not migrate!";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile))
                           .Throw(new ArgumentException(expectedErrorMessage));

            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(ph => ph.CreateNewProject()).Return(project);

            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedErrorMessage, LogLevelConstant.Error));

                Assert.IsNull(gui.ProjectFilePath);
                string expectedTitle = $"{expectedProjectName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_LoadingFromOutdatedAndMigrateThrowsArgumentException_LogErrorAndLoadDefaultProjectInstead()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = $"{fileName}.rtd";
            string targetFile = $"{fileName}_17_1.rtd";

            const string expectedErrorMessage = "You shall not migrate!";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile)).Return(MigrationRequired.Yes);
            projectMigrator.Stub(pm => pm.DetermineMigrationLocation(testFile)).Return(targetFile);
            projectMigrator.Stub(pm => pm.Migrate(testFile, targetFile))
                           .Throw(new ArgumentException(expectedErrorMessage));

            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(ph => ph.CreateNewProject()).Return(project);

            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(expectedErrorMessage, LogLevelConstant.Error),
                    Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages);

                Assert.IsNull(gui.ProjectFilePath);
                string expectedTitle = $"{expectedProjectName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_LoadingFromFileThrowsStorageException_LogErrorAndLoadDefaultProjectInstead()
        {
            // Setup
            const string fileName = "SomeFile";
            string testFile = $"{fileName}.rtd";

            const string storageExceptionText = "<Some error preventing the project from being opened>";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(testFile)).Return(MigrationRequired.No);
            projectStore.Expect(ps => ps.LoadProject(testFile)).Throw(new StorageException(storageExceptionText));
            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(ph => ph.CreateNewProject()).Return(project);

            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<main window title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                Action call = () => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(storageExceptionText, LogLevelConstant.Error),
                    Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages);

                Assert.IsNull(gui.ProjectFilePath);
                string expectedTitle = $"{expectedProjectName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        [Apartment(ApartmentState.STA)]
        public void Run_WithoutFile_DefaultProjectStillSet(string path)
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.StrictMock<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();

            const string expectedProjectName = "Project";
            var project = mocks.Stub<IProject>();
            project.Name = expectedProjectName;
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(ph => ph.CreateNewProject()).Return(project);
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings
            {
                MainWindowTitle = "<title part>"
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                // Call
                gui.Run(path);

                // Assert
                Assert.IsNull(gui.ProjectFilePath);
                Assert.AreSame(project, gui.Project);
                string expectedTitle = $"{expectedProjectName} - {fixedSettings.MainWindowTitle} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_WithPlugins_SetGuiAndActivatePlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.Deactivate());
            plugin.Stub(p => p.Dispose());
            plugin.Expect(p => p.Activate());
            plugin.Expect(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Expect(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
        [Apartment(ApartmentState.STA)]
        public void Run_WithPluginThatThrowsExceptionWhenActivated_DeactivateAndDisposePlugin()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            plugin.Expect(p => p.Deactivate());
            plugin.Expect(p => p.Dispose());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                gui.Run();
            }

            // Assert
            mocks.VerifyAll(); // Expect calls on plugin
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_WithPluginThatThrowsExceptionWhenActivatedAndDeactivated_LogErrorForDeactivatingThenDispose()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.Activate()).Throw(new Exception("ERROR!"));
            plugin.Stub(p => p.Deactivate()).Throw(new Exception("MORE ERROR!"));
            plugin.Expect(p => p.Dispose());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin);

                // Call
                Action call = () => gui.Run();

                // Assert
                const string expectedMessage = "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.";
                Tuple<string, LogLevelConstant> expectedMessageAndLogLevel = Tuple.Create(expectedMessage, LogLevelConstant.Error);
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessageAndLogLevel);
            }

            mocks.VerifyAll(); // Expect Dispose call on plugin
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_InitializesViewController()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetAllDataWithViewDefinitionsRecursively_DataHasNoViewDefinitions_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var rootData = new object();

                // Call
                IEnumerable dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData);

                // Assert
                CollectionAssert.IsEmpty(dataInstancesWithViewDefinitions);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRoot_ReturnRootObject()
        {
            // Setup
            var rootData = new object();

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();

            var plugin1 = mocks.StrictMock<PluginBase>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData))
                   .Return(new[]
                   {
                       rootData
                   });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<PluginBase>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData))
                   .Return(new[]
                   {
                       rootData
                   });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin1);
                gui.Plugins.Add(plugin2);

                // Call
                object[] dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

                // Assert
                object[] expectedDataDefinitions =
                {
                    rootData
                };
                CollectionAssert.AreEquivalent(expectedDataDefinitions, dataInstancesWithViewDefinitions);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRootAndChild_ReturnRootAndChild()
        {
            // Setup
            object rootData = 1;
            object rootChild = 2;

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin1 = mocks.StrictMock<PluginBase>();
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootData))
                   .Return(new[]
                   {
                       rootData,
                       rootChild
                   });
            plugin1.Expect(p => p.GetChildDataWithViewDefinitions(rootChild))
                   .Return(new[]
                   {
                       rootChild
                   });
            plugin1.Stub(p => p.Dispose());
            plugin1.Stub(p => p.Deactivate());
            var plugin2 = mocks.StrictMock<PluginBase>();
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootData))
                   .Return(new[]
                   {
                       rootChild,
                       rootData
                   });
            plugin2.Expect(p => p.GetChildDataWithViewDefinitions(rootChild))
                   .Return(new[]
                   {
                       rootChild
                   });
            plugin2.Stub(p => p.Dispose());
            plugin2.Stub(p => p.Deactivate());
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(plugin1);
                gui.Plugins.Add(plugin2);

                // Call
                object[] dataInstancesWithViewDefinitions = gui.GetAllDataWithViewDefinitionsRecursively(rootData).OfType<object>().ToArray();

                // Assert
                object[] expectedDataDefinitions =
                {
                    rootData,
                    rootChild
                };
                CollectionAssert.AreEquivalent(expectedDataDefinitions, dataInstancesWithViewDefinitions);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetTreeNodeInfos_NoPluginsConfigured_EmptyList()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                // Call
                IEnumerable<TreeNodeInfo> result = gui.GetTreeNodeInfos();

                // Assert
                CollectionAssert.IsEmpty(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
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
            var projectMigrator = mocks.Stub<IMigrateProject>();

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
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(pluginA);
                gui.Plugins.Add(pluginB);
                gui.Plugins.Add(pluginC);

                // Call
                IEnumerable<TreeNodeInfo> result = gui.GetTreeNodeInfos();

                // Assert
                IEnumerable<TreeNodeInfo> expected = nodesPluginA.Concat(nodesPluginB).Concat(nodesPluginC);
                CollectionAssert.AreEquivalent(expected, result);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Get_GuiHasNotRunYet_ThrowsInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                // Call
                TestDelegate call = () => gui.Get(new object(), treeView);

                // Assert
                string message = Assert.Throws<InvalidOperationException>(call).Message;
                Assert.AreEqual("Call IGui.Run in order to initialize dependencies before getting the ContextMenuBuilder.", message);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Get_GuiIsRunning_ReturnsContextMenuBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var treeView = new TreeViewControl())
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
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
        [Apartment(ApartmentState.STA)]
        public void SetProject_SetNewValue_FireProjectOpenedEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var oldProject = mocks.Stub<IProject>();
            var newProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(oldProject);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), storeProject, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var openedCallCount = 0;
                var beforeOpenCallCount = 0;
                gui.BeforeProjectOpened += project =>
                {
                    Assert.AreSame(oldProject, project);
                    beforeOpenCallCount++;
                };
                gui.ProjectOpened += project =>
                {
                    Assert.AreSame(newProject, project);
                    openedCallCount++;
                };

                // Call
                gui.SetProject(newProject, null);

                // Assert
                Assert.AreEqual(1, openedCallCount);
                Assert.AreEqual(1, beforeOpenCallCount);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithoutSelection_WhenSelectionProviderSetAsActiveView_ThenSelectionSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);

                // Precondition
                Assert.IsNull(gui.Selection);

                // When
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                // Then
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenSelectionProviderSetAsActiveView_ThenSelectionSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);

                gui.Selection = new object();

                // When
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                // Then
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenSelectionChangedOnActiveSelectionProvider_ThenSelectionSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                gui.Selection = new object();

                // When
                selectionProvider.ChangeSelection();

                // Then
                Assert.AreSame(selectionProvider.Selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenSelectionChangedOnRemovedSelectionProvider_ThenSelectionNoLongerSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selection = new object();
            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                gui.ViewHost.Remove(selectionProvider);

                gui.Selection = selection;

                // When
                selectionProvider.ChangeSelection();

                // Then
                Assert.AreSame(selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenNonSelectionProviderSetAsActiveView_ThenSelectionPreserved()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var testView = new TestView();
            var selection = new object();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(testView);

                gui.Selection = selection;

                // When
                SetActiveView((AvalonDockViewHost) gui.ViewHost, testView);

                // Then
                Assert.AreSame(selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenSelectionProviderRemoved_ThenSelectionPreserved()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var testView = new TestView();
            var selection = new object();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(testView);

                gui.Selection = selection;

                // When
                gui.ViewHost.Remove(testView);

                // Then
                Assert.AreSame(selection, gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithSelectionFromSelectionProvider_WhenSelectionProviderRemoved_ThenSelectionCleared()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                // Precondition
                Assert.AreSame(selectionProvider.Selection, gui.Selection);

                // When
                gui.ViewHost.Remove(selectionProvider);

                // Then
                Assert.IsNull(gui.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenGuiWithRandomSelection_WhenGuiDisposed_ThenSelectionNoLongerSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                gui.ViewHost.AddDocumentView(selectionProvider);
                SetActiveView((AvalonDockViewHost) gui.ViewHost, selectionProvider);

                gui.Dispose();

                // Precondition
                Assert.IsNull(gui.Selection);

                // When
                selectionProvider.ChangeSelection();

                // Then
                Assert.IsNull(gui.Selection);
            }

            mocks.VerifyAll();
        }

        private static void SetActiveView(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            avalonDockViewHost.DockingManager.Layout.Descendents()
                              .OfType<LayoutContent>()
                              .First(d => ((WindowsFormsHost) d.Content).Child == view)
                              .IsActive = true;
        }

        private static IProjectFactory CreateProjectFactory(MockRepository mocks)
        {
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());

            return projectFactory;
        }

        private class TestSelectionProvider : Control, ISelectionProvider, IView
        {
            public event EventHandler<EventArgs> SelectionChanged;

            public TestSelectionProvider()
            {
                Selection = new object();
            }

            public object Selection { get; private set; }

            public object Data { get; set; }

            public void ChangeSelection()
            {
                Selection = new object();

                SelectionChanged?.Invoke(this, new EventArgs());
            }
        }

        private class TestView : Control, IView
        {
            public object Data { get; set; }
        }
    }
}