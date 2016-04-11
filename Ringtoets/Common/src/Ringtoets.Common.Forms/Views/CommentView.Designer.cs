namespace Ringtoets.Common.Forms.Views
{
    partial class CommentView
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
            this.richTextBoxControl = new Core.Common.Controls.TextEditor.RichTextBoxControl();
            this.SuspendLayout();
            // 
            // richTextBoxControl
            // 
            this.richTextBoxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxControl.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxControl.Name = "richTextBoxControl";
            this.richTextBoxControl.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft S" +
    "ans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            this.richTextBoxControl.Size = new System.Drawing.Size(150, 150);
            this.richTextBoxControl.TabIndex = 0;
            // 
            // CommentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBoxControl);
            this.Name = "CommentView";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.TextEditor.RichTextBoxControl richTextBoxControl;
    }
}
