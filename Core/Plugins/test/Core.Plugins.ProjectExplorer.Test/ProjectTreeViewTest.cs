using System.Configuration;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Plugins.ProjectExplorer.NodePresenters;

using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectTreeViewTest : NUnitFormTest
    {
        [Test]
        public void Init()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var documentViews = mocks.Stub<IViewList>();
            var settings = mocks.Stub<ApplicationSettingsBase>();
            var applicationCore = new ApplicationCore();

            Expect.Call(gui.ApplicationCore).Return(applicationCore).Repeat.Any();
            Expect.Call(gui.UserSettings).Return(settings).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(documentViews).Repeat.Any();

            mocks.ReplayAll();

            var pluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            using (var projectTreeView = new ProjectTreeView(pluginGui))
            {
                Assert.IsNotNull(projectTreeView);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void WhenRemovingNodeFromProjectTree_ThenCommandHandlerRemovesAllViewsForRemovedItems()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                StringAssert.StartsWith("Weet u zeker dat u het volgende element wilt verwijderen:", messageBox.Text);
                Assert.AreEqual("Bevestigen", messageBox.Title);
                messageBox.ClickOk();
            };

            object item = 1;

            var project = new Project();
            project.Items.Add(item);

            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            var integerNodePresenter = mocks.Stub<ITreeNodePresenter>();
            integerNodePresenter.Expect(np => np.CanRemove(project, item)).Return(true);
            integerNodePresenter.Expect(np => np.RemoveNodeData(project, item)).Return(true);
            integerNodePresenter.Stub(np => np.NodeTagType).Return(typeof(int));
            integerNodePresenter.Stub(np => np.UpdateNode(null, null, null)).IgnoreArguments();
            integerNodePresenter.Stub(np => np.GetChildNodeObjects(item)).Return(new object[0]);

            var commandHandler = mocks.Stub<IGuiCommandHandler>();
            commandHandler.Expect(ch => ch.RemoveAllViewsForItem(item));

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.CommandHandler).Return(commandHandler);
            mocks.ReplayAll();

            using(var guiPlugin = new ProjectExplorerGuiPlugin { Gui = gui })
            using (var projectTree = new ProjectTreeView(guiPlugin))
            {
                projectTree.TreeView.RegisterNodePresenter(new ProjectNodePresenter(menuBuilderProvider, commandHandler));
                projectTree.TreeView.RegisterNodePresenter(integerNodePresenter);
                projectTree.Project = project;

                // Call
                projectTree.TreeView.SelectedNode = projectTree.TreeView.Nodes[0].Nodes.First();
                projectTree.TreeView.TryDeleteSelectedNodeData();
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}