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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Selection;
using Core.Plugins.ProjectExplorer.Properties;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public sealed partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        public ProjectExplorer(IApplicationSelection applicationSelection, IViewCommands viewCommands, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            Text = Resources.ProjectExplorerPluginGui_InitializeProjectTreeView_Project_Explorer;

            InitializeComponent();
            ApplicationSelection = applicationSelection;
            ViewCommands = viewCommands;

            foreach (TreeNodeInfo info in treeNodeInfos)
            {
                TreeView.TreeViewController.RegisterTreeNodeInfo(info);
            }

            TreeView.TreeViewController.TreeNodeDoubleClick += TreeViewDoubleClick;
            TreeView.TreeViewController.NodeDataDeleted += ProjectDataDeleted;
            TreeView.AfterSelect += TreeViewSelectedNodeChanged;
            ApplicationSelection.SelectionChanged += GuiSelectionChanged;
        }

        public object Data
        {
            get
            {
                return TreeView.TreeViewController.Data;
            }
            set
            {
                if (!TreeView.IsDisposed)
                {
                    TreeView.TreeViewController.Data = value;
                }
            }
        }

        public TreeView TreeView
        {
            get
            {
                return projectTreeView;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            Data = null;
            TreeView.Dispose();
            ApplicationSelection.SelectionChanged -= GuiSelectionChanged;

            base.Dispose(disposing);
        }

        private IApplicationSelection ApplicationSelection { get; set; }
        private IViewCommands ViewCommands { get; set; }

        private void TreeViewSelectedNodeChanged(object sender, TreeViewEventArgs e)
        {
            ApplicationSelection.Selection = e.Node.Tag;
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            ViewCommands.OpenViewForSelection();
        }

        private void ProjectDataDeleted(object sender, TreeNodeDataDeletedEventArgs e)
        {
            ViewCommands.RemoveAllViewsForItem(e.DeletedDataInstance);
        }

        /// <summary>
        /// Update selected node when selection in gui changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (ApplicationSelection.Selection == null)
            {
                return;
            }

            TreeNode node = TreeView.TreeViewController.GetNodeByTag(ApplicationSelection.Selection);
            if (node != null)
            {
                TreeView.SelectedNode = node;
            }
        }
    }
}