// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Common.Gui
{
    partial class ExceptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.buttonRestart = new System.Windows.Forms.Button();
            this.buttonCopyTextToClipboard = new System.Windows.Forms.Button();
            this.exceptionTextBox = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            this.buttonSaveProject = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRestart
            // 
            resources.ApplyResources(this.buttonRestart, "buttonRestart");
            this.buttonRestart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.ButtonRestartClick);
            // 
            // buttonCopyTextToClipboard
            // 
            resources.ApplyResources(this.buttonCopyTextToClipboard, "buttonCopyTextToClipboard");
            this.buttonCopyTextToClipboard.Name = "buttonCopyTextToClipboard";
            this.buttonCopyTextToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyTextToClipboard.Click += new System.EventHandler(this.ButtonCopyTextToClipboardClick);
            // 
            // exceptionTextBox
            // 
            resources.ApplyResources(this.exceptionTextBox, "exceptionTextBox");
            this.exceptionTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.exceptionTextBox.ForeColor = System.Drawing.Color.Black;
            this.exceptionTextBox.Name = "exceptionTextBox";
            this.exceptionTextBox.ReadOnly = true;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            // 
            // buttonExit
            // 
            resources.ApplyResources(this.buttonExit, "buttonExit");
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExitClick);
            // 
            // buttonOpenLog
            // 
            resources.ApplyResources(this.buttonOpenLog, "buttonOpenLog");
            this.buttonOpenLog.Name = "buttonOpenLog";
            this.buttonOpenLog.UseVisualStyleBackColor = true;
            this.buttonOpenLog.Click += new System.EventHandler(this.ButtonOpenLogClick);
            // 
            // buttonSaveProject
            // 
            resources.ApplyResources(this.buttonSaveProject, "buttonSaveProject");
            this.buttonSaveProject.Name = "buttonSaveProject";
            this.buttonSaveProject.UseVisualStyleBackColor = true;
            this.buttonSaveProject.Click += new System.EventHandler(this.ButtonSaveProjectClick);
            // 
            // ExceptionDialog
            // 
            this.AcceptButton = this.buttonRestart;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonOpenLog);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.exceptionTextBox);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonCopyTextToClipboard);
            this.Controls.Add(this.buttonRestart);
            this.Controls.Add(this.buttonSaveProject);
            this.Name = "ExceptionDialog";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.Button buttonCopyTextToClipboard;
        private System.Windows.Forms.RichTextBox exceptionTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonOpenLog;
        private System.Windows.Forms.Button buttonSaveProject;
    }
}