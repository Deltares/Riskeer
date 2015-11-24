using System;
using System.Linq;
using System.Threading;
using Core.Common.Base.Workflow;
using Core.Common.Gui;
using Core.Common.TestUtils;
using Core.GIS.SharpMap.Map;
using Core.Plugins.CommonTools;
using Core.Plugins.CommonTools.Gui;
using Core.Plugins.ProjectExplorer;
using Core.Plugins.SharpMapGis;
using Core.Plugins.SharpMapGis.Gui;
using NUnit.Framework;
using Control = System.Windows.Controls.Control;

namespace Core.Common.Integration.Tests.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class RingtoetsGuiIntegrationTest
    {
        [SetUp]
        public void SetUp()
        {
            LogHelper.ResetLogging();
        }

        [Test]
        public void DeleteProjectDataDirectoryShouldNotThrowExceptionOnNewProjectAndShouldNotHang()
        {
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                gui.CommandHandler.TryCreateNewProject();
                gui.CommandHandler.TryCreateNewProject();
            }
        }

        [Test]
        public void ProgressDialogIsModal()
        {
            if (!Environment.UserInteractive)
            {
                return; //progress dialog stuff isn't processed on non-interactive environment (see RingtoetsGui::UpdateProgressDialog)
            }

            if (GuiTestHelper.IsBuildServer)
            {
                return; // bleh (fails if users log in etc..)
            }

            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                var mainWindow = (Control) gui.MainWindow;
                Action onShown = delegate
                {
                    var testActivity = new TestActivity();
                    gui.ApplicationCore.ActivityRunner.Enqueue(testActivity);
                    try
                    {
                        while (!gui.ApplicationCore.ActivityRunner.IsRunningActivity(testActivity))
                        {
                            Thread.Sleep(0);
                        }

                        System.Windows.Forms.Application.DoEvents();

                        Assert.IsFalse(mainWindow.IsEnabled);
                    }
                    finally
                    {
                        testActivity.Done = true;
                    }

                    while (gui.ApplicationCore.ActivityRunner.IsRunningActivity(testActivity))
                    {
                        Thread.Sleep(0);
                    }

                    System.Windows.Forms.Application.DoEvents();

                    Assert.IsTrue(mainWindow.IsEnabled);
                };

                WpfTestHelper.ShowModal(mainWindow, onShown);
            }
        }

        [Test]
        public void ClosingEmptyProjectShouldNotGiveException()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.Run();
                gui.CommandHandler.TryCloseProject(); //should not trigger exception
            }
        }

        [Test]
        public void StartGuiWithToolboxDoesNotCrash()
        {
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());

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
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());
                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                gui.Plugins.Add(new SharpMapGisGuiPlugin());
                gui.Plugins.Add(new CommonToolsGuiPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                Action onShown = () => TestHelper.AssertIsFasterThan(300, gui.ApplicationCore.CreateNewProject);

                WpfTestHelper.ShowModal((Control) gui.MainWindow, onShown);
            }
        }

        [Test]
        public void ExitShouldBeFast()
        {
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                applicationCore.UserSettings["autosaveWindowLayout"] = false; // skip damagin of window layout

                gui.MainWindow.Show();

                for (int i = 0; i < 20; i++)
                {
                    var map = new Map();
                    applicationCore.Project.Items.Add(map);
                    gui.CommandHandler.OpenView(map);
                }
                applicationCore.SaveProjectAs(TestHelper.GetCurrentMethodName() + ".dsproj");

                TestHelper.AssertIsFasterThan(200, gui.Exit);
            }
        }

        [Test]
        public void GuiSelectionIsSetToProjectAfterStartWithProjectExplorer()
        {
            // initialize
            using (var gui = new RingtoetsGui())
            {
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                Assert.AreEqual(gui.ApplicationCore.Project, gui.Selection);
            }
        }

        [Test]
        public void FormActionIsRunForMainWindow()
        {
            //testing testhelper + visible changed event of mainwindow.
            //could be tested separately but the combination is vital to many tests. That's why this test is here.
            using (var gui = new RingtoetsGui())
            {
                gui.Run();
                int callCount = 0;
                WpfTestHelper.ShowModal((Control) gui.MainWindow, () => callCount++);
                Assert.AreEqual(1, callCount);
            }
        }

        [Test]
        public void SelectingProjectNodeSetsSelectedItemToProject()
        {
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                var projectExplorer = gui.ToolWindowViews.OfType<ProjectExplorer>().First();

                var treeView = projectExplorer.TreeView;
                treeView.SelectedNode = treeView.Nodes[0]; // project node

                Assert.AreEqual(applicationCore.Project, gui.Selection);
            }
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
            using (var gui = new RingtoetsGui())
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.Plugins.Add(new SharpMapGisApplicationPlugin());
                applicationCore.Plugins.Add(new CommonToolsApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }
        }
    }
}