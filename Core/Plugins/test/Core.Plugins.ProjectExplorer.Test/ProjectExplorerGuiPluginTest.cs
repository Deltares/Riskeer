using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Selection;

using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerGuiPluginTest
    {
        [Test]
        [RequiresSTA]
        public void RegisteringTreeNodeAddsToTreeView()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            var otherGuiPlugin = mocks.StrictMock<GuiPlugin>();

            gui.Expect(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Expect(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());

            gui.Expect(g => g.IsToolWindowOpen<ProjectExplorer>()).Return(true).Repeat.Times(3);
            gui.Expect(g => g.CloseToolView(Arg<ProjectExplorer>.Matches(r => true)));

            gui.Expect(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything);

            gui.Replay();

            using (var projectExplorerGuiPlugin = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            })
            {
                gui.Expect(g => g.Plugins).Return(new List<GuiPlugin>
                {
                    projectExplorerGuiPlugin, otherGuiPlugin
                }).Repeat.Any();

                mocks.ReplayAll();

                // Call
                projectExplorerGuiPlugin.Activate();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
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