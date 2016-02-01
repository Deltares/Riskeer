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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Selection;
using Core.Common.Utils.Extensions;
using ProjectExplorerResources = Core.Plugins.ProjectExplorer.Properties.Resources;

namespace Core.Plugins.ProjectExplorer
{
    public class ProjectExplorerGuiPlugin : GuiPlugin
    {
        private IToolViewController toolViewController;
        private IDocumentViewController documentViewController;
        private IViewCommands viewCommands;
        private IProjectOwner projectOwner;
        private IProjectCommands projectCommands;
        private IApplicationSelection applicationSelection;
        private IGuiPluginsHost guiPluginsHost;

        public ProjectExplorerGuiPlugin()
        {
            Instance = this;
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new Ribbon(toolViewController);
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
                    projectCommands = value.ProjectCommands;
                    applicationSelection = value;
                    documentViewController = value;
                    viewCommands = value.ViewCommands;
                    guiPluginsHost = value;
                }
                else
                {
                    toolViewController = null;
                    projectOwner = null;
                    projectCommands = null;
                    applicationSelection = null;
                    documentViewController = null;
                    viewCommands = null;
                    guiPluginsHost = null;
                }

            }
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<Project>
            {
                Text = project => project.Name,
                Image = project => ProjectExplorerResources.Project,
                ChildNodeObjects = project => project.Items.ToArray(),
                ContextMenuStrip = (project, sourceNode, treeNodeInfo) =>
                {
                    var addItem = new StrictContextMenuItem(
                        ProjectExplorerResources.AddItem,
                        ProjectExplorerResources.AddItem_ToolTip,
                        ProjectExplorerResources.plus,
                        (s, e) => Gui.ProjectCommands.AddNewItem(project));

                    return Gui.Get(sourceNode, treeNodeInfo)
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

        public static ProjectExplorerGuiPlugin Instance { get; private set; }

        public ProjectExplorer ProjectExplorer { get; private set; }

        public void InitializeProjectTreeView()
        {
            if (ProjectExplorer == null || ProjectExplorer.IsDisposed)
            {
                ProjectExplorer = new ProjectExplorer(applicationSelection, viewCommands, projectOwner, documentViewController);

                Gui.Plugins
                   .SelectMany(pluginGui => pluginGui.GetTreeNodeInfos())
                   .ForEachElementDo(tni => ProjectExplorer.ProjectTreeView.TreeView.TreeViewController.RegisterTreeNodeInfo(tni));

                ProjectExplorer.ProjectTreeView.Project = projectOwner.Project;
                ProjectExplorer.ProjectTreeView.TreeView.TreeViewController.NodeUpdated += (s, e) => documentViewController.UpdateToolTips();
                ProjectExplorer.Text = Properties.Resources.ProjectExplorerPluginGui_InitializeProjectTreeView_Project_Explorer;
            }

            //add project treeview as a toolwindowview.
            toolViewController.ToolWindowViews.Add(ProjectExplorer, ViewLocation.Left | ViewLocation.Top);
        }

        public override void Activate()
        {
            base.Activate();

            InitializeProjectTreeView();

            projectOwner.ProjectOpened += ApplicationProjectOpened;
            projectOwner.ProjectClosing += ApplicationProjectClosed;
        }

        public override void Dispose()
        {
            if (ProjectExplorer != null)
            {
                ProjectExplorer.Dispose();
                ProjectExplorer = null;
            }

            base.Dispose();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            projectOwner.ProjectOpened -= ApplicationProjectOpened;
            projectOwner.ProjectClosing -= ApplicationProjectClosed;
            toolViewController.ToolWindowViews.Remove(ProjectExplorer);
        }

        private void ApplicationProjectClosed(Project project)
        {
            ProjectExplorer.ProjectTreeView.Project = null;
        }

        private void ApplicationProjectOpened(Project project)
        {
            ProjectExplorer.ProjectTreeView.Project = project;
        }
    }
}