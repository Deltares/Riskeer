using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Core.Common.BaseDelftTools;
using Core.Common.Controls;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Tests
{
    [TestFixture]
    public class ProjectExplorerPluginGuiTest
    {
        private readonly MockRepository mocks = new MockRepository();
        private IApplication app;
        private IGui gui;

        private ITreeNodePresenter mockNodePresenter;
        private ProjectExplorerGuiPlugin projectExplorerPluginGui;

        [SetUp]
        public void SetUp()
        {
            app = mocks.Stub<IApplication>();
            gui = mocks.Stub<IGui>();

            var settings = mocks.Stub<ApplicationSettingsBase>();
            var plugin = mocks.Stub<ApplicationPlugin>();
            var pluginGui = mocks.Stub<GuiPlugin>();

            projectExplorerPluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            settings["showHiddenDataItems"] = true;
            app.UserSettings = settings;

            app.Settings = new NameValueCollection();
            app.Settings["IsProjectExplorerSorted"] = "false";

            var project = new Project();
            Expect.Call(app.Project).Return(project).Repeat.Any();

            gui.Application = app;
            Expect.Call(gui.ToolWindowViews).Return(mocks.Stub<IViewList>());
            Expect.Call(gui.DocumentViews).Return(mocks.Stub<IViewList>());
            Expect.Call(gui.Plugins).Return(new List<GuiPlugin>
            {
                projectExplorerPluginGui, pluginGui
            }).Repeat.Any();

            plugin.Application = app;

            //create and register a custom np
            mockNodePresenter = mocks.Stub<ITreeNodePresenter>();
            Expect.Call(pluginGui.GetProjectTreeViewNodePresenters()).Return(new[]
            {
                mockNodePresenter
            });

            app.Stub(a => a.Plugins).Return(new[]
            {
                plugin
            });
            app.Stub(a => a.ProjectOpened += null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectOpened -= null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectClosing += null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectClosing -= null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectSaving += null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectSaving -= null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectSaved += null).IgnoreArguments().Repeat.Any();
            app.Stub(a => a.ProjectSaved -= null).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();
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