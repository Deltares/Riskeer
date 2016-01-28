﻿using System;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Plugins.ProjectExplorer.Properties;
using log4net;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectExplorer));
        private readonly IDocumentViewController documentViewController;
        private readonly IViewCommands viewCommands;

        public ProjectExplorer()
        {
            InitializeComponent();
        }

        public ProjectExplorer(IGui gui)
        {
            documentViewController = gui;
            viewCommands = gui.ViewCommands;

            InitializeComponent();
            ProjectTreeView = new ProjectTreeView(gui, viewCommands, gui, documentViewController)
            {
                Dock = DockStyle.Fill
            };
            treeViewPanel.Controls.Add(ProjectTreeView);

            documentViewController.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
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

        public new void Dispose()
        {
            ProjectTreeView.Data = null;
            if (documentViewController != null && documentViewController.DocumentViews != null)
            {
                documentViewController.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
            }

            ProjectTreeView.Dispose();
            base.Dispose();
        }

        public TreeView TreeView
        {
            get
            {
                return ProjectTreeView.TreeView;
            }
        }

        public void ScrollTo(object o)
        {
            var nodeToSelect = TreeView.GetNodeByTag(o);

            if (nodeToSelect == null)
            {
                log.DebugFormat(Resources.ProjectExplorer_ScrollTo_Can_t_find_tree_node_for_item_0_, o);
                return;
            }

            TreeView.SelectedNode = nodeToSelect;
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            buttonScrollToItemInActiveView.Enabled = documentViewController.DocumentViews.ActiveView != null &&
                                                     viewCommands.GetDataOfActiveView() != null;
        }

        private void ButtonScrollToItemInActiveViewClick(object sender, EventArgs e)
        {
            var dataOfActiveView = viewCommands.GetDataOfActiveView();
            if (dataOfActiveView != null)
            {
                ScrollTo(dataOfActiveView);

                return;
            }

            log.WarnFormat(Resources.ProjectExplorer_ButtonScrollToItemInActiveViewClick_Can_t_find_project_item_for_view_data_0_, documentViewController.DocumentViews.ActiveView.Data);
        }
    }
}