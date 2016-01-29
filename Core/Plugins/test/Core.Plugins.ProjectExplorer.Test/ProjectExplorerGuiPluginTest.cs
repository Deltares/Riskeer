using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerGuiPluginTest
    {
        [Test]
        public void RegisteringTreeNodeAddsToTreeView()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var guiStub = mocks.Stub<IGui>();
            var otherGuiPlugin = mocks.Stub<GuiPlugin>();

            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            otherGuiPlugin.Expect(g => g.GetTreeNodeInfos()).Return(new List<TreeNodeInfo> { new TreeNodeInfo { TagType = typeof(int) } });

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            guiStub.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectOpened -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectClosing += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectClosing -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            guiStub.Expect(g => g.DocumentViews).Return(mocks.Stub<IViewList>()).Repeat.Any();

            guiStub.Replay();

            using (var projectExplorerGuiPlugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                guiStub.Expect(g => g.Plugins).Return(new List<GuiPlugin>
                {
                    projectExplorerGuiPlugin, otherGuiPlugin
                }).Repeat.Any();

                mocks.ReplayAll();

                // Call
                projectExplorerGuiPlugin.Activate();

                // Assert
                TreeNodeInfo[] treeNodeInfos = projectExplorerGuiPlugin.ProjectExplorer.TreeView.TreeViewController.TreeNodeInfos.ToArray();
                Assert.AreEqual(2, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Project)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(int)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ReopeningProjectExplorerKeepsCustomNodePresenter()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var guiStub = mocks.Stub<IGui>();
            var otherGuiPlugin = mocks.Stub<GuiPlugin>();

            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            otherGuiPlugin.Stub(g => g.GetTreeNodeInfos()).Return(new List<TreeNodeInfo> { new TreeNodeInfo { TagType = typeof(int) } });

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            guiStub.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectOpened -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectClosing += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ProjectClosing -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            guiStub.Expect(g => g.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            guiStub.Expect(g => g.DocumentViews).Return(mocks.Stub<IViewList>()).Repeat.Any();

            guiStub.Replay();

            using (var projectExplorerGuiPlugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                guiStub.Expect(g => g.Plugins).Return(new List<GuiPlugin>
                {
                    projectExplorerGuiPlugin, otherGuiPlugin
                }).Repeat.Any();

                mocks.ReplayAll();

                // Call
                projectExplorerGuiPlugin.Activate(); // This will create the project treeview
                projectExplorerGuiPlugin.ProjectExplorer.Dispose(); // This is somewhat similar to closing the treeview
                projectExplorerGuiPlugin.InitializeProjectTreeView(); // This is what the show command does

                // Assert
                TreeNodeInfo[] treeNodeInfos = projectExplorerGuiPlugin.ProjectExplorer.TreeView.TreeViewController.TreeNodeInfos.ToArray();
                Assert.AreEqual(2, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Project)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(int)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(1, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Project)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_DataIsProjectWithChildren_ReturnChildren()
        {
            // Setup
            var project = new Project();
            project.Items.Add(1);
            project.Items.Add(2);
            project.Items.Add(3);

            var plugin = new ProjectExplorerGuiPlugin();

            // Call
            var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(project);

            // Assert
            var expectedResult = project.Items;
            CollectionAssert.AreEquivalent(expectedResult, childrenWithViewDefinitions);
        }

        [Test]
        public void GetChildDataWithViewDefinitions_UnsupportedDataType_ReturnEmpty()
        {
            // Setup
            var plugin = new ProjectExplorerGuiPlugin();

            // Call
            var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(2);

            // Assert
            CollectionAssert.IsEmpty(childrenWithViewDefinitions);
        }
    }
}