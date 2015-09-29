using System.Configuration;
using System.Linq;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Services;
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
        public void SetUp()
        {
            
        }

        [Test]
        public void Init()
        {
            var gui = mocks.Stub<IGui>();
            var app = mocks.StrictMock<IApplication>();
            var settings = mocks.Stub<ApplicationSettingsBase>();
            settings["showHiddenDataItems"] = true;

            var documentViews = mocks.Stub<IViewList>();
            gui.Application = app;
            Expect.Call(gui.DocumentViews).Return(documentViews).Repeat.Any();

            var pluginGui = new ProjectExplorerGuiPlugin {Gui = gui};
            
            var projectService = mocks.StrictMock<IProjectService>();

            // in case of mock
            Expect.Call(app.UserSettings).Return(settings).Repeat.Any();

            // we set an expectation that the ProjectSaved event will be attached to, see also
            // http://haacked.com/archive/2006/06/23/UsingRhinoMocksToUnitTestEventsOnInterfaces.aspx
            projectService.ProjectSaved += null;
            LastCall.Repeat.Any();
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

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