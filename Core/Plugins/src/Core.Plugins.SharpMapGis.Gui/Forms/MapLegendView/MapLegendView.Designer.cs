using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.TreeViewControls;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    partial class MapLegendView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapLegendView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonAddWmsLayer = new System.Windows.Forms.ToolStripButton();
            this.buttonAddLayer = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveLayer = new System.Windows.Forms.ToolStripButton();
            this.contextMenuLayer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuLayerOpenAttributeTable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuLayerDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuLayerRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.orderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.bringForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToLayerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToMapExtentsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideAllButThisOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TreeView = new Core.Common.Controls.Swf.TreeViewControls.TreeView();
            this.contextMenuMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayergroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToExtentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToMapExtentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeCoordinateSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.contextMenuLayer.SuspendLayout();
            this.contextMenuMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddWmsLayer,
            this.buttonAddLayer,
            this.buttonRemoveLayer});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // buttonAddWmsLayer
            // 
            this.buttonAddWmsLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.buttonAddWmsLayer, "buttonAddWmsLayer");
            this.buttonAddWmsLayer.Name = "buttonAddWmsLayer";
            this.buttonAddWmsLayer.Tag = "AddNewWmsLayer";
            this.buttonAddWmsLayer.Click += new System.EventHandler(this.ButtonAddWmsLayerClick);
            // 
            // buttonAddLayer
            // 
            this.buttonAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.buttonAddLayer, "buttonAddLayer");
            this.buttonAddLayer.Name = "buttonAddLayer";
            this.buttonAddLayer.Tag = "AddNewLayer";
            this.buttonAddLayer.Click += new System.EventHandler(this.ButtonAddLayerClick);
            // 
            // buttonRemoveLayer
            // 
            this.buttonRemoveLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.buttonRemoveLayer, "buttonRemoveLayer");
            this.buttonRemoveLayer.Name = "buttonRemoveLayer";
            this.buttonRemoveLayer.Tag = "RemoveSelectedLayer";
            this.buttonRemoveLayer.Click += new System.EventHandler(this.ButtonRemoveLayerClick);
            // 
            // contextMenuLayer
            // 
            this.contextMenuLayer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuLayerOpenAttributeTable,
            this.toolStripMenuItem2,
            this.contextMenuLayerDelete,
            this.contextMenuLayerRename,
            this.toolStripMenuItem1,
            this.orderToolStripMenuItem,
            this.zoomToLayerToolStripMenuItem1,
            this.zoomToMapExtentsToolStripMenuItem1,
            this.toolStripSeparator2,
            this.showLabelsToolStripMenuItem,
            this.showInLegendToolStripMenuItem,
            this.hideAllButThisOneToolStripMenuItem});
            this.contextMenuLayer.Name = "contextMenuLayer";
            resources.ApplyResources(this.contextMenuLayer, "contextMenuLayer");
            this.contextMenuLayer.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuLayerOpening);
            this.contextMenuLayer.VisibleChanged += new System.EventHandler(this.ContextMenuLayerVisibleChanged);
            // 
            // contextMenuLayerOpenAttributeTable
            // 
            this.contextMenuLayerOpenAttributeTable.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.Table;
            this.contextMenuLayerOpenAttributeTable.Name = "contextMenuLayerOpenAttributeTable";
            resources.ApplyResources(this.contextMenuLayerOpenAttributeTable, "contextMenuLayerOpenAttributeTable");
            this.contextMenuLayerOpenAttributeTable.Click += new System.EventHandler(this.ContextMenuOpenLayerAttributeTableClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // contextMenuLayerDelete
            // 
            this.contextMenuLayerDelete.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.DeleteHS;
            this.contextMenuLayerDelete.Name = "contextMenuLayerDelete";
            resources.ApplyResources(this.contextMenuLayerDelete, "contextMenuLayerDelete");
            this.contextMenuLayerDelete.Click += new System.EventHandler(this.ContextMenuLayerDeleteClick);
            // 
            // contextMenuLayerRename
            // 
            this.contextMenuLayerRename.Name = "contextMenuLayerRename";
            resources.ApplyResources(this.contextMenuLayerRename, "contextMenuLayerRename");
            this.contextMenuLayerRename.Click += new System.EventHandler(this.ContextMenuLayerRenameClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // orderToolStripMenuItem
            // 
            this.orderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bringToFrontToolStripMenuItem,
            this.sendToBackToolStripMenuItem,
            this.toolStripSeparator6,
            this.bringForwardToolStripMenuItem,
            this.sendBackwardToolStripMenuItem});
            this.orderToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.LayersStack;
            this.orderToolStripMenuItem.Name = "orderToolStripMenuItem";
            resources.ApplyResources(this.orderToolStripMenuItem, "orderToolStripMenuItem");
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.bringToFrontToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.LayersStackArrange;
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            resources.ApplyResources(this.bringToFrontToolStripMenuItem, "bringToFrontToolStripMenuItem");
            this.bringToFrontToolStripMenuItem.Click += new System.EventHandler(this.BringToFrontToolStripMenuItemClick);
            // 
            // sendToBackToolStripMenuItem
            // 
            this.sendToBackToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.LayersStackArrangeBack;
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            resources.ApplyResources(this.sendToBackToolStripMenuItem, "sendToBackToolStripMenuItem");
            this.sendToBackToolStripMenuItem.Click += new System.EventHandler(this.SendToBackToolStripMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // bringForwardToolStripMenuItem
            // 
            this.bringForwardToolStripMenuItem.Name = "bringForwardToolStripMenuItem";
            resources.ApplyResources(this.bringForwardToolStripMenuItem, "bringForwardToolStripMenuItem");
            this.bringForwardToolStripMenuItem.Click += new System.EventHandler(this.BringForwardToolStripMenuItemClick);
            // 
            // sendBackwardToolStripMenuItem
            // 
            this.sendBackwardToolStripMenuItem.Name = "sendBackwardToolStripMenuItem";
            resources.ApplyResources(this.sendBackwardToolStripMenuItem, "sendBackwardToolStripMenuItem");
            this.sendBackwardToolStripMenuItem.Click += new System.EventHandler(this.SendBackwardToolStripMenuItemClick);
            // 
            // zoomToLayerToolStripMenuItem1
            // 
            this.zoomToLayerToolStripMenuItem1.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.MapZoomToExtentsImage;
            this.zoomToLayerToolStripMenuItem1.Name = "zoomToLayerToolStripMenuItem1";
            resources.ApplyResources(this.zoomToLayerToolStripMenuItem1, "zoomToLayerToolStripMenuItem1");
            this.zoomToLayerToolStripMenuItem1.Click += new System.EventHandler(this.ZoomToLayerToolStripMenuItem1Click);
            // 
            // zoomToMapExtentsToolStripMenuItem1
            // 
            this.zoomToMapExtentsToolStripMenuItem1.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.LayersUngroup;
            this.zoomToMapExtentsToolStripMenuItem1.Name = "zoomToMapExtentsToolStripMenuItem1";
            resources.ApplyResources(this.zoomToMapExtentsToolStripMenuItem1, "zoomToMapExtentsToolStripMenuItem1");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // showLabelsToolStripMenuItem
            // 
            this.showLabelsToolStripMenuItem.CheckOnClick = true;
            this.showLabelsToolStripMenuItem.Name = "showLabelsToolStripMenuItem";
            resources.ApplyResources(this.showLabelsToolStripMenuItem, "showLabelsToolStripMenuItem");
            this.showLabelsToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.ShowLabelsToolStripMenuItemCheckStateChanged);
            // 
            // showInLegendToolStripMenuItem
            // 
            this.showInLegendToolStripMenuItem.Checked = true;
            this.showInLegendToolStripMenuItem.CheckOnClick = true;
            this.showInLegendToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showInLegendToolStripMenuItem.Name = "showInLegendToolStripMenuItem";
            resources.ApplyResources(this.showInLegendToolStripMenuItem, "showInLegendToolStripMenuItem");
            this.showInLegendToolStripMenuItem.Click += new System.EventHandler(this.ShowInLegendToolStripMenuItemClick);
            // 
            // hideAllButThisOneToolStripMenuItem
            // 
            this.hideAllButThisOneToolStripMenuItem.Name = "hideAllButThisOneToolStripMenuItem";
            resources.ApplyResources(this.hideAllButThisOneToolStripMenuItem, "hideAllButThisOneToolStripMenuItem");
            this.hideAllButThisOneToolStripMenuItem.Click += new System.EventHandler(this.HideAllButThisOneToolStripMenuItemClick);
            // 
            // TreeView
            // 
            this.TreeView.AllowDrop = true;
            resources.ApplyResources(this.TreeView, "TreeView");
            this.TreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.TreeView.HideSelection = false;
            this.TreeView.LabelEdit = true;
            this.TreeView.Name = "TreeView";
            this.TreeView.SelectNodeOnRightMouseClick = true;
            // 
            // contextMenuMap
            // 
            this.contextMenuMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem,
            this.addLayergroupToolStripMenuItem,
            this.toolStripSeparator4,
            this.zoomToExtentsToolStripMenuItem,
            this.zoomToMapExtentsToolStripMenuItem,
            this.changeCoordinateSystemToolStripMenuItem});
            this.contextMenuMap.Name = "contextMenuMap";
            resources.ApplyResources(this.contextMenuMap, "contextMenuMap");
            this.contextMenuMap.VisibleChanged += new System.EventHandler(this.ContextMenuMapVisibleChanged);
            // 
            // addLayerToolStripMenuItem
            // 
            this.addLayerToolStripMenuItem.Name = "addLayerToolStripMenuItem";
            resources.ApplyResources(this.addLayerToolStripMenuItem, "addLayerToolStripMenuItem");
            this.addLayerToolStripMenuItem.Click += new System.EventHandler(this.AddLayerToolStripMenuItemClick);
            // 
            // addLayergroupToolStripMenuItem
            // 
            this.addLayergroupToolStripMenuItem.Name = "addLayergroupToolStripMenuItem";
            resources.ApplyResources(this.addLayergroupToolStripMenuItem, "addLayergroupToolStripMenuItem");
            this.addLayergroupToolStripMenuItem.Click += new System.EventHandler(this.AddLayergroupToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // zoomToExtentsToolStripMenuItem
            // 
            this.zoomToExtentsToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.MapZoomToExtentsImage;
            this.zoomToExtentsToolStripMenuItem.Name = "zoomToExtentsToolStripMenuItem";
            resources.ApplyResources(this.zoomToExtentsToolStripMenuItem, "zoomToExtentsToolStripMenuItem");
            this.zoomToExtentsToolStripMenuItem.Click += new System.EventHandler(this.ZoomToExtentsToolStripMenuItemClick);
            // 
            // zoomToMapExtentsToolStripMenuItem
            // 
            this.zoomToMapExtentsToolStripMenuItem.Name = "zoomToMapExtentsToolStripMenuItem";
            resources.ApplyResources(this.zoomToMapExtentsToolStripMenuItem, "zoomToMapExtentsToolStripMenuItem");
            // 
            // changeCoordinateSystemToolStripMenuItem
            // 
            this.changeCoordinateSystemToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.GlobePencil;
            this.changeCoordinateSystemToolStripMenuItem.Name = "changeCoordinateSystemToolStripMenuItem";
            resources.ApplyResources(this.changeCoordinateSystemToolStripMenuItem, "changeCoordinateSystemToolStripMenuItem");
            this.changeCoordinateSystemToolStripMenuItem.Click += new System.EventHandler(this.ChangeCoordinateSystemToolStripMenuItem_Click);
            // 
            // MapLegendView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TreeView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MapLegendView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuLayer.ResumeLayout(false);
            this.contextMenuMap.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeView TreeView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonAddLayer;
        private System.Windows.Forms.ToolStripButton buttonRemoveLayer;
        private System.Windows.Forms.ContextMenuStrip contextMenuLayer;
        private ToolStripMenuItem contextMenuLayerDelete;
        private ToolStripMenuItem contextMenuLayerRename;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem contextMenuLayerOpenAttributeTable;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton buttonAddWmsLayer;
        private System.Windows.Forms.ContextMenuStrip contextMenuMap;
        private ToolStripMenuItem addLayerToolStripMenuItem;
        private ToolStripMenuItem zoomToLayerToolStripMenuItem1;
        private ToolStripMenuItem zoomToExtentsToolStripMenuItem;
        private ToolStripMenuItem showLabelsToolStripMenuItem;
        private ToolStripMenuItem showInLegendToolStripMenuItem;
        private ToolStripMenuItem hideAllButThisOneToolStripMenuItem;
        private ToolStripMenuItem addLayergroupToolStripMenuItem;
        private ToolStripMenuItem zoomToMapExtentsToolStripMenuItem;
        private ToolStripMenuItem zoomToMapExtentsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem orderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringToFrontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem sendBackwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bringForwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem changeCoordinateSystemToolStripMenuItem;
    }
}