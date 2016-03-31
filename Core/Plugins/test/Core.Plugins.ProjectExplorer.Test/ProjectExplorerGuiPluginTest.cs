using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Plugins.ProjectExplorer.Exceptions;
using Core.Plugins.ProjectExplorer.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            using (var plugin = new ProjectExplorerGuiPlugin())
            {
                // Assert
                Assert.IsInstanceOf<GuiPlugin>(plugin);
                Assert.IsNull(plugin.Gui);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        [RequiresSTA]
        public void RegisteringTreeNodeAddsToTreeView()
        {
            // Setup
            var mocks = new MockRepository();
            using (var projectExplorerGuiPlugin = new ProjectExplorerGuiPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var otherGuiPlugin = mocks.StrictMock<GuiPlugin>();

                gui.Expect(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Expect(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());

                gui.Expect(g => g.IsToolWindowOpen<ProjectExplorer>()).Return(true);

                gui.Expect(g => g.ProjectOpened += Arg<Action<Project>>.Is.Anything);
                gui.Expect(g => g.ProjectOpened -= Arg<Action<Project>>.Is.Anything);

                gui.Expect(g => g.Plugins).Return(new List<GuiPlugin>
                {
                    projectExplorerGuiPlugin, otherGuiPlugin
                }).Repeat.Any();

                mocks.ReplayAll();

                projectExplorerGuiPlugin.Gui = gui;

                // Call
                projectExplorerGuiPlugin.Activate();
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Activate_WithoutGui_ThrowsPluginActivationException()
        {
            // Setup
            using (var plugin = new ProjectExplorerGuiPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                var message = Assert.Throws<PluginActivationException>(test).Message;
                var expected = string.Format(Resources.ProjectExplorerGuiPlugin_Activation_of_0_failed, Resources.General_ProjectExplorer);
                Assert.AreEqual(expected, message);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithGui_SubscribesToProjectEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            guiStub.Stub(g => g.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            guiStub.Stub(g => g.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            guiStub.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            guiStub.Stub(g => g.SelectionChanged -= null).IgnoreArguments();

            guiStub.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                plugin.Activate();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void Activate_AlreadyActivated_ThrowsPluginActivationException()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            guiStub.Stub(g => g.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            guiStub.Stub(g => g.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            guiStub.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            guiStub.Stub(g => g.SelectionChanged -= null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                plugin.Activate();

                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                var message = Assert.Throws<PluginActivationException>(test).Message;
                var expected = string.Format(Resources.ProjectExplorerGuiPlugin_Cannot_activate_0_twice, Resources.General_ProjectExplorer);
                Assert.AreEqual(expected, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void Deactivate_WhenActive_DesubscribesToProjectEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            guiStub.Stub(g => g.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            guiStub.Stub(g => g.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            guiStub.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            guiStub.Stub(g => g.SelectionChanged -= null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();

            guiStub.Expect(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                plugin.Activate();

                // Call
                plugin.Deactivate();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void Deactivate_AlwaysWhenNotActive_DoesNotThrow()
        {
            // Setup
            using (var plugin = new ProjectExplorerGuiPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Deactivate();

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        [RequiresSTA]
        public void Dispose_AlwaysWhenNotActive_DoesNotThrow()
        {
            // Setup
            var plugin = new ProjectExplorerGuiPlugin();

            // Call
            TestDelegate test = () => plugin.Dispose();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [RequiresSTA]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            var mocks = new MockRepository();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());

            mocks.ReplayAll();

            using (var guiPlugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // Assert
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

            using (var plugin = new ProjectExplorerGuiPlugin())
            {
                // Call
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(project);

                // Assert
                var expectedResult = project.Items;
                CollectionAssert.AreEquivalent(expectedResult, childrenWithViewDefinitions);
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_UnsupportedDataType_ReturnEmpty()
        {
            // Setup
            using (var plugin = new ProjectExplorerGuiPlugin())
            {
                // Call
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(2);

                // Assert
                CollectionAssert.IsEmpty(childrenWithViewDefinitions);
            }
        }

        [Test]
        [RequiresSTA]
        public void ProjectOpened_PluginActivated_UpdateProjectExplorer()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project)
                }
            });

            ProjectExplorer view = null;

            using (mocks.Ordered())
            {
                // Activate
                guiStub.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
                guiStub.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true))).WhenCalled(invocation => {
                    view = invocation.Arguments[0] as ProjectExplorer;
                });
                guiStub.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                guiStub.Expect(dvc => dvc.UpdateToolTips());
                guiStub.Expect(g => g.ProjectOpened += null).IgnoreArguments();

                // UpdateProject
                guiStub.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                guiStub.Expect(dvc => dvc.UpdateToolTips());

                // Dispose
                guiStub.Expect(g => g.ProjectOpened -= null).IgnoreArguments();
                guiStub.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                guiStub.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
            }

            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerGuiPlugin
            {
                Gui = guiStub
            })
            {
                var initialProject = new Project();
                guiStub.Project = initialProject;
                plugin.Activate();

                // Precondition
                Assert.AreSame(view.Data, initialProject);

                // Call
                var newProject = new Project();
                guiStub.Project = newProject;
                guiStub.Raise(s => s.ProjectOpened += null, newProject);

                // Assert
                Assert.AreSame(view.Data, newProject);
            }
        }
    }
}