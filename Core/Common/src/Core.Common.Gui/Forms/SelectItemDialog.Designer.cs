using System.Windows.Forms;

namespace Core.Common.Gui.Forms
{
    partial class SelectItemDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectItemDialog));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewItemTypes = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.checkBoxExample = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // listViewItemTypes
            // 
            resources.ApplyResources(this.listViewItemTypes, "listViewItemTypes");
            this.listViewItemTypes.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listViewItemTypes.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listViewItemTypes.Groups1"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listViewItemTypes.Groups2")))});
            this.listViewItemTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewItemTypes.HideSelection = false;
            this.listViewItemTypes.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items3"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items4"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewItemTypes.Items5")))});
            this.listViewItemTypes.LargeImageList = this.imageList;
            this.listViewItemTypes.MultiSelect = false;
            this.listViewItemTypes.Name = "listViewItemTypes";
            this.listViewItemTypes.SmallImageList = this.imageList;
            this.listViewItemTypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewItemTypes.UseCompatibleStateImageBehavior = false;
            this.listViewItemTypes.View = System.Windows.Forms.View.Tile;
            this.listViewItemTypes.SelectedIndexChanged += new System.EventHandler(this.listViewItemTypes_SelectedIndexChanged);
            this.listViewItemTypes.DoubleClick += new System.EventHandler(this.listViewItemTypes_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "table.png");
            this.imageList.Images.SetKeyName(1, "chart_curve.png");
            this.imageList.Images.SetKeyName(2, "map.png");
            // 
            // checkBoxExample
            // 
            resources.ApplyResources(this.checkBoxExample, "checkBoxExample");
            this.checkBoxExample.Name = "checkBoxExample";
            this.checkBoxExample.UseVisualStyleBackColor = true;
            this.checkBoxExample.CheckedChanged += new System.EventHandler(this.checkBoxDemo_CheckedChanged);
            // 
            // SelectItemDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxExample);
            this.Controls.Add(this.listViewItemTypes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectItemDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Validating += new System.ComponentModel.CancelEventHandler(this.NewDataDialog_Validating);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private ListView listViewItemTypes;
        private System.Windows.Forms.ImageList imageList;
        private CheckBox checkBoxExample;
    }
}