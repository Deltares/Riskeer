using System.Linq;
using System.Windows.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer;
using NUnit.Framework;
using Ringtoets.Integration.Plugin;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
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
        [RequiresSTA]
        public void CloseProject_EmptyProject_DoesNotThrowException()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                gui.Run();
                TestDelegate closeProject = () => gui.StorageCommands.CloseProject();
                Assert.DoesNotThrow(closeProject);
            }
        }

        [Test]
        [RequiresSTA]
        public void StartGuiWithToolboxDoesNotCrash()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                var applicationCore = gui.ApplicationCore;

                applicationCore.AddPlugin(new RingtoetsApplicationPlugin());

                gui.Run();
            }
        }

        [Test]
        [RequiresSTA]
        public void StartWithCommonPluginsShouldBeFast()
        {
            TestHelper.AssertIsFasterThan(7500, StartWithCommonPlugins);
        }

        [Test]
        [RequiresSTA]
        public void GuiSelectionIsSetToProjectAfterStartWithProjectExplorer()
        {
            // initialize
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                Assert.AreEqual(gui.Project, gui.Selection);
            }
        }

        [Test]
        [RequiresSTA]
        public void FormActionIsRunForMainWindow()
        {
            //testing testhelper + visible changed event of mainwindow.
            //could be tested separately but the combination is vital to many tests. That's why this test is here.
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                gui.Run();
                int callCount = 0;
                WpfTestHelper.ShowModal((Control) gui.MainWindow, () => callCount++);
                Assert.AreEqual(1, callCount);
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectingProjectNodeSetsSelectedItemToProject()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                var projectExplorer = gui.ToolWindowViews.OfType<ProjectExplorer>().First();

                var treeView = projectExplorer.TreeView;
                treeView.SelectedNode = treeView.Nodes[0]; // project node

                Assert.AreEqual(gui.Project, gui.Selection);
            }
        }

        private static void StartWithCommonPlugins()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.MainWindow = new MainWindow(gui);
                var applicationCore = gui.ApplicationCore;

                applicationCore.AddPlugin(new RingtoetsApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }
        }
    }
}