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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// This class is responsible for showing and hiding a <see cref="ProjectExplorer"/>.
    /// </summary>
    public class ProjectExplorerViewController : IDisposable
    {
        private readonly IToolViewController toolViewController;
        private readonly IEnumerable<TreeNodeInfo> treeNodeInfos;
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;
        private readonly IDocumentViewController documentViewController;

        public EventHandler<EventArgs> OnOpenView;
        private ProjectExplorer projectExplorer;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectExplorerViewController"/>.
        /// </summary>
        /// <param name="documentViewController">The provider of document view related commands.</param>
        /// <param name="viewCommands">The provider of view related commands.</param>
        /// <param name="applicationSelection">The owner of the selection in the application.</param>
        /// <param name="toolViewController">The provider of tool view related commands.</param>
        /// <param name="treeNodeInfos">The <see cref="IEnumerable{T}"/> of <see cref="TreeNodeInfo"/> which 
        /// are used to draw nodes.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="documentViewController"/> is <c>null</c>,</item>
        /// <item><paramref name="viewCommands"/> is <c>null</c>,</item>
        /// <item><paramref name="applicationSelection"/> is <c>null</c>,</item>
        /// <item><paramref name="toolViewController"/> is <c>null</c>,</item>
        /// <item><paramref name="treeNodeInfos"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        public ProjectExplorerViewController(IDocumentViewController documentViewController, IViewCommands viewCommands, IApplicationSelection applicationSelection, IToolViewController toolViewController, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            if (documentViewController == null)
            {
                throw new ArgumentNullException("documentViewController");
            }
            if (viewCommands == null)
            {
                throw new ArgumentNullException("viewCommands");
            }
            if (applicationSelection == null)
            {
                throw new ArgumentNullException("applicationSelection");
            }
            if (toolViewController == null)
            {
                throw new ArgumentNullException("toolViewController");
            }
            if (treeNodeInfos == null)
            {
                throw new ArgumentNullException("treeNodeInfos");
            }

            this.toolViewController = toolViewController;
            this.applicationSelection = applicationSelection;
            this.treeNodeInfos = treeNodeInfos;
            this.viewCommands = viewCommands;
            this.documentViewController = documentViewController;
        }

        /// <summary>
        /// Toggles the visibility of the <see cref="ProjectExplorer"/>.
        /// </summary>
        public void ToggleView()
        {
            if (IsViewActive())
            {
                CloseView();
            }
            else
            {
                OpenView();
            }
        }

        /// <summary>
        /// Makes the <see cref="ProjectExplorer"/> visisble.
        /// </summary>
        public void OpenView()
        {
            if (!IsViewActive())
            {
                projectExplorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos);
                projectExplorer.TreeViewControl.NodeUpdated += (s, e) => documentViewController.UpdateToolTips();

                toolViewController.OpenToolView(projectExplorer);

                if (OnOpenView != null)
                {
                    OnOpenView(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="ProjectExplorer"/> is visible.
        /// </summary>
        /// <returns></returns>
        public bool IsViewActive()
        {
            return toolViewController.IsToolWindowOpen<ProjectExplorer>();
        }

        /// <summary>
        /// Updates the <see cref="ProjectExplorer"/> with a <paramref name="project"/>.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to set.</param>
        public void Update(Project project)
        {
            if (IsViewActive())
            {
                projectExplorer.Data = project;
            }
        }

        public void Dispose()
        {
            CloseView();
        }

        private void CloseView()
        {
            if (IsViewActive())
            {
                toolViewController.CloseToolView(projectExplorer); // Disposes the view.
                projectExplorer = null;
            }
        }
    }
}