using Core.Common.Controls.Swf;

namespace Core.Plugins.CommonTools.Gui.Forms
{
    partial class TextDocumentView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextDocumentView));
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.findAndReplaceControl1 = new FindAndReplaceControl();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.White;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.HideSelection = false;
            this.textBox.Name = "textBox";
            this.textBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.textBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyUp);
            // 
            // timer
            // 
            this.timer.Interval = 750;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // findAndReplaceControl1
            // 
            resources.ApplyResources(this.findAndReplaceControl1, "findAndReplaceControl1");
            this.findAndReplaceControl1.BackColor = System.Drawing.SystemColors.Control;
            this.findAndReplaceControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.findAndReplaceControl1.HighLightText = null;
            this.findAndReplaceControl1.Name = "findAndReplaceControl1";
            this.findAndReplaceControl1.ReplaceText = null;
            // 
            // TextDocumentView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.findAndReplaceControl1);
            this.Controls.Add(this.textBox);
            this.Name = "TextDocumentView";
            this.Load += new System.EventHandler(this.TextDocumentView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBox;
        private System.Windows.Forms.Timer timer;
        private FindAndReplaceControl findAndReplaceControl1;
    }
}
