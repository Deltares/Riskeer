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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Selection;
using Core.Plugins.ProjectExplorer.Properties;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// This class is responsible for showing and hiding a <see cref="ProjectExplorer"/>.
    /// </summary>
    public class ProjectExplorerViewController : IDisposable
    {
        /// <summary>
        /// Fired when the project explorer view has been opened.
        /// </summary>
        public event EventHandler<EventArgs> OnOpenView;

        private readonly IViewController viewController;
        private readonly IEnumerable<TreeNodeInfo> treeNodeInfos;
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;

        private ProjectExplorer projectExplorer;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectExplorerViewController"/>.
        /// </summary>
        /// <param name="viewCommands">The provider of view related commands.</param>
        /// <param name="applicationSelection">The owner of the selection in the application.</param>
        /// <param name="viewController">The provider of view related commands.</param>
        /// <param name="treeNodeInfos">The <see cref="IEnumerable{T}"/> of <see cref="TreeNodeInfo"/> which 
        /// are used to draw nodes.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="viewCommands"/> is <c>null</c>,</item>
        /// <item><paramref name="applicationSelection"/> is <c>null</c>,</item>
        /// <item><paramref name="viewController"/> is <c>null</c>,</item>
        /// <item><paramref name="treeNodeInfos"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        public ProjectExplorerViewController(IViewCommands viewCommands, IApplicationSelection applicationSelection, IViewController viewController, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException("viewCommands");
            }
            if (applicationSelection == null)
            {
                throw new ArgumentNullException("applicationSelection");
            }
            if (viewController == null)
            {
                throw new ArgumentNullException("viewController");
            }
            if (treeNodeInfos == null)
            {
                throw new ArgumentNullException("treeNodeInfos");
            }

            this.viewController = viewController;
            this.applicationSelection = applicationSelection;
            this.treeNodeInfos = treeNodeInfos;
            this.viewCommands = viewCommands;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ProjectExplorer"/> is visible.
        /// </summary>
        public bool IsProjectExplorerOpen
        {
            get
            {
                return viewController.ViewHost.ToolViews.Contains(projectExplorer);
            }
        }

        /// <summary>
        /// Toggles the visibility of the <see cref="ProjectExplorer"/>.
        /// </summary>
        public void ToggleView()
        {
            if (IsProjectExplorerOpen)
            {
                CloseProjectExplorer();
            }
            else
            {
                OpenProjectExplorer();
            }
        }

        /// <summary>
        /// Updates the <see cref="ProjectExplorer"/> with a <paramref name="project"/>.
        /// </summary>
        /// <param name="project">The <see cref="IProject"/> to set.</param>
        public void Update(IProject project)
        {
            if (IsProjectExplorerOpen)
            {
                projectExplorer.Data = project;
            }
        }

        public void Dispose()
        {
            if (projectExplorer != null)
            {
                if (IsProjectExplorerOpen)
                {
                    CloseProjectExplorer();
                }
                projectExplorer.Dispose();
            }
        }

        private void OpenProjectExplorer()
        {
            projectExplorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos);

            viewController.ViewHost.AddToolView(projectExplorer, ToolViewLocation.Left);
            viewController.ViewHost.SetImage(projectExplorer, Resources.ProjectExplorerIcon);

            if (OnOpenView != null)
            {
                OnOpenView(this, EventArgs.Empty);
            }
        }

        private void CloseProjectExplorer()
        {
            viewController.ViewHost.Remove(projectExplorer);
        }
    }
}