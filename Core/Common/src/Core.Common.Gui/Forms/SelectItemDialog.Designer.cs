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
            this.listViewItemTypes = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
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
            this.listViewItemTypes.LargeImageList = this.imageList;
            this.listViewItemTypes.MultiSelect = false;
            this.listViewItemTypes.Name = "listViewItemTypes";
            this.listViewItemTypes.SmallImageList = this.imageList;
            this.listViewItemTypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewItemTypes.UseCompatibleStateImageBehavior = false;
            this.listViewItemTypes.View = System.Windows.Forms.View.Tile;
            this.listViewItemTypes.DoubleClick += new System.EventHandler(this.ListViewItemTypesDoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "table.png");
            this.imageList.Images.SetKeyName(1, "chart_curve.png");
            this.imageList.Images.SetKeyName(2, "map.png");
            // 
            // SelectItemDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.listViewItemTypes);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "SelectItemDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private ListView listViewItemTypes;
        private System.Windows.Forms.ImageList imageList;
    }
}