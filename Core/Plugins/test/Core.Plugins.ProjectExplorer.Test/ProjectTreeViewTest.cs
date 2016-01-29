using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
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

            bool removedCalled = false;

            var mocks = new MockRepository();

            var projectTreeNodeInfo = mocks.Stub<TreeNodeInfo<Project>>();
            projectTreeNodeInfo.ChildNodeObjects = p => p.Items.ToArray();

            var integerTreeNodeInfo = mocks.Stub<TreeNodeInfo>();
            integerTreeNodeInfo.TagType = typeof(int);
            integerTreeNodeInfo.CanRemove = (nd, pnd) => nd == item && pnd == project;
            integerTreeNodeInfo.OnNodeRemoved = (nd, pnd) => removedCalled = true;

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
                projectTree.TreeView.TreeViewController.RegisterTreeNodeInfo(projectTreeNodeInfo);
                projectTree.TreeView.TreeViewController.RegisterTreeNodeInfo(integerTreeNodeInfo);
                projectTree.Project = project;

                // Call
                var nodeToDelete = projectTree.TreeView.Nodes[0].Nodes.OfType<TreeNode>().First();
                projectTree.TreeView.TreeViewController.DeleteNode(nodeToDelete, integerTreeNodeInfo);
            }

            // Assert
            Assert.IsTrue(removedCalled);

            mocks.VerifyAll();
        }
    }
}