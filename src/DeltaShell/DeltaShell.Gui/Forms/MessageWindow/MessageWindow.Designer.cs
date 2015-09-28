using System.Windows.Forms;

namespace DeltaShell.Gui.Forms.MessageWindow
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
            if (disposing && addNewMessagesTimer != null)
            {
                addNewMessagesTimer.Dispose();
            }

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
            this.showDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonClearAll = new System.Windows.Forms.ToolStripMenuItem();
            this.levelImages = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonShowDebug = new System.Windows.Forms.ToolStripButton();
            this.buttonShowInfo = new System.Windows.Forms.ToolStripButton();
            this.buttonShowWarning = new System.Windows.Forms.ToolStripButton();
            this.buttonShowError = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClearAllMessages = new System.Windows.Forms.ToolStripButton();
            this.messagesDataGridView = new System.Windows.Forms.DataGridView();
            this.imageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Exception = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messagesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.messageWindowData = new DeltaShell.Gui.Forms.MessageWindow.MessageWindowData();
            this.contextMenu.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messageWindowData)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonCopy,
            this.cToolStripMenuItem,
            this.showDetailsToolStripMenuItem,
            this.buttonClearAll});
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            // 
            // buttonCopy
            // 
            this.buttonCopy.Name = "buttonCopy";
            resources.ApplyResources(this.buttonCopy, "buttonCopy");
            this.buttonCopy.Click += new System.EventHandler(this.ButtonCopyClick);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            resources.ApplyResources(this.cToolStripMenuItem, "cToolStripMenuItem");
            // 
            // showDetailsToolStripMenuItem
            // 
            this.showDetailsToolStripMenuItem.Name = "showDetailsToolStripMenuItem";
            resources.ApplyResources(this.showDetailsToolStripMenuItem, "showDetailsToolStripMenuItem");
            this.showDetailsToolStripMenuItem.Click += new System.EventHandler(this.showDetailsToolStripMenuItem_Click);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Name = "buttonClearAll";
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Click += new System.EventHandler(this.ButtonClearAllClick);
            // 
            // levelImages
            // 
            this.levelImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("levelImages.ImageStream")));
            this.levelImages.TransparentColor = System.Drawing.Color.Transparent;
            this.levelImages.Images.SetKeyName(0, "error.png");
            this.levelImages.Images.SetKeyName(1, "info.png");
            this.levelImages.Images.SetKeyName(2, "warning.png");
            this.levelImages.Images.SetKeyName(3, "debug.png");
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonShowInfo,
            this.buttonShowWarning,
            this.buttonShowError,
            this.buttonShowDebug,
            this.toolStripSeparator1,
            this.buttonClearAllMessages});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip2.Name = "toolStrip2";
            // 
            // buttonShowDebug
            // 
            this.buttonShowDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowDebug.Image = global::DeltaShell.Gui.Properties.Resources.bug;
            resources.ApplyResources(this.buttonShowDebug, "buttonShowDebug");
            this.buttonShowDebug.Name = "buttonShowDebug";
            this.buttonShowDebug.Click += new System.EventHandler(this.ButtonShowDebugClick);
            // 
            // buttonShowInfo
            // 
            this.buttonShowInfo.Checked = true;
            this.buttonShowInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowInfo.Image = global::DeltaShell.Gui.Properties.Resources.information;
            resources.ApplyResources(this.buttonShowInfo, "buttonShowInfo");
            this.buttonShowInfo.Name = "buttonShowInfo";
            this.buttonShowInfo.Click += new System.EventHandler(this.ButtonShowInfoClick);
            // 
            // buttonShowWarning
            // 
            this.buttonShowWarning.Checked = true;
            this.buttonShowWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowWarning.Image = global::DeltaShell.Gui.Properties.Resources.error;
            resources.ApplyResources(this.buttonShowWarning, "buttonShowWarning");
            this.buttonShowWarning.Name = "buttonShowWarning";
            this.buttonShowWarning.Click += new System.EventHandler(this.ButtonShowWarningClick);
            // 
            // buttonShowError
            // 
            this.buttonShowError.Checked = true;
            this.buttonShowError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowError.Image = global::DeltaShell.Gui.Properties.Resources.exclamation;
            resources.ApplyResources(this.buttonShowError, "buttonShowError");
            this.buttonShowError.Name = "buttonShowError";
            this.buttonShowError.Click += new System.EventHandler(this.ButtonShowErrorClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // buttonClearAllMessages
            // 
            this.buttonClearAllMessages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.buttonClearAllMessages, "buttonClearAllMessages");
            this.buttonClearAllMessages.Name = "buttonClearAllMessages";
            this.buttonClearAllMessages.Click += new System.EventHandler(this.ButtonClearAllMessagesClick);
            // 
            // messagesDataGridView
            // 
            this.messagesDataGridView.AllowUserToAddRows = false;
            this.messagesDataGridView.AllowUserToDeleteRows = false;
            this.messagesDataGridView.AllowUserToResizeRows = false;
            this.messagesDataGridView.AutoGenerateColumns = false;
            this.messagesDataGridView.ColumnHeadersVisible = false;
            this.messagesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.imageDataGridViewTextBoxColumn,
            this.Id,
            this.timeDataGridViewTextBoxColumn,
            this.messageDataGridViewTextBoxColumn,
            this.sourceDataGridViewTextBoxColumn,
            this.Exception});
            this.messagesDataGridView.ContextMenuStrip = this.contextMenu;
            this.messagesDataGridView.DataSource = this.messagesBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messagesDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.messagesDataGridView, "messagesDataGridView");
            this.messagesDataGridView.Name = "messagesDataGridView";
            this.messagesDataGridView.ReadOnly = true;
            this.messagesDataGridView.RowHeadersVisible = false;
            this.messagesDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.messagesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // imageDataGridViewTextBoxColumn
            // 
            this.imageDataGridViewTextBoxColumn.DataPropertyName = "Image";
            this.imageDataGridViewTextBoxColumn.Frozen = true;
            resources.ApplyResources(this.imageDataGridViewTextBoxColumn, "imageDataGridViewTextBoxColumn");
            this.imageDataGridViewTextBoxColumn.Name = "imageDataGridViewTextBoxColumn";
            this.imageDataGridViewTextBoxColumn.ReadOnly = true;
            this.imageDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.imageDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            resources.ApplyResources(this.Id, "Id");
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // timeDataGridViewTextBoxColumn
            // 
            this.timeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.timeDataGridViewTextBoxColumn.DataPropertyName = "Time";
            dataGridViewCellStyle1.Format = "HH:mm:ss.ffff";
            dataGridViewCellStyle1.NullValue = null;
            this.timeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.timeDataGridViewTextBoxColumn, "timeDataGridViewTextBoxColumn");
            this.timeDataGridViewTextBoxColumn.Name = "timeDataGridViewTextBoxColumn";
            this.timeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // messageDataGridViewTextBoxColumn
            // 
            this.messageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.messageDataGridViewTextBoxColumn.DataPropertyName = "Message";
            this.messageDataGridViewTextBoxColumn.FillWeight = 70F;
            resources.ApplyResources(this.messageDataGridViewTextBoxColumn, "messageDataGridViewTextBoxColumn");
            this.messageDataGridViewTextBoxColumn.Name = "messageDataGridViewTextBoxColumn";
            this.messageDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // sourceDataGridViewTextBoxColumn
            // 
            this.sourceDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sourceDataGridViewTextBoxColumn.DataPropertyName = "Source";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sourceDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.sourceDataGridViewTextBoxColumn.FillWeight = 30F;
            resources.ApplyResources(this.sourceDataGridViewTextBoxColumn, "sourceDataGridViewTextBoxColumn");
            this.sourceDataGridViewTextBoxColumn.Name = "sourceDataGridViewTextBoxColumn";
            this.sourceDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Exception
            // 
            this.Exception.DataPropertyName = "Exception";
            resources.ApplyResources(this.Exception, "Exception");
            this.Exception.Name = "Exception";
            this.Exception.ReadOnly = true;
            // 
            // messagesBindingSource
            // 
            this.messagesBindingSource.DataMember = "Messages";
            this.messagesBindingSource.DataSource = this.messageWindowData;
            this.messagesBindingSource.Sort = "Id";
            // 
            // messageWindowData
            // 
            this.messageWindowData.DataSetName = "MessageWindowData";
            this.messageWindowData.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // MessageWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.messagesDataGridView);
            this.Controls.Add(this.toolStrip2);
            this.Name = "MessageWindow";
            this.contextMenu.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messageWindowData)).EndInit();
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
        private MessageWindowData messageWindowData;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonShowDebug;
        private System.Windows.Forms.ToolStripButton buttonShowWarning;
        private System.Windows.Forms.ToolStripButton buttonShowError;
        private System.Windows.Forms.ToolStripButton buttonShowInfo;
        private System.Windows.Forms.DataGridView messagesDataGridView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonClearAllMessages;
        private System.Windows.Forms.DataGridViewImageColumn imageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Exception;
        private ToolStripMenuItem showDetailsToolStripMenuItem;
    }
}