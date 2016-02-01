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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;

using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public sealed class ProjectTreeView : TreeView, IView
    {
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;
        private readonly IProjectOwner projectOwner;
        private Project project;

        private bool selectingNode;

        public ProjectTreeView(IApplicationSelection applicationSelection, IViewCommands viewCommands,
                               IProjectOwner projectOwner)
            : this()
        {
            this.applicationSelection = applicationSelection;
            this.applicationSelection.SelectionChanged += GuiSelectionChanged;

            this.viewCommands = viewCommands;

            this.projectOwner = projectOwner;
        }

        private ProjectTreeView()
        {
            Dock = DockStyle.Fill;

            TreeViewController.TreeNodeDoubleClick += TreeViewDoubleClick;
            TreeViewController.NodeDataDeleted += ProjectDataDeleted;
            AfterSelect += TreeViewSelectedNodeChanged;
        }

        public Project Project
        {
            set
            {
                Data = value;
            }
        }

        public object Data
        {
            get
            {
                return project;
            }
            set
            {
                if (IsDisposed) //only set value in case form is not disposed.
                {
                    return;
                }

                if (project != null)
                {
                    UnsubscribeProjectEvents();
                }

                project = value as Project;
                TreeViewController.Data = project;

                if (project != null)
                {
                    SubscribeProjectEvents();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            applicationSelection.SelectionChanged -= GuiSelectionChanged;

            if (project != null)
            {
                UnsubscribeProjectEvents();
            }

            TreeViewController.Data = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Update selected node when selection in gui changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (selectingNode || applicationSelection.Selection == null
                || (SelectedNode != null && SelectedNode.Tag == applicationSelection.Selection))
            {
                return;
            }

            TreeNode node = TreeViewController.GetNodeByTag(applicationSelection.Selection);
            if (node != null)
            {
                SelectedNode = node;
            }
        }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            selectingNode = true;

            var tag = SelectedNode != null ? SelectedNode.Tag : null;

            applicationSelection.Selection = tag;

            selectingNode = false;
        }

        private void SubscribeProjectEvents()
        {
            projectOwner.ProjectOpened += ApplicationCoreProjectOpened;
        }

        private void UnsubscribeProjectEvents()
        {
            projectOwner.ProjectOpened -= ApplicationCoreProjectOpened;
        }

        private void ApplicationCoreProjectOpened(Project project)
        {
            Project = project;
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            viewCommands.OpenViewForSelection();
        }

        private void ProjectDataDeleted(object sender, TreeNodeDataDeletedEventArgs e)
        {
            viewCommands.RemoveAllViewsForItem(e.DeletedDataInstance);
        }
    }
}