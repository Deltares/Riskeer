using System;
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
            var selectionStub = mocks.Stub<IApplicationSelection>();
            var viewCommandsStub = mocks.Stub<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            using (var projectTreeView = new ProjectTreeView(selectionStub, viewCommandsStub, projectOwner, documentViewController))
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

            var projectCommands = mocks.Stub<IProjectCommands>();

            var commandHandler = mocks.Stub<IViewCommands>();
            commandHandler.Expect(ch => ch.RemoveAllViewsForItem(item));

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything);
            projectOwner.Stub(g => g.ProjectOpened -= Arg<Action<Project>>.Is.Anything);

            var selectionStub = mocks.Stub<IApplicationSelection>();
            selectionStub.Stub(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            selectionStub.Stub(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);

            var documentViewController = mocks.Stub<IDocumentViewController>();

            mocks.ReplayAll();

            using (var projectTree = new ProjectTreeView(selectionStub, commandHandler, projectOwner, documentViewController))
            {
                projectTree.TreeView.RegisterNodePresenter(new ProjectNodePresenter(menuBuilderProvider, projectCommands));
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