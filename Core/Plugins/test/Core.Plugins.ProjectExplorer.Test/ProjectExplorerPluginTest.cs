// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Threading;
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
    public class ProjectExplorerPluginTest
    {
        [Test]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            using (var plugin = new ProjectExplorerPlugin())
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
            using (var plugin = new ProjectExplorerPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                string message = Assert.Throws<PluginActivationException>(test).Message;
                string expected = string.Format(Resources.ProjectExplorerPlugin_Activation_of_0_failed, Resources.General_ProjectExplorer);
                Assert.AreEqual(expected, message);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Activate_WithGui_SubscribesToProjectEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var viewHost = mocks.Stub<IViewHost>();
            gui.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.Project).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerPlugin
            {
                Gui = gui
            })
            {
                // Call
                plugin.Activate();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Activate_AlreadyActivated_ThrowsPluginActivationException()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var viewHost = mocks.Stub<IViewHost>();
            gui.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.Project).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerPlugin
            {
                Gui = gui
            })
            {
                plugin.Activate();

                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                string message = Assert.Throws<PluginActivationException>(test).Message;
                string expected = string.Format(Resources.ProjectExplorerPlugin_Cannot_activate_0_twice, Resources.General_ProjectExplorer);
                Assert.AreEqual(expected, message);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Deactivate_WhenActive_DesubscribesToProjectEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var viewHost = mocks.Stub<IViewHost>();
            gui.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.Project).Return(mocks.Stub<IProject>());

            gui.Expect(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerPlugin
            {
                Gui = gui
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
        [Apartment(ApartmentState.STA)]
        public void Deactivate_AlwaysWhenNotActive_DoesNotThrow()
        {
            // Setup
            using (var plugin = new ProjectExplorerPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Deactivate();

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dispose_AlwaysWhenNotActive_DoesNotThrow()
        {
            // Setup
            var plugin = new ProjectExplorerPlugin();

            // Call
            TestDelegate test = () => plugin.Dispose();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetChildDataWithViewDefinitions_UnsupportedDataType_ReturnEmpty()
        {
            // Setup
            using (var plugin = new ProjectExplorerPlugin())
            {
                // Call
                IEnumerable<object> childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(2);

                // Assert
                CollectionAssert.IsEmpty(childrenWithViewDefinitions);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ProjectOpened_PluginActivated_UpdateProjectExplorer()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var initialProject = mocks.Stub<IProject>();
            var newProject = mocks.Stub<IProject>();
            gui.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.GetTreeNodeInfos()).Return(new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject)
                }
            });
            gui.Stub(g => g.ViewHost).Return(viewHost);

            // Activate
            var toolViews = new List<IView>();
            viewHost.Stub(vm => vm.ToolViews).Return(toolViews);
            viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation => toolViews.Add(invocation.Arguments[0] as ProjectExplorer));
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();

            // Dispose
            viewHost.Expect(tvc => tvc.Remove(Arg<ProjectExplorer>.Is.NotNull));

            gui.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Expect(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Expect(g => g.Project).Return(initialProject);
            gui.Expect(g => g.Project).Return(newProject);

            mocks.ReplayAll();

            using (var plugin = new ProjectExplorerPlugin
            {
                Gui = gui
            })
            {
                plugin.Activate();

                // Precondition
                Assert.AreEqual(1, toolViews.Count);
                Assert.AreSame(initialProject, toolViews[0].Data);

                // Call
                gui.Raise(s => s.ProjectOpened += null, newProject);

                // Assert
                Assert.AreSame(newProject, toolViews[0].Data);
            }

            mocks.VerifyAll();
        }
    }
}