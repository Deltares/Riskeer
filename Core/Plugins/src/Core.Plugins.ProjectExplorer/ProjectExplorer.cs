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
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Selection;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        public ProjectExplorer(IApplicationSelection applicationSelection, IViewCommands viewCommands,
                               IProjectOwner projectOwner)
        {
            InitializeComponent();
            ProjectTreeView = new ProjectTreeView(applicationSelection, viewCommands, projectOwner);
            treeViewPanel.Controls.Add(ProjectTreeView);
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

        public TreeView TreeView
        {
            get
            {
                return ProjectTreeView;
            }
        }
    }
}