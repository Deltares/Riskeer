using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Editors
{
    partial class ObjectEditor
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
            this.chkValue = new System.Windows.Forms.CheckBox();
            this.dtpValue = new System.Windows.Forms.DateTimePicker();
            this.txtValue = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // chkValue
            // 
            this.chkValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.chkValue.AutoSize = true;
            this.chkValue.Location = new System.Drawing.Point(0, 0);
            this.chkValue.Name = "chkValue";
            this.chkValue.Size = new System.Drawing.Size(15, 14);
            this.chkValue.TabIndex = 0;
            this.chkValue.ThreeState = true;
            this.chkValue.UseVisualStyleBackColor = true;
            // 
            // dtpValue
            // 
            this.dtpValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpValue.Location = new System.Drawing.Point(0, 0);
            this.dtpValue.Name = "dtpValue";
            this.dtpValue.Size = new System.Drawing.Size(231, 20);
            this.dtpValue.TabIndex = 2;
            this.dtpValue.CustomFormat = "dd-MM-yyyy  hh:mm:ss";
            this.dtpValue.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(0, 0);
            this.txtValue.Name = "txtValue";
            this.txtValue.Properties.Mask.IgnoreMaskBlank = false;
            this.txtValue.Size = new System.Drawing.Size(231, 20);
            this.txtValue.TabIndex = 3;
            // 
            // ObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.dtpValue);
            this.Controls.Add(this.chkValue);
            this.Name = "ObjectEditor";
            this.Size = new System.Drawing.Size(231, 21);
            ((System.ComponentModel.ISupportInitialize)(this.txtValue.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkValue;
        private System.Windows.Forms.DateTimePicker dtpValue;
        private DevExpress.XtraEditors.TextEdit txtValue;
    }
}