﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Plugin.TestUtil
{
    /// <summary>
    /// Test fixture for verifying the InAssembly behavior of a TreeNodeInfo.
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugin to create the tests for.</typeparam>
    /// <typeparam name="TFailurePath">The type of <see cref="IFailureMechanism"/> to create the tests for.</typeparam>
    /// <typeparam name="TFailurePathContext">The type of <see cref="IFailurePathContext{T}"/> to create the tests for.</typeparam>
    [TestFixture]
    public abstract class FailurePathInAssemblyTreeNodeInfoTestFixtureBase<TPlugin, TFailurePath, TFailurePathContext>
        where TPlugin : PluginBase, new()
        where TFailurePath : IFailureMechanism, new()
        where TFailurePathContext : IFailurePathContext<TFailurePath>
    {
        private readonly int contextMenuIndexWhenInAssemblyFalse;
        private readonly int contextMenuIndexWhenInAssemblyTrue;

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyTrueAndClickOnInAssemblyItem_MakeFailureMechanismInAssemblyFalseAndRemovesAllViewsForItem()
        {
            // Setup
            var mocks = new MockRepository();

            var failureMechanism = new TFailurePath();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            TFailurePathContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(context));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                       {
                           Gui = gui
                       })
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuIndexWhenInAssemblyTrue].PerformClick();

                        // Assert
                        Assert.IsFalse(failureMechanism.InAssembly);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyFalseAndClickOnInAssemblyItem_MakeFailureMechanismInAssemblyTrueAndRemovesAllViewsForItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new TFailurePath
            {
                InAssembly = false
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            TFailurePathContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(context));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                       {
                           Gui = gui
                       })
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuIndexWhenInAssemblyFalse].PerformClick();

                        // Assert
                        Assert.IsTrue(failureMechanism.InAssembly);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyTrue_AddCustomItems()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new TFailurePath();
                TFailurePathContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                       {
                           Gui = gui
                       })
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuIndexWhenInAssemblyTrue,
                                                                      "I&n assemblage",
                                                                      "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                                      RiskeerCommonFormsResources.Checkbox_ticked);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyFalse_AddCustomItems()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new TFailurePath
                {
                    InAssembly = false
                };

                TFailurePathContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                       {
                           Gui = gui
                       })
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuIndexWhenInAssemblyFalse,
                                                                      "I&n assemblage",
                                                                      "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                                      RiskeerCommonFormsResources.Checkbox_empty);
                    }
                }
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FailurePathInAssemblyTreeNodeInfoTestFixtureBase{TPlugin,TFailurePath,TFailurePathContext}"/>.
        /// </summary>
        /// <param name="contextMenuIndexWhenInAssemblyTrue">The index of the InAssembly context menu item when the <typeparamref name="TFailurePath"/>
        /// is part of the assembly.</param>
        /// <param name="contextMenuIndexWhenInAssemblyFalse">The index of the InAssembly context menu item when the <typeparamref name="TFailurePath"/>
        /// is not part of the assembly.</param>
        protected FailurePathInAssemblyTreeNodeInfoTestFixtureBase(int contextMenuIndexWhenInAssemblyTrue,
                                                                   int contextMenuIndexWhenInAssemblyFalse)
        {
            this.contextMenuIndexWhenInAssemblyTrue = contextMenuIndexWhenInAssemblyTrue;
            this.contextMenuIndexWhenInAssemblyFalse = contextMenuIndexWhenInAssemblyFalse;
        }

        protected abstract TFailurePathContext CreateFailureMechanismContext(TFailurePath failureMechanism, IAssessmentSection assessmentSection);

        private static TreeNodeInfo GetInfo(TPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(TFailurePathContext));
        }
    }
}