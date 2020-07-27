﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Riskeer.Integration.Forms.Dialogs
{
    partial class HydraulicBoundaryDatabaseImporterSettingsDialog
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelHlcd = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelHrd = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelLocations = new System.Windows.Forms.TableLayoutPanel();
            this.labelHlcd = new System.Windows.Forms.Label();
            this.labelHrd = new System.Windows.Forms.Label();
            this.labelLocations = new System.Windows.Forms.Label();
            this.buttonHlcd = new System.Windows.Forms.Button();
            this.buttonHrd = new System.Windows.Forms.Button();
            this.buttonLocations = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanelHlcd.SuspendLayout();
            this.tableLayoutPanelHrd.SuspendLayout();
            this.tableLayoutPanelLocations.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelHlcd, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelHrd, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelLocations, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(800, 153);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanelHlcd
            // 
            this.tableLayoutPanelHlcd.ColumnCount = 3;
            this.tableLayoutPanelHlcd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHlcd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHlcd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelHlcd.Controls.Add(this.labelHlcd, 0, 0);
            this.tableLayoutPanelHlcd.Controls.Add(this.buttonHlcd, 2, 1);
            this.tableLayoutPanelHlcd.Controls.Add(this.textBox1, 0, 1);
            this.tableLayoutPanelHlcd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelHlcd.Location = new System.Drawing.Point(3, 6);
            this.tableLayoutPanelHlcd.Name = "tableLayoutPanelHlcd";
            this.tableLayoutPanelHlcd.RowCount = 2;
            this.tableLayoutPanelHlcd.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelHlcd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHlcd.Size = new System.Drawing.Size(794, 44);
            this.tableLayoutPanelHlcd.TabIndex = 0;
            // 
            // tableLayoutPanelHrd
            // 
            this.tableLayoutPanelHrd.ColumnCount = 3;
            this.tableLayoutPanelHrd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHrd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHrd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelHrd.Controls.Add(this.labelHrd, 0, 0);
            this.tableLayoutPanelHrd.Controls.Add(this.buttonHrd, 2, 1);
            this.tableLayoutPanelHrd.Controls.Add(this.textBox2, 0, 1);
            this.tableLayoutPanelHrd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelHrd.Location = new System.Drawing.Point(3, 56);
            this.tableLayoutPanelHrd.Name = "tableLayoutPanelHrd";
            this.tableLayoutPanelHrd.RowCount = 2;
            this.tableLayoutPanelHrd.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelHrd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHrd.Size = new System.Drawing.Size(794, 44);
            this.tableLayoutPanelHrd.TabIndex = 1;
            // 
            // tableLayoutPanelLocations
            // 
            this.tableLayoutPanelLocations.ColumnCount = 3;
            this.tableLayoutPanelLocations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLocations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLocations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelLocations.Controls.Add(this.labelLocations, 0, 0);
            this.tableLayoutPanelLocations.Controls.Add(this.buttonLocations, 2, 1);
            this.tableLayoutPanelLocations.Controls.Add(this.textBox3, 0, 1);
            this.tableLayoutPanelLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLocations.Location = new System.Drawing.Point(3, 106);
            this.tableLayoutPanelLocations.Name = "tableLayoutPanelLocations";
            this.tableLayoutPanelLocations.RowCount = 2;
            this.tableLayoutPanelLocations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLocations.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLocations.Size = new System.Drawing.Size(794, 44);
            this.tableLayoutPanelLocations.TabIndex = 2;
            // 
            // labelHlcd
            // 
            this.labelHlcd.AutoSize = true;
            this.labelHlcd.Location = new System.Drawing.Point(3, 0);
            this.labelHlcd.Name = "labelHlcd";
            this.labelHlcd.Size = new System.Drawing.Size(77, 13);
            this.labelHlcd.TabIndex = 0;
            this.labelHlcd.Text = "HLCD-bestand";
            // 
            // labelHrd
            // 
            this.labelHrd.AutoSize = true;
            this.labelHrd.Location = new System.Drawing.Point(3, 0);
            this.labelHrd.Name = "labelHrd";
            this.labelHrd.Size = new System.Drawing.Size(97, 13);
            this.labelHrd.TabIndex = 0;
            this.labelHrd.Text = "HRD-bestandsmap";
            // 
            // labelLocations
            // 
            this.labelLocations.AutoSize = true;
            this.labelLocations.Location = new System.Drawing.Point(3, 0);
            this.labelLocations.Name = "labelLocations";
            this.labelLocations.Size = new System.Drawing.Size(80, 13);
            this.labelLocations.TabIndex = 0;
            this.labelLocations.Text = "Locatiebestand";
            // 
            // buttonHlcd
            // 
            this.buttonHlcd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHlcd.Location = new System.Drawing.Point(757, 16);
            this.buttonHlcd.Name = "buttonHlcd";
            this.buttonHlcd.Size = new System.Drawing.Size(34, 25);
            this.buttonHlcd.TabIndex = 1;
            this.buttonHlcd.Text = "...";
            this.buttonHlcd.UseVisualStyleBackColor = true;
            // 
            // buttonHrd
            // 
            this.buttonHrd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHrd.Location = new System.Drawing.Point(757, 16);
            this.buttonHrd.Name = "buttonHrd";
            this.buttonHrd.Size = new System.Drawing.Size(34, 25);
            this.buttonHrd.TabIndex = 1;
            this.buttonHrd.Text = "...";
            this.buttonHrd.UseVisualStyleBackColor = true;
            // 
            // buttonLocations
            // 
            this.buttonLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLocations.Location = new System.Drawing.Point(757, 16);
            this.buttonLocations.Name = "buttonLocations";
            this.buttonLocations.Size = new System.Drawing.Size(34, 25);
            this.buttonLocations.TabIndex = 1;
            this.buttonLocations.Text = "...";
            this.buttonLocations.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.tableLayoutPanelHlcd.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 16);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(748, 20);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.tableLayoutPanelHrd.SetColumnSpan(this.textBox2, 2);
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(3, 16);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(748, 20);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.tableLayoutPanelLocations.SetColumnSpan(this.textBox3, 2);
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(3, 16);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(748, 20);
            this.textBox3.TabIndex = 2;
            // 
            // HydraulicBoundaryDatabaseImporterSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 303);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "HydraulicBoundaryDatabaseImporterSettingsDialog";
            this.Text = "Koppel aan database";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanelHlcd.ResumeLayout(false);
            this.tableLayoutPanelHlcd.PerformLayout();
            this.tableLayoutPanelHrd.ResumeLayout(false);
            this.tableLayoutPanelHrd.PerformLayout();
            this.tableLayoutPanelLocations.ResumeLayout(false);
            this.tableLayoutPanelLocations.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHlcd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHrd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLocations;
        private System.Windows.Forms.Label labelHlcd;
        private System.Windows.Forms.Label labelHrd;
        private System.Windows.Forms.Label labelLocations;
        private System.Windows.Forms.Button buttonHlcd;
        private System.Windows.Forms.Button buttonHrd;
        private System.Windows.Forms.Button buttonLocations;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
    }
}