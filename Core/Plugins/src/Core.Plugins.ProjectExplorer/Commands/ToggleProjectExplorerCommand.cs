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

using Core.Common.Controls.Commands;
using Core.Common.Gui;

namespace Core.Plugins.ProjectExplorer.Commands
{
    public class ToggleProjectExplorerCommand : ICommand
    {
        private readonly IToolViewController toolViewController;

        public ToggleProjectExplorerCommand(IToolViewController toolViewController)
        {
            this.toolViewController = toolViewController;
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                if (toolViewController == null || toolViewController.ToolWindowViews == null)
                {
                    return false;
                }

                return toolViewController.ToolWindowViews.Contains(ProjectExplorerGuiPlugin.Instance.ProjectExplorer);
            }
        }

        public void Execute(params object[] arguments)
        {
            var view = ProjectExplorerGuiPlugin.Instance.ProjectExplorer;
            var active = toolViewController.ToolWindowViews.Contains(view);

            if (active)
            {
                toolViewController.ToolWindowViews.Remove(view);
            }
            else
            {
                ProjectExplorerGuiPlugin.Instance.InitializeProjectTreeView();
            }
        }
    }
}