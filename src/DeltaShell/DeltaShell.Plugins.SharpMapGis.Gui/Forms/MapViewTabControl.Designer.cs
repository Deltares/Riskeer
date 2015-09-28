using System;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    partial class MapViewTabControl
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
            if (disposing)
            {
                if (dockingManager != null)
                {
                    dockingManager.ActiveContentChanged -= DockingManagerActiveContentChanged;
                    dockingManager.DocumentClosed -= DockingManagerOnDocumentClosed;
                }
                
                foreach (var view in views)
                {
                    view.Data = null;
                    view.Dispose();
                }
                views.Clear();

                foreach (var viewHost in viewHosts)
                {
                    viewHost.Child = null;
                    viewHost.Dispose();
                }
                viewHosts.Clear();

                if (elementHost != null)
                {
                    elementHost.Dispose();
                    elementHost = null;
                }

                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }

            try
            {
                base.Dispose(disposing);
            }
            catch (InvalidOperationException)
            {
                if (disposing)
                    throw; //only throw if not on GC thread
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // elementHost
            // 
            this.elementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost.Location = new System.Drawing.Point(0, 0);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(775, 470);
            this.elementHost.TabIndex = 0;
            this.elementHost.Text = "elementHost1";
            this.elementHost.Child = null;
            // 
            // MapViewTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.elementHost);
            this.Name = "MapViewTabControl";
            this.Size = new System.Drawing.Size(775, 470);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost;
    }
}
