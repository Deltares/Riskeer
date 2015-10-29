using System.Windows.Forms;

namespace Application.Ringtoets.Forms.MessageWindow
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.messagesDataGridView = new System.Windows.Forms.DataGridView();
            this.imageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messagesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.messageWindowData = new MessageWindowData();
            this.contextMenu.SuspendLayout();
            this.messagesToolStrip.SuspendLayout();
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
            this.buttonClearAll});
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            // 
            // buttonCopy
            // 
            this.buttonCopy.Image = global::Application.Ringtoets.Properties.Resources.CopyHS;
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
            this.buttonClearAll.Image = global::Application.Ringtoets.Properties.Resources.icon_clear_all_messages;
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
            this.toolStripButton1});
            this.messagesToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.messagesToolStrip.Name = "messagesToolStrip";
            // 
            // buttonShowInfo
            // 
            this.buttonShowInfo.Checked = true;
            this.buttonShowInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowInfo.Image = global::Application.Ringtoets.Properties.Resources.information;
            resources.ApplyResources(this.buttonShowInfo, "buttonShowInfo");
            this.buttonShowInfo.Name = "buttonShowInfo";
            this.buttonShowInfo.Click += new System.EventHandler(this.ButtonShowInfoClick);
            // 
            // buttonShowWarning
            // 
            this.buttonShowWarning.Checked = true;
            this.buttonShowWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowWarning.Image = global::Application.Ringtoets.Properties.Resources.error;
            resources.ApplyResources(this.buttonShowWarning, "buttonShowWarning");
            this.buttonShowWarning.Name = "buttonShowWarning";
            this.buttonShowWarning.Click += new System.EventHandler(this.ButtonShowWarningClick);
            // 
            // buttonShowError
            // 
            this.buttonShowError.Checked = true;
            this.buttonShowError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowError.Image = global::Application.Ringtoets.Properties.Resources.exclamation;
            resources.ApplyResources(this.buttonShowError, "buttonShowError");
            this.buttonShowError.Name = "buttonShowError";
            this.buttonShowError.Click += new System.EventHandler(this.ButtonShowErrorClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Application.Ringtoets.Properties.Resources.application_import_blue;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.showDetailsToolStripMenuItem_Click);
            // 
            // messagesDataGridView
            // 
            this.messagesDataGridView.AllowUserToAddRows = false;
            this.messagesDataGridView.AllowUserToDeleteRows = false;
            this.messagesDataGridView.AllowUserToResizeRows = false;
            this.messagesDataGridView.AutoGenerateColumns = false;
            this.messagesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.messagesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.imageDataGridViewTextBoxColumn,
            this.Id,
            this.timeDataGridViewTextBoxColumn,
            this.messageDataGridViewTextBoxColumn});
            this.messagesDataGridView.ContextMenuStrip = this.contextMenu;
            this.messagesDataGridView.DataSource = this.messagesBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messagesDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
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
            dataGridViewCellStyle1.Format = "HH:mm:ss";
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
            this.Controls.Add(this.messagesToolStrip);
            this.Name = "MessageWindow";
            this.contextMenu.ResumeLayout(false);
            this.messagesToolStrip.ResumeLayout(false);
            this.messagesToolStrip.PerformLayout();
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
        private System.Windows.Forms.ToolStrip messagesToolStrip;
        private System.Windows.Forms.ToolStripButton buttonShowWarning;
        private System.Windows.Forms.ToolStripButton buttonShowError;
        private System.Windows.Forms.ToolStripButton buttonShowInfo;
        private System.Windows.Forms.DataGridView messagesDataGridView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private DataGridViewImageColumn imageDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn timeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
        private ToolStripButton toolStripButton1;
    }
}