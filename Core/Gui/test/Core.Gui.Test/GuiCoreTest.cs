// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Media;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Settings;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.MainWindow;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.MessageWindow;
using Core.Gui.Forms.ProjectExplorer;
using Core.Gui.Forms.PropertyGridView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.Settings;
using Core.Gui.TestUtil;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock.Layout;
using CoreGuiTestUtilResources = Core.Gui.TestUtil.Properties.Resources;

namespace Core.Gui.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
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
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings();

            // Call
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                // Assert
                Assert.AreEqual(null, gui.PropertyResolver);

                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);

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

                Assert.AreSame(projectStore, gui.ProjectStore);
            }

            mocks.VerifyAll();
        }

        [Test]
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
            void Call() => new GuiCore(null, projectStore, projectMigrator, projectFactory, guiCoreSettings);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mainWindow", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
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
                void Call() => new GuiCore(mainWindow, null, projectMigrator, projectFactory, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("projectStore", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                void Call() => new GuiCore(mainWindow, projectStore, null, projectFactory, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("projectMigrator", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                void Call() => new GuiCore(mainWindow, projectStore, projectMigrator, null, guiCoreSettings);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("projectFactory", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                void Call() => new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("fixedSettings", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                void Call()
                {
                    using (new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings)) {}
                }

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                void Call() => gui.SetProject(null, null);

                // Assert
                Assert.Throws<ArgumentNullException>(Call);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_WithPlugin_PluginRemoved()
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
            CollectionAssert.IsEmpty(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_WithPluginThatThrowsExceptionDuringDeactivation_LogsErrorAndPluginRemoved()
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
            void Call() => gui.Dispose();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.", 1);
            CollectionAssert.IsEmpty(gui.Plugins);
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_HasSelection_SelectionCleared()
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
        public void Dispose_HasMainWindow_MainWindowDisposed()
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
        public void Dispose_HasInitializedMessageWindowForLogAppender_MessageWindowCleared()
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
                var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                // Precondition
                Assert.IsNotNull(MessageWindowLogAppender.Instance.MessageWindow);
                Assert.IsNotNull(messageWindowLogAppender.MessageWindow);

                // Call
                gui.Dispose();

                // Assert
                Assert.IsNull(MessageWindowLogAppender.Instance.MessageWindow);
                Assert.IsNull(messageWindowLogAppender.MessageWindow);
                CollectionAssert.DoesNotContain(rootLogger.Appenders, messageWindowLogAppender);
            }
            finally
            {
                rootLogger.RemoveAppender(messageWindowLogAppender);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_HasOpenedToolView_ToolViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var toolView = new TestView())
            {
                var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                gui.ViewHost.AddToolView(toolView, ToolViewLocation.Left, string.Empty);

                // Call
                gui.Dispose();

                // Assert
                CollectionAssert.IsEmpty(gui.ViewHost.ToolViews);
                Assert.IsTrue(toolView.IsDisposed);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_HasOpenedDocumentView_DocumentViewsClearedAndViewsDisposed()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var documentView = new TestView())
            {
                var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
                gui.Plugins.Add(new TestPlugin());
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
        public void Run_NoMessageWindowLogAppender_AddsNewLogAppender()
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
                    gui.Plugins.Add(new TestPlugin());

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
            bool rootLoggerConfigured = rootLogger.Repository.Configured;

            try
            {
                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
                {
                    gui.Plugins.Add(new TestPlugin());

                    // Call
                    gui.Run();

                    // Assert
                    Assert.AreEqual(1, rootLogger.Appenders.Count);
                    Assert.AreSame(appender, rootLogger.Appenders[0]);
                    Assert.AreSame(appender, MessageWindowLogAppender.Instance);
                    Assert.AreEqual(rootLoggerConfigured, rootLogger.Repository.Configured);

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
        public void Run_WithFile_LoadProjectFromFile()
        {
            // Setup
            const string fileName = "SomeFile";
            var testFile = $"{fileName}.rtd";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(testFile)).Return(MigrationRequired.No);
            var project = mocks.Stub<IProject>();
            projectStore.Expect(ps => ps.LoadProject(testFile)).Return(project);
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

            project.Name = fileName;

            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationName = "<main window title part>",
                ApplicationIcon = CoreGuiTestUtilResources.TestIcon
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                gui.Plugins.Add(new TestPlugin(new[]
                {
                    new StateInfo("Name", "Symbol", new FontFamily(), p => p)
                }));

                // Call
                void Call() => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages);
                Assert.AreEqual(testFile, gui.ProjectFilePath);
                Assert.AreSame(project, gui.Project);
                Assert.AreEqual(fileName, gui.Project.Name);

                var expectedTitle = $"{fileName} - {guiCoreSettings.ApplicationName} {SettingsHelper.Instance.ApplicationVersion}";
                Assert.AreEqual(expectedTitle, mainWindow.Title);
                Assert.AreSame(gui.Project, mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_LoadingFromOutdatedFileAndMigrationCancelled_NoProjectSet()
        {
            // Setup
            const string fileName = "SomeFile";
            var testFile = $"{fileName}.rtd";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile)).Return(MigrationRequired.Yes);
            projectMigrator.Stub(pm => pm.DetermineMigrationLocation(testFile)).Return(null);
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                gui.Run(testFile);

                // Assert
                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_LoadingFromOutdatedFileAndShouldMigrateThrowsArgumentException_LogsError()
        {
            // Setup
            const string fileName = "SomeFile";
            var testFile = $"{fileName}.rtd";

            const string expectedErrorMessage = "You shall not migrate!";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile))
                           .Throw(new ArgumentException(expectedErrorMessage));
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                void Call() => gui.Run(testFile);

                // Assert
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedErrorMessage, LogLevelConstant.Error));

                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_LoadingFromOutdatedFileAndMigrateThrowsArgumentException_LogsError()
        {
            // Setup
            const string fileName = "SomeFile";
            var testFile = $"{fileName}.rtd";
            var targetFile = $"{fileName}_17_1.rtd";

            const string expectedErrorMessage = "You shall not migrate!";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(pm => pm.ShouldMigrate(testFile)).Return(MigrationRequired.Yes);
            projectMigrator.Stub(pm => pm.DetermineMigrationLocation(testFile)).Return(targetFile);
            projectMigrator.Stub(pm => pm.Migrate(testFile, targetFile))
                           .Throw(new ArgumentException(expectedErrorMessage));
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = CoreGuiTestUtilResources.TestIcon
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                void Call() => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(expectedErrorMessage, LogLevelConstant.Error),
                    Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages);

                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_LoadingFromFileThrowsStorageException_LogsError()
        {
            // Setup
            const string fileName = "SomeFile";
            var testFile = $"{fileName}.rtd";

            const string storageExceptionText = "<Some error preventing the project from being opened>";

            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(testFile)).Return(MigrationRequired.No);
            projectStore.Expect(ps => ps.LoadProject(testFile)).Throw(new StorageException(storageExceptionText));
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = CoreGuiTestUtilResources.TestIcon
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                void Call() => gui.Run(testFile);

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(storageExceptionText, LogLevelConstant.Error),
                    Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages);

                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void Run_WithoutFile_NoProjectSet(string path)
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.StrictMock<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var fixedSettings = new GuiCoreSettings();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, fixedSettings))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                gui.Run(path);

                // Assert
                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_WithPlugins_SetGuiAndActivatePlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            // Setup
            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new TestPlugin();
                gui.Plugins.Add(plugin);

                // Call
                gui.Run();

                // Assert
                Assert.AreSame(gui, plugin.Gui);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Run_WithPluginThatThrowsExceptionWhenActivated_PluginDeactivatedAndDisposed()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetStateInfos()).Return(Enumerable.Empty<StateInfo>());
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.GetTreeNodeInfos()).Return(new TreeNodeInfo[]
            {
                new TreeNodeInfo<IProject>()
            });
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
        public void Run_WithPluginThatThrowsExceptionWhenActivatedAndDeactivated_LogsErrorForDeactivatingThenDisposed()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var plugin = mocks.Stub<PluginBase>();
            plugin.Stub(p => p.GetStateInfos()).Return(Enumerable.Empty<StateInfo>());
            plugin.Stub(p => p.GetViewInfos()).Return(Enumerable.Empty<ViewInfo>());
            plugin.Stub(p => p.GetPropertyInfos()).Return(Enumerable.Empty<PropertyInfo>());
            plugin.Stub(p => p.GetTreeNodeInfos()).Return(new TreeNodeInfo[]
            {
                new TreeNodeInfo<IProject>()
            });
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
                void Call() => gui.Run();

                // Assert
                const string expectedMessage = "Kritieke fout opgetreden tijdens deactivering van de grafische interface plugin.";
                Tuple<string, LogLevelConstant> expectedMessageAndLogLevel = Tuple.Create(expectedMessage, LogLevelConstant.Error);
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, expectedMessageAndLogLevel);
            }

            mocks.VerifyAll(); // Expect Dispose call on plugin
        }

        [Test]
        public void Run_InitializesViewHost()
        {
            // Setup
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());

                // Call
                gui.Run();

                // Assert
                CollectionAssert.IsEmpty(gui.ViewHost.DocumentViews);
                Assert.IsNull(gui.ViewHost.ActiveDocumentView);

                Assert.AreEqual(5, gui.ViewHost.ToolViews.Count());
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is ProjectExplorer));
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is PropertyGridView));
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is MessageWindow));
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is MapLegendView));
                Assert.AreEqual(1, gui.ViewHost.ToolViews.Count(v => v is ChartLegendView));

                Assert.IsNotNull(gui.DocumentViewController);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetAllDataWithViewDefinitionsRecursively_DataHasNoViewDefinitions_ReturnsEmptyCollection()
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
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRoot_ReturnsRoot()
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
        public void GetAllDataWithViewDefinitionsRecursively_MultiplePluginsHaveViewDefinitionsForRootAndChild_ReturnsRootAndChild()
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
        public void GetTreeNodeInfos_NoPluginsConfigured_ReturnsEmptyCollection()
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
                void Call() => gui.Get(new object(), treeView);

                // Assert
                string message = Assert.Throws<InvalidOperationException>(Call).Message;
                Assert.AreEqual("Call IGui.Run in order to initialize dependencies before getting the ContextMenuBuilder.", message);
            }

            mocks.VerifyAll();
        }

        [Test]
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
                gui.Plugins.Add(new TestPlugin());
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
        public void SetProject_SetNewValue_FiresProjectOpenedEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var newProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), storeProject, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var openedCallCount = 0;
                var beforeOpenCallCount = 0;
                gui.BeforeProjectOpened += project =>
                {
                    Assert.IsNull(project);
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
                gui.Plugins.Add(new TestPlugin());
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
        public void GivenGuiWithRandomSelection_WhenGuiDisposed_ThenSelectionNoLongerSynced()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            IProjectFactory projectFactory = CreateProjectFactory(mocks);
            mocks.ReplayAll();

            var selectionProvider = new TestSelectionProvider();

            var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings());
            gui.Plugins.Add(new TestPlugin());
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
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());

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