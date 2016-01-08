using System.Linq;
using Core.Common.Gui;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer;
using Core.Plugins.SharpMapGis;
using NUnit.Framework;
using Control = System.Windows.Controls.Control;

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

                applicationCore.AddPlugin(new SharpMapGisApplicationPlugin());

                gui.Run();
            }
        }

        [Test]
        public void StartWithCommonPluginsShouldBeFast()
        {
            TestHelper.AssertIsFasterThan(7500, StartWithCommonPlugins);
        }

        [Test]
        public void GuiSelectionIsSetToProjectAfterStartWithProjectExplorer()
        {
            // initialize
            using (var gui = new RingtoetsGui())
            {
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                Assert.AreEqual(gui.Project, gui.Selection);
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
                var applicationCore = gui.ApplicationCore;

                applicationCore.AddPlugin(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }
        }
    }
}