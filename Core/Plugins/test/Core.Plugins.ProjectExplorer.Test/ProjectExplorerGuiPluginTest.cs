// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewHost;
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
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsNull(plugin.Gui);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
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
            var viewHost = mocks.Stub<IViewHost>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            guiStub.Stub(g => g.ViewHost).Return(viewHost);
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
            var viewHost = mocks.Stub<IViewHost>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            guiStub.Stub(g => g.ViewHost).Return(viewHost);
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
            var viewHost = mocks.Stub<IViewHost>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            guiStub.Stub(g => g.ProjectCommands).Return(mocks.Stub<IProjectCommands>());
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            guiStub.Stub(g => g.ViewHost).Return(viewHost);
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
            var viewHost = mocks.StrictMock<IViewHost>();
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
            guiStub.Stub(g => g.ViewHost).Return(viewHost);

            // Activate
            var toolViews = new List<IView>();
            viewHost.Stub(vm => vm.ToolViews).Return(toolViews);
            viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Matches(v => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation =>
            {
                toolViews.Add(invocation.Arguments[0] as ProjectExplorer);
            });
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();

            // Dispose
            viewHost.Expect(tvc => tvc.Remove(Arg<ProjectExplorer>.Matches(v => true)));

            guiStub.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Expect(g => g.ProjectOpened -= null).IgnoreArguments();

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
                Assert.AreEqual(1, toolViews.Count);
                Assert.AreSame(initialProject, toolViews[0].Data);

                // Call
                var newProject = new Project();
                guiStub.Project = newProject;
                guiStub.Raise(s => s.ProjectOpened += null, newProject);

                // Assert
                Assert.AreSame(newProject, toolViews[0].Data);
            }
        }
    }
}