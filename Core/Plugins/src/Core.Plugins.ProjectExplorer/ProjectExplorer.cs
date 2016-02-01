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

using System;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Selection;
using Core.Plugins.ProjectExplorer.Properties;
using log4net;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectExplorer));
        private readonly IDocumentViewController documentViewController;
        private readonly IViewCommands viewCommands;

        public ProjectExplorer(IApplicationSelection applicationSelection, IViewCommands viewCommands,
                               IProjectOwner projectOwner, IDocumentViewController documentViewController)
        {
            InitializeComponent();
            ProjectTreeView = new ProjectTreeView(applicationSelection, viewCommands, projectOwner, documentViewController)
            {
                Dock = DockStyle.Fill
            };
            treeViewPanel.Controls.Add(ProjectTreeView);

            this.viewCommands = viewCommands;
            this.documentViewController = documentViewController;
            this.documentViewController.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
        }

        public ProjectTreeView ProjectTreeView { get; private set; }

        public object Data
        {
            get
            {
                return ProjectTreeView.Data;
            }
            set
            {
                ProjectTreeView.Data = value;
            }
        }

        public new void Dispose()
        {
            ProjectTreeView.Data = null;
            if (documentViewController != null && documentViewController.DocumentViews != null)
            {
                documentViewController.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
            }

            ProjectTreeView.Dispose();
            base.Dispose();
        }

        public TreeView TreeView
        {
            get
            {
                return ProjectTreeView.TreeView;
            }
        }

        public void ScrollTo(object o)
        {
            var nodeToSelect = TreeView.TreeViewController.GetNodeByTag(o);

            if (nodeToSelect == null)
            {
                log.DebugFormat(Resources.ProjectExplorer_ScrollTo_Can_t_find_tree_node_for_item_0_, o);
                return;
            }

            TreeView.SelectedNode = nodeToSelect;
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            buttonScrollToItemInActiveView.Enabled = documentViewController.DocumentViews.ActiveView != null &&
                                                     viewCommands.GetDataOfActiveView() != null;
        }

        private void ButtonScrollToItemInActiveViewClick(object sender, EventArgs e)
        {
            var dataOfActiveView = viewCommands.GetDataOfActiveView();
            if (dataOfActiveView != null)
            {
                ScrollTo(dataOfActiveView);

                return;
            }

            log.WarnFormat(Resources.ProjectExplorer_ButtonScrollToItemInActiveViewClick_Can_t_find_project_item_for_view_data_0_, documentViewController.DocumentViews.ActiveView.Data);
        }
    }
}