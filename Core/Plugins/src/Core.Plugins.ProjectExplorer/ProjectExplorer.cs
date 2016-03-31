﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Selection;
using Core.Common.Utils.Events;
using Core.Plugins.ProjectExplorer.Properties;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// This class describes a Project Explorer, which can be used to navigate and open views for elements
    /// in the project.
    /// </summary>
    public sealed partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectExplorer"/>.
        /// </summary>
        /// <param name="applicationSelection">The owner of the selection in the application.</param>
        /// <param name="viewCommands">The provider of view related commands.</param>
        /// <param name="treeNodeInfos">The <see cref="IEnumerable{T}"/> of <see cref="TreeNodeInfo"/> which 
        /// are used to draw nodes.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="applicationSelection"/> is <c>null</c>,</item>
        /// <item><paramref name="viewCommands"/> is <c>null</c>,</item>
        /// <item><paramref name="treeNodeInfos"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        public ProjectExplorer(IApplicationSelection applicationSelection, IViewCommands viewCommands, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            if (applicationSelection == null)
            {
                throw new ArgumentNullException("applicationSelection");
            }
            if (viewCommands == null)
            {
                throw new ArgumentNullException("viewCommands");
            }
            if (treeNodeInfos == null)
            {
                throw new ArgumentNullException("treeNodeInfos");
            }
            InitializeComponent();

            Text = Resources.General_ProjectExplorer;

            this.applicationSelection = applicationSelection;
            this.viewCommands = viewCommands;

            RegisterTreeNodeInfos(treeNodeInfos);
            BindTreeInteractionEvents();
        }

        public object Data
        {
            get
            {
                return treeViewControl.Data;
            }
            set
            {
                if (!treeViewControl.IsDisposed)
                {
                    treeViewControl.Data = value;
                }
            }
        }

        public TreeViewControl TreeViewControl
        {
            get
            {
                return treeViewControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (treeViewControl != null)
            {
                Data = null;
                treeViewControl.Dispose();
            }

            base.Dispose(disposing);
        }

        private void BindTreeInteractionEvents()
        {
            treeViewControl.DataDoubleClick += TreeViewControlDataDoubleClick;
            treeViewControl.DataDeleted += TreeViewControlDataDeleted;
            treeViewControl.SelectedDataChanged += TreeViewControlSelectedDataChanged;
        }

        private void RegisterTreeNodeInfos(IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            foreach (TreeNodeInfo info in treeNodeInfos)
            {
                treeViewControl.RegisterTreeNodeInfo(info);
            }
        }

        private void TreeViewControlSelectedDataChanged(object sender, EventArgs e)
        {
            applicationSelection.Selection = treeViewControl.SelectedData;
        }

        private void TreeViewControlDataDoubleClick(object sender, EventArgs e)
        {
            viewCommands.OpenViewForSelection();
        }

        private void TreeViewControlDataDeleted(object sender, EventArgs<object> e)
        {
            viewCommands.RemoveAllViewsForItem(e.Value);
        }
    }
}