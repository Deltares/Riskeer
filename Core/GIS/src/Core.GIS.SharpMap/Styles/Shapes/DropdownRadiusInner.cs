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

namespace Core.GIS.SharpMap.Styles.Shapes
{
    [ToolboxItem(false)]
    [ToolboxItemFilter("Prevent", ToolboxItemFilterType.Prevent)]
    public class DropdownRadiusInner : UserControl
    {
        // Fields
        [AccessedThroughProperty("butApply")]
        private Button _butApply;

        private readonly IWindowsFormsEditorService _editorService = null;

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
            InitializeComponent();
            _editorService = editorService;
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Properties
        internal virtual Button butApply
        {
            [DebuggerNonUserCode]
            get
            {
                return _butApply;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_butApply != null)
                {
                    _butApply.Click -= new EventHandler(butApply_Click);
                }
                _butApply = value;
                if (_butApply != null)
                {
                    _butApply.Click += new EventHandler(butApply_Click);
                }
            }
        }

        internal virtual Label Label1
        {
            [DebuggerNonUserCode]
            get
            {
                return _Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                _Label1 = value;
            }
        }

        internal virtual Label lblValue
        {
            [DebuggerNonUserCode]
            get
            {
                return _lblValue;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                _lblValue = value;
            }
        }

        internal virtual LinkLabel llNormal
        {
            [DebuggerNonUserCode]
            get
            {
                return _llNormal;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_llNormal != null)
                {
                    _llNormal.LinkClicked -= new LinkLabelLinkClickedEventHandler(llNormal_LinkClicked);
                }
                _llNormal = value;
                if (_llNormal != null)
                {
                    _llNormal.LinkClicked += new LinkLabelLinkClickedEventHandler(llNormal_LinkClicked);
                }
            }
        }

        internal virtual LinkLabel llPentagon
        {
            [DebuggerNonUserCode]
            get
            {
                return _llPentagon;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_llPentagon != null)
                {
                    _llPentagon.LinkClicked -= new LinkLabelLinkClickedEventHandler(llPentagon_LinkClicked);
                }
                _llPentagon = value;
                if (_llPentagon != null)
                {
                    _llPentagon.LinkClicked += new LinkLabelLinkClickedEventHandler(llPentagon_LinkClicked);
                }
            }
        }

        internal virtual LinkLabel llThin
        {
            [DebuggerNonUserCode]
            get
            {
                return _llThin;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_llThin != null)
                {
                    _llThin.LinkClicked -= new LinkLabelLinkClickedEventHandler(llThin_LinkClicked);
                }
                _llThin = value;
                if (_llThin != null)
                {
                    _llThin.LinkClicked += new LinkLabelLinkClickedEventHandler(llThin_LinkClicked);
                }
            }
        }

        internal virtual Panel panShapeHolder
        {
            [DebuggerNonUserCode]
            get
            {
                return _panShapeHolder;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                _panShapeHolder = value;
            }
        }

        internal virtual TrackBar tbarRadiusInner
        {
            [DebuggerNonUserCode]
            get
            {
                return _tbarRadiusInner;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_tbarRadiusInner != null)
                {
                    _tbarRadiusInner.Scroll -= new EventHandler(tbarRadiusInner_Scroll);
                }
                _tbarRadiusInner = value;
                if (_tbarRadiusInner != null)
                {
                    _tbarRadiusInner.Scroll += new EventHandler(tbarRadiusInner_Scroll);
                }
            }
        }

        internal virtual Shape TheShape
        {
            [DebuggerNonUserCode]
            get
            {
                return _TheShape;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                _TheShape = value;
            }
        }

        private void butApply_Click(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            tbarRadiusInner = new TrackBar();
            lblValue = new Label();
            butApply = new Button();
            llThin = new LinkLabel();
            llNormal = new LinkLabel();
            llPentagon = new LinkLabel();
            Label1 = new Label();
            panShapeHolder = new Panel();
            TheShape = new Shape();
            tbarRadiusInner.BeginInit();
            panShapeHolder.SuspendLayout();
            SuspendLayout();
            Point S0 = new Point(0x51, 0x1c);
            tbarRadiusInner.Location = S0;
            tbarRadiusInner.Maximum = 50;
            tbarRadiusInner.Minimum = -100;
            tbarRadiusInner.Name = "tbarRadiusInner";
            Size S1 = new Size(240, 0x2d);
            tbarRadiusInner.Size = S1;
            tbarRadiusInner.TabIndex = 3;
            tbarRadiusInner.TickFrequency = 10;
            lblValue.AutoSize = true;
            S0 = new Point(0x51, 60);
            lblValue.Location = S0;
            lblValue.Name = "lblValue";
            S1 = new Size(0x27, 13);
            lblValue.Size = S1;
            lblValue.TabIndex = 4;
            lblValue.Text = "Label1";
            S0 = new Point(280, 0);
            butApply.Location = S0;
            butApply.Name = "butApply";
            S1 = new Size(0x2e, 0x17);
            butApply.Size = S1;
            butApply.TabIndex = 5;
            butApply.Text = "Apply";
            butApply.UseVisualStyleBackColor = true;
            llThin.AutoSize = true;
            S0 = new Point(0x89, 0x3e);
            llThin.Location = S0;
            llThin.Name = "llThin";
            S1 = new Size(0x1c, 13);
            llThin.Size = S1;
            llThin.TabIndex = 7;
            llThin.TabStop = true;
            llThin.Text = "Thin";
            llNormal.AutoSize = true;
            S0 = new Point(200, 0x3e);
            llNormal.Location = S0;
            llNormal.Name = "llNormal";
            S1 = new Size(40, 13);
            llNormal.Size = S1;
            llNormal.TabIndex = 8;
            llNormal.TabStop = true;
            llNormal.Text = "Normal";
            llPentagon.AutoSize = true;
            S0 = new Point(0x10c, 0x3e);
            llPentagon.Location = S0;
            llPentagon.Name = "llPentagon";
            S1 = new Size(0x35, 13);
            llPentagon.Size = S1;
            llPentagon.TabIndex = 9;
            llPentagon.TabStop = true;
            llPentagon.Text = "Pentagon";
            Label1.AutoSize = true;
            Label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Underline, GraphicsUnit.Point, 0);
            Label1.ForeColor = Color.Red;
            S0 = new Point(90, 5);
            Label1.Location = S0;
            Label1.Name = "Label1";
            S1 = new Size(0xab, 13);
            Label1.Size = S1;
            Label1.TabIndex = 4;
            Label1.Text = "This applies to the Star shape Only";
            panShapeHolder.Controls.Add(TheShape);
            S0 = new Point(2, -1);
            panShapeHolder.Location = S0;
            panShapeHolder.Name = "panShapeHolder";
            S1 = new Size(80, 80);
            panShapeHolder.Size = S1;
            panShapeHolder.TabIndex = 10;
            TheShape.BorderColor = Color.Black;
            TheShape.BorderStyle = DashStyle.Solid;
            TheShape.BorderWidth = 2f;
            TheShape.ColorFillBlend = null;
            TheShape.ColorFillSolid = SystemColors.Control;
            TheShape.Corners.All = 0;
            TheShape.Corners.LowerLeft = 0;
            TheShape.Corners.LowerRight = 0;
            TheShape.Corners.UpperLeft = 0;
            TheShape.Corners.UpperRight = 0;
            TheShape.FillType = Shape.eFillType.Solid;
            TheShape.FillTypeLinear = LinearGradientMode.Horizontal;
            TheShape.FocalPoints = null;
            S0 = new Point(0, 0);
            TheShape.Location = S0;
            TheShape.Name = "TheShape";
            TheShape.RadiusInner = 0f;
            TheShape.RegionClip = false;
            TheShape.ShapeType = Shape.eShape.Rectangle;
            S1 = new Size(80, 80);
            TheShape.Size = S1;
            TheShape.TabIndex = 1;
            SizeF S2 = new SizeF(6f, 13f);
            AutoScaleDimensions = S2;
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(panShapeHolder);
            Controls.Add(llPentagon);
            Controls.Add(llNormal);
            Controls.Add(llThin);
            Controls.Add(butApply);
            Controls.Add(Label1);
            Controls.Add(lblValue);
            Controls.Add(tbarRadiusInner);
            Name = "DropdownRadiusInner";
            S1 = new Size(0x149, 0x4f);
            Size = S1;
            tbarRadiusInner.EndInit();
            panShapeHolder.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private void llNormal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TheShape.RadiusInner = 0f;
            tbarRadiusInner.Value = 0;
            lblValue.Text = Convert.ToString(TheShape.RadiusInner);
        }

        private void llPentagon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TheShape.RadiusInner = 0.43f;
            tbarRadiusInner.Value = 0x2b;
            lblValue.Text = Convert.ToString(TheShape.RadiusInner);
        }

        private void llThin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TheShape.RadiusInner = -0.25f;
            tbarRadiusInner.Value = -25;
            lblValue.Text = Convert.ToString(TheShape.RadiusInner);
        }

        private void tbarRadiusInner_Scroll(object sender, EventArgs e)
        {
            TheShape.RadiusInner = (float) (((double) tbarRadiusInner.Value)/100.0);
            lblValue.Text = Convert.ToString(TheShape.RadiusInner);
        }
    }
}