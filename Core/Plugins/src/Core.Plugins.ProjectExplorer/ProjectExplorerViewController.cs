using System;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewManager;
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
        public ProjectExplorerViewController(IDocumentViewController documentViewController, IViewCommands viewCommands, IApplicationSelection applicationSelection, IToolViewController toolViewController, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
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
                projectExplorer.TreeView.TreeViewController.NodeUpdated += (s, e) => documentViewController.UpdateToolTips();

                toolViewController.ToolWindowViews.Add(projectExplorer, ViewLocation.Left | ViewLocation.Top);

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
            if (IsViewActive())
            {
                CloseView();
            }
        }

        private void CloseView()
        {
            if (IsViewActive())
            {
                toolViewController.ToolWindowViews.Remove(projectExplorer); // Disposes the view.
                projectExplorer = null;
            }
        }
    }
}