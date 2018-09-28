// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Plugins.ProjectExplorer.Commands;
using Core.Plugins.ProjectExplorer.Exceptions;
using ProjectExplorerResources = Core.Plugins.ProjectExplorer.Properties.Resources;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// The plug-in for the <see cref="ProjectExplorer"/> component.
    /// </summary>
    public class ProjectExplorerPlugin : PluginBase
    {
        private IViewController viewController;
        private ProjectExplorerViewController projectExplorerViewController;
        private IViewCommands viewCommands;
        private IProjectOwner projectOwner;
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
                    viewController = value;
                    projectOwner = value;
                    viewCommands = value.ViewCommands;
                    treeNodeInfos = value.GetTreeNodeInfos();
                }
                else
                {
                    viewController = null;
                    projectOwner = null;
                    viewCommands = null;
                    treeNodeInfos = null;
                }
            }
        }

        /// <summary>
        /// Activates the <see cref="ProjectExplorerPlugin"/>
        /// </summary>
        /// <exception cref="PluginActivationException">Thrown when <see cref="Gui"/> is <c>null</c>.</exception>
        public override void Activate()
        {
            if (active)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               ProjectExplorerResources.ProjectExplorerPlugin_Cannot_activate_0_twice,
                                               ProjectExplorerResources.General_ProjectExplorer);
                throw new PluginActivationException(message);
            }

            base.Activate();
            try
            {
                projectExplorerViewController = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos);
            }
            catch (ArgumentNullException e)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               ProjectExplorerResources.ProjectExplorerPlugin_Activation_of_0_failed,
                                               ProjectExplorerResources.General_ProjectExplorer);
                throw new PluginActivationException(message, e);
            }

            ribbonCommandHandler = new Ribbon
            {
                ToggleExplorerCommand = new ToggleProjectExplorerCommand(projectExplorerViewController)
            };

            projectExplorerViewController.OnOpenView += (s, e) => UpdateProject();
            projectExplorerViewController.ToggleView();

            projectOwner.ProjectOpened += ApplicationProjectOpened;
            active = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (active)
            {
                projectOwner.ProjectOpened -= ApplicationProjectOpened;
                projectExplorerViewController.Dispose();
                active = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Deactivate();
            }

            base.Dispose(disposing);
        }

        private void ApplicationProjectOpened(IProject project)
        {
            UpdateProject();
        }

        private void UpdateProject()
        {
            projectExplorerViewController.Update(projectOwner.Project);
        }
    }
}