// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Windows.Forms;
using Core.Common.Gui.Forms.MessageWindow;

namespace Core.Common.Gui.Forms.MessageWindow
{
    partial class MessageWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageWindow));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClearAll = new System.Windows.Forms.ToolStripMenuItem();
            this.levelImages = new System.Windows.Forms.ImageList(this.components);
            this.messagesToolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonShowInfo = new System.Windows.Forms.ToolStripButton();
            this.buttonShowWarning = new System.Windows.Forms.ToolStripButton();
            this.buttonShowError = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonShowDetails = new System.Windows.Forms.ToolStripButton();
            this.messagesDataGridView = new System.Windows.Forms.DataGridView();
            this.levelColumnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.timeColumnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fullMessageColumnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messagesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.messageWindowData = new System.Data.DataSet();
            this.Messages = new System.Data.DataTable();
            this.levelColumn = new System.Data.DataColumn();
            this.timeColumn = new System.Data.DataColumn();
            this.messageColumn = new System.Data.DataColumn();
            this.fullMessageColumn = new System.Data.DataColumn();
            this.contextMenu.SuspendLayout();
            this.messagesToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messageWindowData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Messages)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonCopy,
            this.cToolStripMenuItem,
            this.buttonClearAll});
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            // 
            // buttonCopy
            // 
            this.buttonCopy.Image = global::Core.Common.Gui.Properties.Resources.CopyHS;
            this.buttonCopy.Name = "buttonCopy";
            resources.ApplyResources(this.buttonCopy, "buttonCopy");
            this.buttonCopy.Click += new System.EventHandler(this.ButtonCopyClick);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            resources.ApplyResources(this.cToolStripMenuItem, "cToolStripMenuItem");
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Image = global::Core.Common.Gui.Properties.Resources.icon_clear_all_messages;
            this.buttonClearAll.Name = "buttonClearAll";
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Click += new System.EventHandler(this.ButtonClearAllClick);
            // 
            // levelImages
            // 
            this.levelImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("levelImages.ImageStream")));
            this.levelImages.TransparentColor = System.Drawing.Color.Transparent;
            this.levelImages.Images.SetKeyName(0, "exclamation.png");
            this.levelImages.Images.SetKeyName(1, "information.png");
            this.levelImages.Images.SetKeyName(2, "error.png");
            this.levelImages.Images.SetKeyName(3, "debug.png");
            // 
            // messagesToolStrip
            // 
            this.messagesToolStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.messagesToolStrip, "messagesToolStrip");
            this.messagesToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.messagesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonShowInfo,
            this.buttonShowWarning,
            this.buttonShowError,
            this.toolStripSeparator1,
            this.buttonShowDetails});
            this.messagesToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.messagesToolStrip.Name = "messagesToolStrip";
            // 
            // buttonShowInfo
            // 
            this.buttonShowInfo.Checked = true;
            this.buttonShowInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowInfo.Image = global::Core.Common.Gui.Properties.Resources.information;
            resources.ApplyResources(this.buttonShowInfo, "buttonShowInfo");
            this.buttonShowInfo.Name = "buttonShowInfo";
            this.buttonShowInfo.Click += new System.EventHandler(this.ButtonShowInfoClick);
            // 
            // buttonShowWarning
            // 
            this.buttonShowWarning.Checked = true;
            this.buttonShowWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowWarning.Image = global::Core.Common.Gui.Properties.Resources.error;
            resources.ApplyResources(this.buttonShowWarning, "buttonShowWarning");
            this.buttonShowWarning.Name = "buttonShowWarning";
            this.buttonShowWarning.Click += new System.EventHandler(this.ButtonShowWarningClick);
            // 
            // buttonShowError
            // 
            this.buttonShowError.Checked = true;
            this.buttonShowError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowError.Image = global::Core.Common.Gui.Properties.Resources.exclamation;
            resources.ApplyResources(this.buttonShowError, "buttonShowError");
            this.buttonShowError.Name = "buttonShowError";
            this.buttonShowError.Click += new System.EventHandler(this.ButtonShowErrorClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // buttonShowDetails
            // 
            this.buttonShowDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowDetails.Image = global::Core.Common.Gui.Properties.Resources.application_import_blue;
            resources.ApplyResources(this.buttonShowDetails, "buttonShowDetails");
            this.buttonShowDetails.Name = "buttonShowDetails";
            this.buttonShowDetails.Click += new System.EventHandler(this.ButtonShowDetailsClick);
            // 
            // messagesDataGridView
            // 
            this.messagesDataGridView.AllowUserToAddRows = false;
            this.messagesDataGridView.AllowUserToDeleteRows = false;
            this.messagesDataGridView.AllowUserToResizeColumns = false;
            this.messagesDataGridView.AllowUserToResizeRows = false;
            this.messagesDataGridView.AutoGenerateColumns = false;
            this.messagesDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.messagesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.messagesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.levelColumnDataGridViewTextBoxColumn,
            this.timeColumnDataGridViewTextBoxColumn,
            this.messageColumnDataGridViewTextBoxColumn,
            this.fullMessageColumnDataGridViewTextBoxColumn});
            this.messagesDataGridView.ContextMenuStrip = this.contextMenu;
            this.messagesDataGridView.DataSource = this.messagesBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.messagesDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.messagesDataGridView, "messagesDataGridView");
            this.messagesDataGridView.Name = "messagesDataGridView";
            this.messagesDataGridView.ReadOnly = true;
            this.messagesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.messagesDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.messagesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.messagesDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MessagesDataGridViewCellMouseDoubleClick);
            this.messagesDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessagesDataGridViewKeyDown);
            // 
            // levelColumnDataGridViewTextBoxColumn
            // 
            this.levelColumnDataGridViewTextBoxColumn.DataPropertyName = "levelColumn";
            resources.ApplyResources(this.levelColumnDataGridViewTextBoxColumn, "levelColumnDataGridViewTextBoxColumn");
            this.levelColumnDataGridViewTextBoxColumn.Name = "levelColumnDataGridViewTextBoxColumn";
            this.levelColumnDataGridViewTextBoxColumn.ReadOnly = true;
            this.levelColumnDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.levelColumnDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // timeColumnDataGridViewTextBoxColumn
            // 
            this.timeColumnDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.timeColumnDataGridViewTextBoxColumn.DataPropertyName = "timeColumn";
            dataGridViewCellStyle1.Format = "HH:mm:ss";
            dataGridViewCellStyle1.NullValue = null;
            this.timeColumnDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.timeColumnDataGridViewTextBoxColumn, "timeColumnDataGridViewTextBoxColumn");
            this.timeColumnDataGridViewTextBoxColumn.Name = "timeColumnDataGridViewTextBoxColumn";
            this.timeColumnDataGridViewTextBoxColumn.ReadOnly = true;
            this.timeColumnDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // messageColumnDataGridViewTextBoxColumn
            // 
            this.messageColumnDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.messageColumnDataGridViewTextBoxColumn.DataPropertyName = "messageColumn";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messageColumnDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.messageColumnDataGridViewTextBoxColumn, "messageColumnDataGridViewTextBoxColumn");
            this.messageColumnDataGridViewTextBoxColumn.Name = "messageColumnDataGridViewTextBoxColumn";
            this.messageColumnDataGridViewTextBoxColumn.ReadOnly = true;
            this.messageColumnDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // fullMessageColumnDataGridViewTextBoxColumn
            // 
            this.fullMessageColumnDataGridViewTextBoxColumn.DataPropertyName = "fullMessageColumn";
            resources.ApplyResources(this.fullMessageColumnDataGridViewTextBoxColumn, "fullMessageColumnDataGridViewTextBoxColumn");
            this.fullMessageColumnDataGridViewTextBoxColumn.Name = "fullMessageColumnDataGridViewTextBoxColumn";
            this.fullMessageColumnDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // messagesBindingSource
            // 
            this.messagesBindingSource.DataMember = "MessageTable";
            this.messagesBindingSource.DataSource = this.messageWindowData;
            this.messagesBindingSource.Sort = "";
            // 
            // messageWindowData
            // 
            this.messageWindowData.DataSetName = "NewDataSet";
            this.messageWindowData.Tables.AddRange(new System.Data.DataTable[] {
            this.Messages});
            // 
            // Messages
            // 
            this.Messages.Columns.AddRange(new System.Data.DataColumn[] {
            this.levelColumn,
            this.timeColumn,
            this.messageColumn,
            this.fullMessageColumn});
            this.Messages.TableName = "MessageTable";
            // 
            // levelColumn
            // 
            this.levelColumn.Caption = "";
            this.levelColumn.ColumnName = "levelColumn";
            // 
            // timeColumn
            // 
            this.timeColumn.Caption = "";
            this.timeColumn.ColumnName = "timeColumn";
            this.timeColumn.DataType = typeof(System.DateTime);
            this.timeColumn.DateTimeMode = System.Data.DataSetDateTime.Local;
            // 
            // messageColumn
            // 
            this.messageColumn.ColumnName = "messageColumn";
            // 
            // fullMessageColumn
            // 
            this.fullMessageColumn.ColumnName = "fullMessageColumn";
            // 
            // MessageWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.messagesDataGridView);
            this.Controls.Add(this.messagesToolStrip);
            this.Name = "MessageWindow";
            this.contextMenu.ResumeLayout(false);
            this.messagesToolStrip.ResumeLayout(false);
            this.messagesToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messageWindowData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Messages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList levelImages;
        //Moved this one to main class so it can be precompiled
        //private System.Windows.Forms.DataGridViewImageColumn Level;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem buttonCopy;
        private System.Windows.Forms.ToolStripSeparator cToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buttonClearAll;
        private System.Windows.Forms.BindingSource messagesBindingSource;
        private System.Windows.Forms.ToolStrip messagesToolStrip;
        private System.Windows.Forms.ToolStripButton buttonShowWarning;
        private System.Windows.Forms.ToolStripButton buttonShowError;
        private System.Windows.Forms.ToolStripButton buttonShowInfo;
        private System.Windows.Forms.DataGridView messagesDataGridView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private ToolStripButton buttonShowDetails;
        private System.Data.DataSet messageWindowData;
        private System.Data.DataTable Messages;
        private System.Data.DataColumn levelColumn;
        private System.Data.DataColumn timeColumn;
        private System.Data.DataColumn messageColumn;
        private System.Data.DataColumn fullMessageColumn;
        private DataGridViewImageColumn levelColumnDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn timeColumnDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn messageColumnDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fullMessageColumnDataGridViewTextBoxColumn;
    }
}