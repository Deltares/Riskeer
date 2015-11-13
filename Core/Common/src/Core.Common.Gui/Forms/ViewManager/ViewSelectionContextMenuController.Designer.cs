namespace Core.Common.Gui.Forms.ViewManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSelectionContextMenuController));
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
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            resources.ApplyResources(this.menuItemClose, "menuItemClose");
            this.menuItemClose.Click += new System.EventHandler(this.MenuItemCloseClick);
            // 
            // menuItemCloseOther
            // 
            this.menuItemCloseOther.Name = "menuItemCloseOther";
            resources.ApplyResources(this.menuItemCloseOther, "menuItemCloseOther");
            this.menuItemCloseOther.Click += new System.EventHandler(this.MenuItemCloseOtherClick);
            // 
            // menuItemCloseAll
            // 
            this.menuItemCloseAll.Name = "menuItemCloseAll";
            resources.ApplyResources(this.menuItemCloseAll, "menuItemCloseAll");
            this.menuItemCloseAll.Click += new System.EventHandler(this.MenuItemCloseAllClick);
            // 
            // menuItemLockUnlock
            // 
            this.menuItemLockUnlock.Image = global::Core.Common.Gui.Properties.Resources.lock_edit;
            this.menuItemLockUnlock.Name = "menuItemLockUnlock";
            resources.ApplyResources(this.menuItemLockUnlock, "menuItemLockUnlock");
            this.menuItemLockUnlock.Click += new System.EventHandler(this.LockToolStripMenuItemClick);
            // 
            // ViewSelectionContextMenuController
            // 
            resources.ApplyResources(this, "$this");
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