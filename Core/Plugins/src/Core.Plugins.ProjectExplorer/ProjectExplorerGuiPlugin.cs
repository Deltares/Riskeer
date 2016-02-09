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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Selection;
using Core.Plugins.ProjectExplorer.Commands;
using Core.Plugins.ProjectExplorer.Exceptions;
using ProjectExplorerResources = Core.Plugins.ProjectExplorer.Properties.Resources;

namespace Core.Plugins.ProjectExplorer
{
    public class ProjectExplorerGuiPlugin : GuiPlugin
    {
        private IToolViewController toolViewController;
        private ProjectExplorerViewController projectExplorerViewController;
        private IDocumentViewController documentViewController;
        private IViewCommands viewCommands;
        private IProjectOwner projectOwner;
        private IApplicationSelection applicationSelection;
        private Ribbon ribbonCommandHandler;
        private IEnumerable<TreeNodeInfo> treeNodeInfos;
        private bool active;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbonCommandHandler;
            }
        }

        public override IGui Gui
        {
            get
            {
                return base.Gui;
            }
            set
            {
                base.Gui = value;

                if (value != null)
                {
                    toolViewController = value;
                    projectOwner = value;
                    applicationSelection = value;
                    documentViewController = value;
                    viewCommands = value.ViewCommands;
                    treeNodeInfos = value.GetTreeNodeInfos();
                }
                else
                {
                    toolViewController = null;
                    projectOwner = null;
                    applicationSelection = null;
                    documentViewController = null;
                    viewCommands = null;
                    treeNodeInfos = null;
                }
            }
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<Project>
            {
                Text = project => project.Name,
                Image = project => ProjectExplorerResources.ProjectIcon,
                ChildNodeObjects = project => project.Items.ToArray(),
                ContextMenuStrip = (project, parentData, treeViewControl) =>
                {
                    var addItem = new StrictContextMenuItem(
                        ProjectExplorerResources.AddItem,
                        ProjectExplorerResources.AddItem_ToolTip,
                        ProjectExplorerResources.PlusIcon,
                        (s, e) => Gui.ProjectCommands.AddNewItem(project));

                    return Gui.Get(project, treeViewControl)
                              .AddCustomItem(addItem)
                              .AddSeparator()
                              .AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddExpandAllItem()
                              .AddCollapseAllItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
                }
            };
        }

        public override IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            var project = dataObject as Project;
            if (project != null)
            {
                foreach (var item in project.Items)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Activates the <see cref="ProjectExplorerGuiPlugin"/>
        /// </summary>
        /// <exception cref="PluginActivationException">Thrown when <see cref="Gui"/> is <c>null</c>.</exception>
        public override void Activate()
        {
            if (active)
            {
                var message = string.Format(ProjectExplorerResources.ProjectExplorerGuiPlugin_Cannot_activate_0_twice, ProjectExplorerResources.General_ProjectExplorer);
                throw new PluginActivationException(message);
            }

            base.Activate();
            try
            {
                projectExplorerViewController = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);
            }
            catch (ArgumentNullException e)
            {
                var message = string.Format(ProjectExplorerResources.ProjectExplorerGuiPlugin_Activation_of_0_failed, ProjectExplorerResources.General_ProjectExplorer);
                throw new PluginActivationException(message, e);
            }

            ribbonCommandHandler = new Ribbon
            {
                ToggleExplorerCommand = new ToggleProjectExplorerCommand(projectExplorerViewController)
            };

            projectExplorerViewController.OnOpenView += (s, e) => UpdateProject();
            projectExplorerViewController.OpenView();

            projectOwner.ProjectOpened += ApplicationProjectOpened;
            active = true;
        }

        public override void Dispose()
        {
            Deactivate();
            if (projectExplorerViewController != null)
            {
                projectExplorerViewController.Dispose();
            }
        }

        public override void Deactivate()
        {
            if (active)
            {
                base.Deactivate();
                projectOwner.ProjectOpened -= ApplicationProjectOpened;
                active = false;
            }
        }

        private void ApplicationProjectOpened(Project project)
        {
            UpdateProject();
        }

        private void UpdateProject()
        {
            projectExplorerViewController.Update(projectOwner.Project);
        }
    }
}