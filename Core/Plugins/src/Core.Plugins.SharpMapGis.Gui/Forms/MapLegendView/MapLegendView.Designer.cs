using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.TreeViewControls;

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
            this.buttonAddLayer = new System.Windows.Forms.ToolStripButton();
            this.buttonAddWmsLayer = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveLayer = new System.Windows.Forms.ToolStripButton();
            this.contextMenuLayer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuLayerOpenAttributeTable = new ClonableToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuLayerDelete = new ClonableToolStripMenuItem();
            this.contextMenuLayerRename = new ClonableToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.orderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.bringForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToLayerToolStripMenuItem1 = new ClonableToolStripMenuItem();
            this.zoomToMapExtentsToolStripMenuItem1 = new ClonableToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showLabelsToolStripMenuItem = new ClonableToolStripMenuItem();
            this.showInLegendToolStripMenuItem = new ClonableToolStripMenuItem();
            this.hideAllButThisOneToolStripMenuItem = new ClonableToolStripMenuItem();
            this.TreeView = new TreeView();
            this.contextMenuWmsLayer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zoomToLayerToolStripMenuItem = new ClonableToolStripMenuItem();
            this.contextMenuMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLayerToolStripMenuItem = new ClonableToolStripMenuItem();
            this.addLayergroupToolStripMenuItem = new ClonableToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToExtentsToolStripMenuItem = new ClonableToolStripMenuItem();
            this.zoomToMapExtentsToolStripMenuItem = new ClonableToolStripMenuItem();
            this.changeCoordinateSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.contextMenuLayer.SuspendLayout();
            this.contextMenuWmsLayer.SuspendLayout();
            this.contextMenuMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddWmsLayer,
            this.buttonAddLayer,
            this.buttonRemoveLayer});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(263, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonAddLayer
            // 
            this.buttonAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddLayer.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddLayer.Image")));
            this.buttonAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddLayer.Name = "buttonAddLayer";
            this.buttonAddLayer.Size = new System.Drawing.Size(23, 22);
            this.buttonAddLayer.Tag = "AddNewLayer";
            this.buttonAddLayer.Text = "Add New layer ...";
            this.buttonAddLayer.ToolTipText = "Add New Layer ...";
            this.buttonAddLayer.Click += new System.EventHandler(this.ButtonAddLayerClick);
            // 
            // buttonAddWmsLayer
            // 
            this.buttonAddWmsLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddWmsLayer.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddWmsLayer.Image")));
            this.buttonAddWmsLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddWmsLayer.Name = "buttonAddWmsLayer";
            this.buttonAddWmsLayer.Size = new System.Drawing.Size(23, 22);
            this.buttonAddWmsLayer.Tag = "AddNewWmsLayer";
            this.buttonAddWmsLayer.Text = "Add New Wms Layer ...";
            this.buttonAddWmsLayer.ToolTipText = "Add New Wms Layer ...";
            this.buttonAddWmsLayer.Click += new System.EventHandler(this.ButtonAddWmsLayerClick);
            // 
            // buttonRemoveLayer
            // 
            this.buttonRemoveLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveLayer.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveLayer.Image")));
            this.buttonRemoveLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveLayer.Name = "buttonRemoveLayer";
            this.buttonRemoveLayer.Size = new System.Drawing.Size(23, 22);
            this.buttonRemoveLayer.Tag = "RemoveSelectedLayer";
            this.buttonRemoveLayer.Text = "Remove Selected Layer";
            this.buttonRemoveLayer.ToolTipText = "Remove Selected Layer";
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
            this.contextMenuLayer.Size = new System.Drawing.Size(257, 220);
            this.contextMenuLayer.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuLayerOpening);
            this.contextMenuLayer.VisibleChanged += new System.EventHandler(this.ContextMenuLayerVisibleChanged);
            // 
            // contextMenuLayerOpenAttributeTable
            // 
            this.contextMenuLayerOpenAttributeTable.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.table;
            this.contextMenuLayerOpenAttributeTable.Name = "contextMenuLayerOpenAttributeTable";
            this.contextMenuLayerOpenAttributeTable.Size = new System.Drawing.Size(256, 22);
            this.contextMenuLayerOpenAttributeTable.Text = "Open Attribute Table";
            this.contextMenuLayerOpenAttributeTable.Click += new System.EventHandler(this.ContextMenuOpenLayerAttributeTableClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(253, 6);
            // 
            // contextMenuLayerDelete
            // 
            this.contextMenuLayerDelete.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.DeleteHS;
            this.contextMenuLayerDelete.Name = "contextMenuLayerDelete";
            this.contextMenuLayerDelete.Size = new System.Drawing.Size(256, 22);
            this.contextMenuLayerDelete.Text = "&Delete";
            this.contextMenuLayerDelete.Click += new System.EventHandler(this.ContextMenuLayerDeleteClick);
            // 
            // contextMenuLayerRename
            // 
            this.contextMenuLayerRename.Name = "contextMenuLayerRename";
            this.contextMenuLayerRename.Size = new System.Drawing.Size(256, 22);
            this.contextMenuLayerRename.Text = "&Rename";
            this.contextMenuLayerRename.Click += new System.EventHandler(this.ContextMenuLayerRenameClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(253, 6);
            // 
            // orderToolStripMenuItem
            // 
            this.orderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bringToFrontToolStripMenuItem,
            this.sendToBackToolStripMenuItem,
            this.toolStripSeparator6,
            this.bringForwardToolStripMenuItem,
            this.sendBackwardToolStripMenuItem});
            this.orderToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.layers_stack;
            this.orderToolStripMenuItem.Name = "orderToolStripMenuItem";
            this.orderToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.orderToolStripMenuItem.Text = "Order";
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.bringToFrontToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.layers_stack_arrange;
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            this.bringToFrontToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.bringToFrontToolStripMenuItem.Text = "Bring to front";
            this.bringToFrontToolStripMenuItem.Click += new System.EventHandler(this.BringToFrontToolStripMenuItemClick);
            // 
            // sendToBackToolStripMenuItem
            // 
            this.sendToBackToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.layers_stack_arrange_back;
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            this.sendToBackToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.sendToBackToolStripMenuItem.Text = "Send to back";
            this.sendToBackToolStripMenuItem.Click += new System.EventHandler(this.SendToBackToolStripMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(151, 6);
            // 
            // bringForwardToolStripMenuItem
            // 
            this.bringForwardToolStripMenuItem.Name = "bringForwardToolStripMenuItem";
            this.bringForwardToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.bringForwardToolStripMenuItem.Text = "Bring forward";
            this.bringForwardToolStripMenuItem.Click += new System.EventHandler(this.BringForwardToolStripMenuItemClick);
            // 
            // sendBackwardToolStripMenuItem
            // 
            this.sendBackwardToolStripMenuItem.Name = "sendBackwardToolStripMenuItem";
            this.sendBackwardToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.sendBackwardToolStripMenuItem.Text = "Send backward";
            this.sendBackwardToolStripMenuItem.Click += new System.EventHandler(this.SendBackwardToolStripMenuItemClick);
            // 
            // zoomToLayerToolStripMenuItem1
            // 
            this.zoomToLayerToolStripMenuItem1.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.MapZoomToExtentsImage;
            this.zoomToLayerToolStripMenuItem1.Name = "zoomToLayerToolStripMenuItem1";
            this.zoomToLayerToolStripMenuItem1.Size = new System.Drawing.Size(256, 22);
            this.zoomToLayerToolStripMenuItem1.Text = "&Zoom to Extent";
            this.zoomToLayerToolStripMenuItem1.Click += new System.EventHandler(this.ZoomToLayerToolStripMenuItem1Click);
            // 
            // zoomToMapExtentsToolStripMenuItem1
            // 
            this.zoomToMapExtentsToolStripMenuItem1.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.layers_ungroup;
            this.zoomToMapExtentsToolStripMenuItem1.Name = "zoomToMapExtentsToolStripMenuItem1";
            this.zoomToMapExtentsToolStripMenuItem1.Size = new System.Drawing.Size(256, 22);
            this.zoomToMapExtentsToolStripMenuItem1.Text = "Synchronize Zoom Level with Map";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(253, 6);
            // 
            // showLabelsToolStripMenuItem
            // 
            this.showLabelsToolStripMenuItem.CheckOnClick = true;
            this.showLabelsToolStripMenuItem.Name = "showLabelsToolStripMenuItem";
            this.showLabelsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.showLabelsToolStripMenuItem.Text = "Show Labels";
            this.showLabelsToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.ShowLabelsToolStripMenuItemCheckStateChanged);
            // 
            // showInLegendToolStripMenuItem
            // 
            this.showInLegendToolStripMenuItem.Checked = true;
            this.showInLegendToolStripMenuItem.CheckOnClick = true;
            this.showInLegendToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showInLegendToolStripMenuItem.Name = "showInLegendToolStripMenuItem";
            this.showInLegendToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.showInLegendToolStripMenuItem.Text = "Show in Legend";
            this.showInLegendToolStripMenuItem.Click += new System.EventHandler(this.ShowInLegendToolStripMenuItemClick);
            // 
            // hideAllButThisOneToolStripMenuItem
            // 
            this.hideAllButThisOneToolStripMenuItem.Name = "hideAllButThisOneToolStripMenuItem";
            this.hideAllButThisOneToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.hideAllButThisOneToolStripMenuItem.Text = "Hide All Layers but this one";
            this.hideAllButThisOneToolStripMenuItem.Click += new System.EventHandler(this.HideAllButThisOneToolStripMenuItemClick);
            // 
            // TreeView
            // 
            this.TreeView.AllowDrop = true;
            this.TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.TreeView.HideSelection = false;
            this.TreeView.ImageIndex = 0;
            this.TreeView.LabelEdit = true;
            this.TreeView.Location = new System.Drawing.Point(0, 25);
            this.TreeView.Name = "TreeView";
            this.TreeView.SelectedImageIndex = 0;
            this.TreeView.SelectNodeOnRightMouseClick = true;
            this.TreeView.Size = new System.Drawing.Size(263, 363);
            this.TreeView.TabIndex = 4;
            // 
            // contextMenuWmsLayer
            // 
            this.contextMenuWmsLayer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToLayerToolStripMenuItem});
            this.contextMenuWmsLayer.Name = "contextMenuLayer";
            this.contextMenuWmsLayer.Size = new System.Drawing.Size(152, 26);
            // 
            // zoomToLayerToolStripMenuItem
            // 
            this.zoomToLayerToolStripMenuItem.Name = "zoomToLayerToolStripMenuItem";
            this.zoomToLayerToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.zoomToLayerToolStripMenuItem.Text = "Zoom to Layer";
            this.zoomToLayerToolStripMenuItem.Click += new System.EventHandler(this.ZoomToLayerToolStripMenuItemClick);
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
            this.contextMenuMap.Size = new System.Drawing.Size(258, 142);
            this.contextMenuMap.VisibleChanged += new System.EventHandler(this.ContextMenuMapVisibleChanged);
            // 
            // addLayerToolStripMenuItem
            // 
            this.addLayerToolStripMenuItem.Name = "addLayerToolStripMenuItem";
            this.addLayerToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.addLayerToolStripMenuItem.Text = "Add Layer";
            this.addLayerToolStripMenuItem.Click += new System.EventHandler(this.AddLayerToolStripMenuItemClick);
            // 
            // addLayergroupToolStripMenuItem
            // 
            this.addLayergroupToolStripMenuItem.Name = "addLayergroupToolStripMenuItem";
            this.addLayergroupToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.addLayergroupToolStripMenuItem.Text = "Add Layer Group";
            this.addLayergroupToolStripMenuItem.Click += new System.EventHandler(this.AddLayergroupToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(254, 6);
            // 
            // zoomToExtentsToolStripMenuItem
            // 
            this.zoomToExtentsToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.MapZoomToExtentsImage;
            this.zoomToExtentsToolStripMenuItem.Name = "zoomToExtentsToolStripMenuItem";
            this.zoomToExtentsToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.zoomToExtentsToolStripMenuItem.Text = "Zoom to Extents";
            this.zoomToExtentsToolStripMenuItem.Click += new System.EventHandler(this.ZoomToExtentsToolStripMenuItemClick);
            // 
            // zoomToMapExtentsToolStripMenuItem
            // 
            this.zoomToMapExtentsToolStripMenuItem.Name = "zoomToMapExtentsToolStripMenuItem";
            this.zoomToMapExtentsToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.zoomToMapExtentsToolStripMenuItem.Text = "Zoom to Map";
            // 
            // changeCoordinateSystemToolStripMenuItem
            // 
            this.changeCoordinateSystemToolStripMenuItem.Image = global::Core.Plugins.SharpMapGis.Gui.Properties.Resources.globe__pencil;
            this.changeCoordinateSystemToolStripMenuItem.Name = "changeCoordinateSystemToolStripMenuItem";
            this.changeCoordinateSystemToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.changeCoordinateSystemToolStripMenuItem.Text = "Change Map Coordinate System ...";
            this.changeCoordinateSystemToolStripMenuItem.Click += new System.EventHandler(this.ChangeCoordinateSystemToolStripMenuItem_Click);
            // 
            // MapLegendView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TreeView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MapLegendView";
            this.Size = new System.Drawing.Size(263, 388);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuLayer.ResumeLayout(false);
            this.contextMenuWmsLayer.ResumeLayout(false);
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
        private ClonableToolStripMenuItem contextMenuLayerDelete;
        private ClonableToolStripMenuItem contextMenuLayerRename;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private ClonableToolStripMenuItem contextMenuLayerOpenAttributeTable;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton buttonAddWmsLayer;
        private System.Windows.Forms.ContextMenuStrip contextMenuWmsLayer;
        private ClonableToolStripMenuItem zoomToLayerToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuMap;
        private ClonableToolStripMenuItem addLayerToolStripMenuItem;
        private ClonableToolStripMenuItem zoomToLayerToolStripMenuItem1;
        private ClonableToolStripMenuItem zoomToExtentsToolStripMenuItem;
        private ClonableToolStripMenuItem showLabelsToolStripMenuItem;
        private ClonableToolStripMenuItem showInLegendToolStripMenuItem;
        private ClonableToolStripMenuItem hideAllButThisOneToolStripMenuItem;
        private ClonableToolStripMenuItem addLayergroupToolStripMenuItem;
        private ClonableToolStripMenuItem zoomToMapExtentsToolStripMenuItem;
        private ClonableToolStripMenuItem zoomToMapExtentsToolStripMenuItem1;
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