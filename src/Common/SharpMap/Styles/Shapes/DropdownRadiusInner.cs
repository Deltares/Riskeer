using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    [ToolboxItem(false), ToolboxItemFilter("Prevent", ToolboxItemFilterType.Prevent)]
    public class DropdownRadiusInner : UserControl
    {
        // Fields
        [AccessedThroughProperty("butApply")]
        private Button _butApply;
        private IWindowsFormsEditorService _editorService = null;
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("lblValue")]
        private Label _lblValue;
        [AccessedThroughProperty("llNormal")]
        private LinkLabel _llNormal;
        [AccessedThroughProperty("llPentagon")]
        private LinkLabel _llPentagon;
        [AccessedThroughProperty("llThin")]
        private LinkLabel _llThin;
        [AccessedThroughProperty("panShapeHolder")]
        private Panel _panShapeHolder;
        [AccessedThroughProperty("tbarRadiusInner")]
        private TrackBar _tbarRadiusInner;
        [AccessedThroughProperty("TheShape")]
        private Shape _TheShape;
        private IContainer components;

        // Methods
        public DropdownRadiusInner(IWindowsFormsEditorService editorService)
        {
            this.InitializeComponent();
            this._editorService = editorService;
        }

        private void butApply_Click(object sender, EventArgs e)
        {
            this._editorService.CloseDropDown();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.tbarRadiusInner = new TrackBar();
            this.lblValue = new Label();
            this.butApply = new Button();
            this.llThin = new LinkLabel();
            this.llNormal = new LinkLabel();
            this.llPentagon = new LinkLabel();
            this.Label1 = new Label();
            this.panShapeHolder = new Panel();
            this.TheShape = new Shape();
            this.tbarRadiusInner.BeginInit();
            this.panShapeHolder.SuspendLayout();
            this.SuspendLayout();
            Point S0 = new Point(0x51, 0x1c);
            this.tbarRadiusInner.Location = S0;
            this.tbarRadiusInner.Maximum = 50;
            this.tbarRadiusInner.Minimum = -100;
            this.tbarRadiusInner.Name = "tbarRadiusInner";
            System.Drawing.Size S1 = new System.Drawing.Size(240, 0x2d);
            this.tbarRadiusInner.Size = S1;
            this.tbarRadiusInner.TabIndex = 3;
            this.tbarRadiusInner.TickFrequency = 10;
            this.lblValue.AutoSize = true;
            S0 = new Point(0x51, 60);
            this.lblValue.Location = S0;
            this.lblValue.Name = "lblValue";
            S1 = new System.Drawing.Size(0x27, 13);
            this.lblValue.Size = S1;
            this.lblValue.TabIndex = 4;
            this.lblValue.Text = "Label1";
            S0 = new Point(280, 0);
            this.butApply.Location = S0;
            this.butApply.Name = "butApply";
            S1 = new System.Drawing.Size(0x2e, 0x17);
            this.butApply.Size = S1;
            this.butApply.TabIndex = 5;
            this.butApply.Text = "Apply";
            this.butApply.UseVisualStyleBackColor = true;
            this.llThin.AutoSize = true;
            S0 = new Point(0x89, 0x3e);
            this.llThin.Location = S0;
            this.llThin.Name = "llThin";
            S1 = new System.Drawing.Size(0x1c, 13);
            this.llThin.Size = S1;
            this.llThin.TabIndex = 7;
            this.llThin.TabStop = true;
            this.llThin.Text = "Thin";
            this.llNormal.AutoSize = true;
            S0 = new Point(200, 0x3e);
            this.llNormal.Location = S0;
            this.llNormal.Name = "llNormal";
            S1 = new System.Drawing.Size(40, 13);
            this.llNormal.Size = S1;
            this.llNormal.TabIndex = 8;
            this.llNormal.TabStop = true;
            this.llNormal.Text = "Normal";
            this.llPentagon.AutoSize = true;
            S0 = new Point(0x10c, 0x3e);
            this.llPentagon.Location = S0;
            this.llPentagon.Name = "llPentagon";
            S1 = new System.Drawing.Size(0x35, 13);
            this.llPentagon.Size = S1;
            this.llPentagon.TabIndex = 9;
            this.llPentagon.TabStop = true;
            this.llPentagon.Text = "Pentagon";
            this.Label1.AutoSize = true;
            this.Label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.Label1.ForeColor = Color.Red;
            S0 = new Point(90, 5);
            this.Label1.Location = S0;
            this.Label1.Name = "Label1";
            S1 = new System.Drawing.Size(0xab, 13);
            this.Label1.Size = S1;
            this.Label1.TabIndex = 4;
            this.Label1.Text = "This applies to the Star shape Only";
            this.panShapeHolder.Controls.Add(this.TheShape);
            S0 = new Point(2, -1);
            this.panShapeHolder.Location = S0;
            this.panShapeHolder.Name = "panShapeHolder";
            S1 = new Size(80, 80);
            this.panShapeHolder.Size = S1;
            this.panShapeHolder.TabIndex = 10;
            this.TheShape.BorderColor = Color.Black;
            this.TheShape.BorderStyle = DashStyle.Solid;
            this.TheShape.BorderWidth = 2f;
            this.TheShape.ColorFillBlend = null;
            this.TheShape.ColorFillSolid = SystemColors.Control;
            this.TheShape.Corners.All = 0;
            this.TheShape.Corners.LowerLeft = 0;
            this.TheShape.Corners.LowerRight = 0;
            this.TheShape.Corners.UpperLeft = 0;
            this.TheShape.Corners.UpperRight = 0;
            this.TheShape.FillType = Shape.eFillType.Solid;
            this.TheShape.FillTypeLinear = LinearGradientMode.Horizontal;
            this.TheShape.FocalPoints = null;
            S0 = new Point(0, 0);
            this.TheShape.Location = S0;
            this.TheShape.Name = "TheShape";
            this.TheShape.RadiusInner = 0f;
            this.TheShape.RegionClip = false;
            this.TheShape.ShapeType = Shape.eShape.Rectangle;
            S1 = new System.Drawing.Size(80, 80);
            this.TheShape.Size = S1;
            this.TheShape.TabIndex = 1;
            SizeF S2 = new SizeF(6f, 13f);
            this.AutoScaleDimensions = S2;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Controls.Add(this.panShapeHolder);
            this.Controls.Add(this.llPentagon);
            this.Controls.Add(this.llNormal);
            this.Controls.Add(this.llThin);
            this.Controls.Add(this.butApply);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.tbarRadiusInner);
            this.Name = "DropdownRadiusInner";
            S1 = new System.Drawing.Size(0x149, 0x4f);
            this.Size = S1;
            this.tbarRadiusInner.EndInit();
            this.panShapeHolder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void llNormal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.TheShape.RadiusInner = 0f;
            this.tbarRadiusInner.Value = 0;
            this.lblValue.Text = Convert.ToString(this.TheShape.RadiusInner);
        }

        private void llPentagon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.TheShape.RadiusInner = 0.43f;
            this.tbarRadiusInner.Value = 0x2b;
            this.lblValue.Text = Convert.ToString(this.TheShape.RadiusInner);
        }

        private void llThin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.TheShape.RadiusInner = -0.25f;
            this.tbarRadiusInner.Value = -25;
            this.lblValue.Text = Convert.ToString(this.TheShape.RadiusInner);
        }

        private void tbarRadiusInner_Scroll(object sender, EventArgs e)
        {
            this.TheShape.RadiusInner = (float) (((double) this.tbarRadiusInner.Value) / 100.0);
            this.lblValue.Text = Convert.ToString(this.TheShape.RadiusInner);
        }

        // Properties
        internal virtual Button butApply
        {
            [DebuggerNonUserCode]
            get
            {
                return this._butApply;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._butApply != null)
                {
                    this._butApply.Click -= new EventHandler(this.butApply_Click);
                }
                this._butApply = value;
                if (this._butApply != null)
                {
                    this._butApply.Click += new EventHandler(this.butApply_Click);
                }
            }
        }

        internal virtual Label Label1
        {
            [DebuggerNonUserCode]
            get
            {
                return this._Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._Label1 = value;
            }
        }

        internal virtual Label lblValue
        {
            [DebuggerNonUserCode]
            get
            {
                return this._lblValue;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._lblValue = value;
            }
        }

        internal virtual LinkLabel llNormal
        {
            [DebuggerNonUserCode]
            get
            {
                return this._llNormal;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._llNormal != null)
                {
                    this._llNormal.LinkClicked -= new LinkLabelLinkClickedEventHandler(this.llNormal_LinkClicked);
                }
                this._llNormal = value;
                if (this._llNormal != null)
                {
                    this._llNormal.LinkClicked += new LinkLabelLinkClickedEventHandler(this.llNormal_LinkClicked);
                }
            }
        }

        internal virtual LinkLabel llPentagon
        {
            [DebuggerNonUserCode]
            get
            {
                return this._llPentagon;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._llPentagon != null)
                {
                    this._llPentagon.LinkClicked -= new LinkLabelLinkClickedEventHandler(this.llPentagon_LinkClicked);
                }
                this._llPentagon = value;
                if (this._llPentagon != null)
                {
                    this._llPentagon.LinkClicked += new LinkLabelLinkClickedEventHandler(this.llPentagon_LinkClicked);
                }
            }
        }

        internal virtual LinkLabel llThin
        {
            [DebuggerNonUserCode]
            get
            {
                return this._llThin;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._llThin != null)
                {
                    this._llThin.LinkClicked -= new LinkLabelLinkClickedEventHandler(this.llThin_LinkClicked);
                }
                this._llThin = value;
                if (this._llThin != null)
                {
                    this._llThin.LinkClicked += new LinkLabelLinkClickedEventHandler(this.llThin_LinkClicked);
                }
            }
        }

        internal virtual Panel panShapeHolder
        {
            [DebuggerNonUserCode]
            get
            {
                return this._panShapeHolder;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._panShapeHolder = value;
            }
        }

        internal virtual TrackBar tbarRadiusInner
        {
            [DebuggerNonUserCode]
            get
            {
                return this._tbarRadiusInner;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._tbarRadiusInner != null)
                {
                    this._tbarRadiusInner.Scroll -= new EventHandler(this.tbarRadiusInner_Scroll);
                }
                this._tbarRadiusInner = value;
                if (this._tbarRadiusInner != null)
                {
                    this._tbarRadiusInner.Scroll += new EventHandler(this.tbarRadiusInner_Scroll);
                }
            }
        }

        internal virtual Shape TheShape
        {
            [DebuggerNonUserCode]
            get
            {
                return this._TheShape;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._TheShape = value;
            }
        }
    }
}