using System.Linq;
using System.Windows.Controls;

using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer;
using NUnit.Framework;

using Rhino.Mocks;

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
        public void StartGuiWithToolboxDoesNotCrash()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(null), projectStore))
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.AddPlugin(new RingtoetsApplicationPlugin());

                gui.Run();
            }
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(null), projectStore))
            {
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                Assert.AreEqual(gui.Project, gui.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void FormActionIsRunForMainWindow()
        {
            //testing testhelper + visible changed event of mainwindow.
            //could be tested separately but the combination is vital to many tests. That's why this test is here.
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(null), projectStore))
            {
                gui.Run();
                int callCount = 0;
                WpfTestHelper.ShowModal((Control) gui.MainWindow, () => callCount++);
                Assert.AreEqual(1, callCount);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void SelectingProjectNodeSetsSelectedItemToProject()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(null), projectStore))
            {
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());
                gui.Run();

                var projectExplorer = gui.ToolWindowViews.OfType<ProjectExplorer>().First();

                var treeView = projectExplorer.TreeView;
                treeView.SelectedNode = treeView.Nodes[0]; // project node

                Assert.AreEqual(gui.Project, gui.Selection);
            }
            mocks.VerifyAll();
        }

        private static void StartWithCommonPlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new RingtoetsGui(new MainWindow(null), projectStore))
            {
                var applicationCore = gui.ApplicationCore;

                applicationCore.AddPlugin(new RingtoetsApplicationPlugin());
                gui.Plugins.Add(new ProjectExplorerGuiPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }
            mocks.VerifyAll();
        }
    }
}