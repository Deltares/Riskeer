using System.Configuration;
using System.Linq;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DeltaShell.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

// note: this is a reference to a builded assembly! not the project itself

namespace DeltaShell.Plugins.ProjectExplorer.Tests
{
    [TestFixture]
    public class ProjectTreeViewTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [SetUp]
        public void SetUp() {}

        [Test]
        public void Init()
        {
            var gui = mocks.Stub<IGui>();
            var app = mocks.StrictMock<IApplication>();
            var settings = mocks.Stub<ApplicationSettingsBase>();

            var documentViews = mocks.Stub<IViewList>();
            gui.Application = app;
            Expect.Call(gui.DocumentViews).Return(documentViews).Repeat.Any();

            // in case of mock
            Expect.Call(app.UserSettings).Return(settings).Repeat.Any();
            mocks.ReplayAll();

            var pluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            var projectTreeView = new ProjectTreeView(pluginGui);
            Assert.IsNotNull(projectTreeView);
        }

        [Test]
        public void SelectingProjectNodeSetsSelectedItemToProject()
        {
            var gui = new DeltaShellGui();
            var app = gui.Application;

            gui.Plugins.Add(new ProjectExplorerGuiPlugin());
            gui.Run();

            var projectExplorer = gui.ToolWindowViews.OfType<ProjectExplorer>().First();

            var treeView = projectExplorer.TreeView;
            treeView.SelectedNode = treeView.Nodes[0]; // project node

            gui.Selection.Should().Be.EqualTo(app.Project);
        }
    }
}