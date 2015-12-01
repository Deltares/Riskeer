using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerPluginGuiTest
    {
        private IGui gui;
        private MockRepository mocks;
        private ApplicationCore applicationCore;
        private ITreeNodePresenter mockNodePresenter;
        private ProjectExplorerGuiPlugin projectExplorerPluginGui;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();

            var project = new Project();
            var plugin = mocks.Stub<ApplicationPlugin>();
            var pluginGui = mocks.Stub<GuiPlugin>();
            projectExplorerPluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything).Repeat.Any();
            gui.Expect(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            gui.Expect(g => g.ProjectOpened -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            gui.Expect(g => g.ProjectClosing += Arg<Action<Project>>.Is.Anything).Repeat.Any();
            gui.Expect(g => g.ProjectClosing -= Arg<Action<Project>>.Is.Anything).Repeat.Any();
            Expect.Call(gui.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new List<GuiPlugin>
            {
                projectExplorerPluginGui, pluginGui
            }).Repeat.Any();
            Expect.Call(gui.ContextMenuProvider).Return(mocks.StrictMock<IContextMenuBuilderProvider>()).Repeat.Any();
            gui.CommandHandler = mocks.StrictMock<IGuiCommandHandler>();

            gui.Project = project;

            //create and register a custom np
            mockNodePresenter = mocks.Stub<ITreeNodePresenter>();
            Expect.Call(pluginGui.GetProjectTreeViewNodePresenters()).Return(new[]
            {
                mockNodePresenter
            });

            applicationCore = new ApplicationCore();
            gui.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            gui.ApplicationCore.AddPlugin(plugin);
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void RegisteringTreeNodeAddsToTreeView()
        {
            projectExplorerPluginGui.Activate(); //this will create the projecttreeview.

            var projectTreeView = projectExplorerPluginGui.ProjectExplorer.TreeView;
            Assert.IsTrue(projectTreeView.NodePresenters.Contains(mockNodePresenter));
        }

        [Test]
        public void ReopeningProjectExplorerKeepsCustomNodePresenter()
        {
            projectExplorerPluginGui.Activate(); //this will create the projecttreeview.

            //this is somewhat similar to closing the treeview..
            projectExplorerPluginGui.ProjectExplorer.Dispose();

            //this is what the show command does
            projectExplorerPluginGui.InitializeProjectTreeView();

            //assert all is well again
            var projectTreeView = projectExplorerPluginGui.ProjectExplorer.TreeView;
            Assert.IsTrue(projectTreeView.NodePresenters.Contains(mockNodePresenter));
        }
    }
}