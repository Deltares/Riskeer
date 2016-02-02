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
    public class ProjectExplorerViewController : IDisposable
    {
        private readonly IToolViewController toolViewController;
        private ProjectExplorer projectExplorer;
        private readonly IEnumerable<TreeNodeInfo> treeNodeInfos;
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;
        private readonly IDocumentViewController documentViewController;

        public EventHandler<EventArgs> OnOpenView;

        public ProjectExplorerViewController(IDocumentViewController documentViewController, IViewCommands viewCommands, IApplicationSelection applicationSelection, IToolViewController toolViewController, IEnumerable<TreeNodeInfo> treeNodeInfos)
        {
            this.toolViewController = toolViewController;
            this.applicationSelection = applicationSelection;
            this.treeNodeInfos = treeNodeInfos;
            this.viewCommands = viewCommands;
            this.documentViewController = documentViewController;
        }

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

        public void OpenView()
        {
            projectExplorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos);
            projectExplorer.TreeView.TreeViewController.NodeUpdated += (s, e) => documentViewController.UpdateToolTips();

            toolViewController.ToolWindowViews.Add(projectExplorer, ViewLocation.Left | ViewLocation.Top);

            if (OnOpenView != null)
            {
                OnOpenView(this, EventArgs.Empty);
            }
        }

        private void CloseView()
        {
            toolViewController.ToolWindowViews.Remove(projectExplorer); // Disposes the view.
            projectExplorer = null;
        }

        public bool IsViewActive()
        {
            return toolViewController.IsToolWindowOpen<ProjectExplorer>();
        }

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
    }
}