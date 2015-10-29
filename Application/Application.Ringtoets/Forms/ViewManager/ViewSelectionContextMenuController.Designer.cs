namespace Application.Ringtoets.Forms.ViewManager
{
    partial class ViewSelectionContextMenuController
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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCloseOther = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLockUnlock = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemClose,
            this.menuItemCloseOther,
            this.menuItemCloseAll,
            this.menuItemLockUnlock});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 114);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(152, 22);
            this.menuItemClose.Text = "Close tab";
            this.menuItemClose.Click += new System.EventHandler(this.MenuItemCloseClick);
            // 
            // menuItemCloseOther
            // 
            this.menuItemCloseOther.Name = "menuItemCloseOther";
            this.menuItemCloseOther.Size = new System.Drawing.Size(152, 22);
            this.menuItemCloseOther.Text = "Close other tabs";
            this.menuItemCloseOther.Click += new System.EventHandler(this.MenuItemCloseOtherClick);
            // 
            // menuItemCloseAll
            // 
            this.menuItemCloseAll.Name = "menuItemCloseAll";
            this.menuItemCloseAll.Size = new System.Drawing.Size(152, 22);
            this.menuItemCloseAll.Text = "Close all tabs";
            this.menuItemCloseAll.Click += new System.EventHandler(this.MenuItemCloseAllClick);
            // 
            // menuItemLockUnlock
            // 
            this.menuItemLockUnlock.Image = global::Application.Ringtoets.Properties.Resources.lock_edit;
            this.menuItemLockUnlock.Name = "menuItemLockUnlock";
            this.menuItemLockUnlock.Size = new System.Drawing.Size(152, 22);
            this.menuItemLockUnlock.Text = "Lock/Unlock";
            this.menuItemLockUnlock.Click += new System.EventHandler(this.LockToolStripMenuItemClick);
            // 
            // ViewSelectionContextMenuController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ViewSelectionContextMenuController";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;

        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.ToolStripMenuItem menuItemCloseOther;
        private System.Windows.Forms.ToolStripMenuItem menuItemCloseAll;
        private System.Windows.Forms.ToolStripMenuItem menuItemLockUnlock;
    }
}