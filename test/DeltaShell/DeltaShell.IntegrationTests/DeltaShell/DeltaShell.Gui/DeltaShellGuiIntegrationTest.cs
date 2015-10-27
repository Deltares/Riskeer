using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections;
using DeltaShell.Core;
using DeltaShell.Gui;
using DeltaShell.Plugins.CommonTools;
using DeltaShell.Plugins.CommonTools.Gui;
using DeltaShell.Plugins.ProjectExplorer;
using DeltaShell.Plugins.SharpMapGis;
using DeltaShell.Plugins.SharpMapGis.Gui;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpTestsEx;
using Application = System.Windows.Forms.Application;
using Control = System.Windows.Controls.Control;

namespace DeltaShell.IntegrationTests.DeltaShell.DeltaShell.Gui
{
    [TestFixture]
    public class DeltaShellGuiIntegrationTest
    {
        private readonly MockRepository mocks = new MockRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(DeltaShellGuiIntegrationTest));

        [SetUp]
        public void SetUp()
        {
            LogHelper.ResetLogging();
        }

        [Test]
        public void DeleteProjectDataDirectoryShouldNotThrowExceptionOnNewProjectAndShouldNotHang()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new CommonToolsApplicationPlugin());
                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                gui.CommandHandler.TryCreateNewWTIProject();
                gui.CommandHandler.TryCreateNewWTIProject();
            }
        }

        [Test]
        [Ignore("potentially hangs")]
        public void RunManyActivitiesCancelCheckForThreadingIssuesTools9791()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;
                gui.Run();

                Action onShown = delegate
                {
                    var smallActivity = new TestActivity2();

                    for (int i = 0; i < 1000; i++)
                    {
                        app.RunActivityInBackground(smallActivity);

                        // cancel
                        while (!app.IsActivityRunning())
                        {
                            Thread.Sleep(1);
                        }
                        app.ActivityRunner.CancelAll();
                        while (app.IsActivityRunning())
                        {
                            Application.DoEvents();
                        }
                    }
                };
                WpfTestHelper.ShowModal((Control) gui.MainWindow, onShown);
            }
        }

        [Test]
        public void ProgressDialogIsModal()
        {
            if (!Environment.UserInteractive)
            {
                return; //progress dialog stuff isn't processed on non-interactive environment (see DeltaShellGui::UpdateProgressDialog)
            }

            if (GuiTestHelper.IsBuildServer)
            {
                return; // bleh (fails if users log in etc..)
            }

            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new CommonToolsApplicationPlugin());
                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                var mainWindow = (Control) gui.MainWindow;
                Action onShown = delegate
                {
                    var testActivity = new TestActivity();
                    gui.Application.ActivityRunner.Enqueue(testActivity);
                    try
                    {
                        while (!gui.Application.ActivityRunner.IsRunningActivity(testActivity))
                        {
                            Thread.Sleep(0);
                        }

                        Application.DoEvents();

                        Assert.IsFalse(mainWindow.IsEnabled);
                    }
                    finally
                    {
                        testActivity.Done = true;
                    }

                    while (gui.Application.ActivityRunner.IsRunningActivity(testActivity))
                    {
                        Thread.Sleep(0);
                    }

                    Application.DoEvents();

                    Assert.IsTrue(mainWindow.IsEnabled);
                };

                WpfTestHelper.ShowModal(mainWindow, onShown);
            }
        }

        [Test]
        public void ClosingEmptyProjectShouldNotGiveException()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;
                app.Plugins.Add(new CommonToolsApplicationPlugin());
                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                gui.CommandHandler.TryCloseWTIProject(); //should not trigger exception
            }
        }

        [Test]
        public void StartGuiWithToolboxDoesNotCrash()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new CommonToolsApplicationPlugin());
                app.Plugins.Add(new SharpMapGisApplicationPlugin());

                gui.Run();
            }
        }

        [Test]
        public void StartWithCommonPluginsShouldBeFast()
        {
            TestHelper.AssertIsFasterThan(7500, StartWithCommonPlugins);
        }

        [Test]
        public void CreateNewProjectAfterStartWithCommonPluginsShouldBeFast()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                app.Plugins.Add(new CommonToolsApplicationPlugin());
                gui.Plugins.Add(new SharpMapGisGuiPlugin());
                gui.Plugins.Add(new CommonToolsGuiPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                Action onShown = () => TestHelper.AssertIsFasterThan(300, gui.Application.CreateNewProject);

                WpfTestHelper.ShowModal((Control) gui.MainWindow, onShown);
            }
        }

        [Test]
        public void ExitShouldBeFast()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new CommonToolsApplicationPlugin());
                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                app.UserSettings["autosaveWindowLayout"] = false; // skip damagin of window layout

                gui.MainWindow.Show();

                for (int i = 0; i < 20; i++)
                {
                    var map = new Map();
                    app.Project.Items.Add(map);
                    gui.CommandHandler.OpenView(map);
                }
                app.SaveProjectAs(TestHelper.GetCurrentMethodName() + ".dsproj");

                TestHelper.AssertIsFasterThan(200, gui.Exit);
            }
        }

        [Test]
        public void GuiSelectionIsSetToProjectAfterStartWithProjectExplorer()
        {
            // initialize
            using (var gui = new DeltaShellGui())
            {
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                gui.Selection.Should().Be.EqualTo(gui.Application.Project);
            }
        }

        [Test]
        public void FormActionIsRunForMainWindow()
        {
            //testing testhelper + visible changed event of mainwindow.
            //could be tested separately but the combination is vital to many tests. That's why this test is here.
            using (var gui = new DeltaShellGui())
            {
                gui.Run();
                int callCount = 0;
                WpfTestHelper.ShowModal((Control) gui.MainWindow, () => callCount++);
                Assert.AreEqual(1, callCount);
            }
        }

        private class TestActivity2 : Activity
        {
            private bool shouldCancel;

            public override void Execute()
            {
                if (shouldCancel)
                {
                    Status = ActivityStatus.Cancelled;
                    shouldCancel = false;
                    return;
                }

                base.Execute();
            }

            public override void Cancel()
            {
                shouldCancel = true;
            }

            protected override void OnInitialize() {}

            protected override void OnExecute()
            {
                Thread.Sleep(100);
            }

            protected override void OnCancel()
            {
                throw new NotImplementedException();
            }

            protected override void OnCleanUp()
            {
                shouldCancel = false;
            }

            protected override void OnFinish() {}
        }

        private class TestActivity : Activity
        {
            public bool Done { get; set; }

            protected override void OnInitialize()
            {
                while (!Done)
                {
                    Thread.Sleep(0);
                }

                Status = ActivityStatus.Done;
            }

            protected override void OnExecute() {}

            protected override void OnCancel() {}

            protected override void OnCleanUp() {}

            protected override void OnFinish() {}
        }

        private static void StartWithCommonPlugins()
        {
            using (var gui = new DeltaShellGui())
            {
                var app = gui.Application;

                app.Plugins.Add(new SharpMapGisApplicationPlugin());
                app.Plugins.Add(new CommonToolsApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }
        }
    }
}