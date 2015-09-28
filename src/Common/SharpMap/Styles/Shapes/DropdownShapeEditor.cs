// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SharpMap.Styles.Shapes
{
    [ToolboxItemFilter("Prevent", ToolboxItemFilterType.Prevent), ToolboxItem(false)]
    public class DropdownShapeEditor : UserControl
    {
        // Fields
        private IWindowsFormsEditorService _editorService = null;
        [AccessedThroughProperty("sCircle")]
        private Shape _sCircle;
        [AccessedThroughProperty("sDiamond")]
        private Shape _sDiamond;
        [AccessedThroughProperty("sRectangle")]
        private Shape _sRectangle;
        [AccessedThroughProperty("sStar")]
        private Shape _sStar;
        [AccessedThroughProperty("sTriangle")]
        private Shape _sTriangle;
        private Shape _TheShape = new Shape();
        private IContainer components;

        Shape defaultShape = new Shape();
        private Shape.eShape selectedShapeType;


        // Methods
        public DropdownShapeEditor(IWindowsFormsEditorService editorService)
        {
            this.InitializeComponent();
            this._editorService = editorService;
        
            sCircle = _sCircle;
            sRectangle = _sRectangle;
            sTriangle = _sTriangle;
            sDiamond = _sDiamond;

            defaultShape.FillType = _sCircle.FillType;
            defaultShape.FillTypeLinear = _sCircle.FillTypeLinear;
            defaultShape.ColorFillSolid = _sCircle.ColorFillSolid;
            defaultShape.BorderWidth = _sCircle.BorderWidth;
            defaultShape.BorderColor = _sCircle.BorderColor;
            defaultShape.BorderStyle = _sCircle.BorderStyle;
            defaultShape.RadiusInner = _sCircle.RadiusInner;
            defaultShape.FocalPoints = _sCircle.FocalPoints;
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

        public Shape.eShape SelectedShapeType
        {
            get { return selectedShapeType; }
        }

        private void sCircle_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape) sender;

            TheShape.ShapeType = shape.ShapeType;
       
            this._editorService.CloseDropDown();
        }

        private void Shape_MouseEnter(object sender, EventArgs e)
        {
            Shape shape = (Shape) sender;

            shape.FillType = Shape.eFillType.Solid;
            shape.ColorFillSolid = Pens.LightGreen.Color;
            shape.BorderWidth = 2;
            shape.BorderColor = Color.Black;
            shape.BorderStyle = DashStyle.Solid;
            shape.RadiusInner = 0;
        }

        private void Shape_MouseLeave(object sender, EventArgs e)
        {
            Shape shape = (Shape) sender;

            shape.FillType = defaultShape.FillType;
            shape.FillTypeLinear = defaultShape.FillTypeLinear;
            shape.ColorFillSolid = defaultShape.ColorFillSolid;
            shape.BorderWidth = defaultShape.BorderWidth;
            shape.BorderColor = defaultShape.BorderColor;
            shape.BorderStyle = defaultShape.BorderStyle;
            shape.RadiusInner = defaultShape.RadiusInner;
            shape.FocalPoints = defaultShape.FocalPoints;
        }

        // Properties
        internal virtual Shape sCircle
        {
            [DebuggerNonUserCode]
            get
            {
                return this._sCircle;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._sCircle != null)
                {
                    this._sCircle.Click -= new EventHandler(this.sCircle_Click);
                    this._sCircle.MouseLeave -= new EventHandler(this.Shape_MouseLeave);
                    this._sCircle.MouseEnter -= new EventHandler(this.Shape_MouseEnter);
                }
                this._sCircle = value;
                if (this._sCircle != null)
                {
                    this._sCircle.Click += new EventHandler(this.sCircle_Click);
                    this._sCircle.MouseLeave += new EventHandler(this.Shape_MouseLeave);
                    this._sCircle.MouseEnter += new EventHandler(this.Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sDiamond
        {
            [DebuggerNonUserCode]
            get
            {
                return this._sDiamond;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._sDiamond != null)
                {
                    this._sDiamond.Click -= new EventHandler(this.sCircle_Click);
                    this._sDiamond.MouseLeave -= new EventHandler(this.Shape_MouseLeave);
                    this._sDiamond.MouseEnter -= new EventHandler(this.Shape_MouseEnter);
                }
                this._sDiamond = value;
                if (this._sDiamond != null)
                {
                    this._sDiamond.Click += new EventHandler(this.sCircle_Click);
                    this._sDiamond.MouseLeave += new EventHandler(this.Shape_MouseLeave);
                    this._sDiamond.MouseEnter += new EventHandler(this.Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sRectangle
        {
            [DebuggerNonUserCode]
            get
            {
                return this._sRectangle;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._sRectangle != null)
                {
                    this._sRectangle.Click -= new EventHandler(this.sCircle_Click);
                    this._sRectangle.MouseLeave -= new EventHandler(this.Shape_MouseLeave);
                    this._sRectangle.MouseEnter -= new EventHandler(this.Shape_MouseEnter);
                }
                this._sRectangle = value;
                if (this._sRectangle != null)
                {
                    this._sRectangle.Click += new EventHandler(this.sCircle_Click);
                    this._sRectangle.MouseLeave += new EventHandler(this.Shape_MouseLeave);
                    this._sRectangle.MouseEnter += new EventHandler(this.Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sStar
        {
            [DebuggerNonUserCode]
            get
            {
                return this._sStar;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._sStar != null)
                {
                    this._sStar.Click -= new EventHandler(this.sCircle_Click);
                    this._sStar.MouseLeave -= new EventHandler(this.Shape_MouseLeave);
                    this._sStar.MouseEnter -= new EventHandler(this.Shape_MouseEnter);
                }
                this._sStar = value;
                if (this._sStar != null)
                {
                    this._sStar.Click += new EventHandler(this.sCircle_Click);
                    this._sStar.MouseLeave += new EventHandler(this.Shape_MouseLeave);
                    this._sStar.MouseEnter += new EventHandler(this.Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sTriangle
        {
            [DebuggerNonUserCode]
            get
            {
                return this._sTriangle;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                if (this._sTriangle != null)
                {
                    this._sTriangle.Click -= new EventHandler(this.sCircle_Click);
                    this._sTriangle.MouseLeave -= new EventHandler(this.Shape_MouseLeave);
                    this._sTriangle.MouseEnter -= new EventHandler(this.Shape_MouseEnter);
                }
                this._sTriangle = value;
                if (this._sTriangle != null)
                {
                    this._sTriangle.Click += new EventHandler(this.sCircle_Click);
                    this._sTriangle.MouseLeave += new EventHandler(this.Shape_MouseLeave);
                    this._sTriangle.MouseEnter += new EventHandler(this.Shape_MouseEnter);
                }
            }
        }

        public Shape TheShape
        {
            get
            {
                return this._TheShape;
            }
            set
            {
                this._TheShape = value;
                this.Invalidate();
            }
        }

        private void InitializeComponent()
        {
            cBlendItems cBlendItems1 = new cBlendItems();
            cFocalPoints cFocalPoints1 = new cFocalPoints();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropdownShapeEditor));
            cBlendItems cBlendItems2 = new cBlendItems();
            cFocalPoints cFocalPoints2 = new cFocalPoints();
            cBlendItems cBlendItems3 = new cBlendItems();
            cFocalPoints cFocalPoints3 = new cFocalPoints();
            cBlendItems cBlendItems4 = new cBlendItems();
            cFocalPoints cFocalPoints4 = new cFocalPoints();
            this._sDiamond = new Shape();
            this._sTriangle = new Shape();
            this._sRectangle = new Shape();
            this._sCircle = new Shape();
            this.SuspendLayout();
            // 
            // _sDiamond
            // 
            this._sDiamond.BorderColor = System.Drawing.Color.Black;
            this._sDiamond.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this._sDiamond.BorderWidth = 2F;
            cBlendItems1.iColor = new System.Drawing.Color[] {
                                                                 System.Drawing.Color.White,
                                                                 System.Drawing.Color.White};
            cBlendItems1.iPoint = new float[] {
                                                  0F,
                                                  1F};
            this._sDiamond.ColorFillBlend = cBlendItems1;
            this._sDiamond.ColorFillSolid = System.Drawing.SystemColors.Control;
            this._sDiamond.Corners.All = ((short)(0));
            this._sDiamond.Corners.LowerLeft = ((short)(0));
            this._sDiamond.Corners.LowerRight = ((short)(0));
            this._sDiamond.Corners.UpperLeft = ((short)(0));
            this._sDiamond.Corners.UpperRight = ((short)(0));
            this._sDiamond.FillType = Shape.eFillType.Solid;
            this._sDiamond.FillTypeLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            cFocalPoints1.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.CenterPoint")));
            cFocalPoints1.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.FocusScales")));
            this._sDiamond.FocalPoints = cFocalPoints1;
            this._sDiamond.Location = new System.Drawing.Point(175, 3);
            this._sDiamond.Name = "_sDiamond";
            this._sDiamond.RadiusInner = 0F;
            this._sDiamond.RegionClip = false;
            this._sDiamond.ShapeType = Shape.eShape.Diamond;
            this._sDiamond.Size = new System.Drawing.Size(50, 50);
            this._sDiamond.TabIndex = 3;
            this._sDiamond.Text = "_sDiamond";
            // 
            // _sTriangle
            // 
            this._sTriangle.BorderColor = System.Drawing.Color.Black;
            this._sTriangle.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this._sTriangle.BorderWidth = 2F;
            cBlendItems2.iColor = new System.Drawing.Color[] {
                                                                 System.Drawing.Color.White,
                                                                 System.Drawing.Color.White};
            cBlendItems2.iPoint = new float[] {
                                                  0F,
                                                  1F};
            this._sTriangle.ColorFillBlend = cBlendItems2;
            this._sTriangle.ColorFillSolid = System.Drawing.SystemColors.Control;
            this._sTriangle.Corners.All = ((short)(0));
            this._sTriangle.Corners.LowerLeft = ((short)(0));
            this._sTriangle.Corners.LowerRight = ((short)(0));
            this._sTriangle.Corners.UpperLeft = ((short)(0));
            this._sTriangle.Corners.UpperRight = ((short)(0));
            this._sTriangle.FillType = Shape.eFillType.Solid;
            this._sTriangle.FillTypeLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            cFocalPoints2.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints2.CenterPoint")));
            cFocalPoints2.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints2.FocusScales")));
            this._sTriangle.FocalPoints = cFocalPoints2;
            this._sTriangle.Location = new System.Drawing.Point(119, 3);
            this._sTriangle.Name = "_sTriangle";
            this._sTriangle.RadiusInner = 0F;
            this._sTriangle.RegionClip = false;
            this._sTriangle.ShapeType = Shape.eShape.Triangle;
            this._sTriangle.Size = new System.Drawing.Size(50, 50);
            this._sTriangle.TabIndex = 2;
            this._sTriangle.Text = "_sTriangle";
            // 
            // _sRectangle
            // 
            this._sRectangle.BorderColor = System.Drawing.Color.Black;
            this._sRectangle.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this._sRectangle.BorderWidth = 2F;
            cBlendItems3.iColor = new System.Drawing.Color[] {
                                                                 System.Drawing.Color.White,
                                                                 System.Drawing.Color.White};
            cBlendItems3.iPoint = new float[] {
                                                  0F,
                                                  1F};
            this._sRectangle.ColorFillBlend = cBlendItems3;
            this._sRectangle.ColorFillSolid = System.Drawing.SystemColors.Control;
            this._sRectangle.Corners.All = ((short)(0));
            this._sRectangle.Corners.LowerLeft = ((short)(0));
            this._sRectangle.Corners.LowerRight = ((short)(0));
            this._sRectangle.Corners.UpperLeft = ((short)(0));
            this._sRectangle.Corners.UpperRight = ((short)(0));
            this._sRectangle.FillType = Shape.eFillType.Solid;
            this._sRectangle.FillTypeLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            cFocalPoints3.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints3.CenterPoint")));
            cFocalPoints3.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints3.FocusScales")));
            this._sRectangle.FocalPoints = cFocalPoints3;
            this._sRectangle.Location = new System.Drawing.Point(63, 3);
            this._sRectangle.Name = "_sRectangle";
            this._sRectangle.RadiusInner = 0F;
            this._sRectangle.RegionClip = false;
            this._sRectangle.ShapeType = Shape.eShape.Rectangle;
            this._sRectangle.Size = new System.Drawing.Size(50, 50);
            this._sRectangle.TabIndex = 1;
            this._sRectangle.Text = "_sRectangle";
            // 
            // _sCircle
            // 
            this._sCircle.BorderColor = System.Drawing.Color.Black;
            this._sCircle.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this._sCircle.BorderWidth = 2F;
            cBlendItems4.iColor = new System.Drawing.Color[] {
                                                                 System.Drawing.Color.White,
                                                                 System.Drawing.Color.White};
            cBlendItems4.iPoint = new float[] {
                                                  0F,
                                                  1F};
            this._sCircle.ColorFillBlend = cBlendItems4;
            this._sCircle.ColorFillSolid = System.Drawing.SystemColors.Control;
            this._sCircle.Corners.All = ((short)(0));
            this._sCircle.Corners.LowerLeft = ((short)(0));
            this._sCircle.Corners.LowerRight = ((short)(0));
            this._sCircle.Corners.UpperLeft = ((short)(0));
            this._sCircle.Corners.UpperRight = ((short)(0));
            this._sCircle.FillType = Shape.eFillType.Solid;
            this._sCircle.FillTypeLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            cFocalPoints4.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints4.CenterPoint")));
            cFocalPoints4.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints4.FocusScales")));
            this._sCircle.FocalPoints = cFocalPoints4;
            this._sCircle.Location = new System.Drawing.Point(7, 3);
            this._sCircle.Name = "_sCircle";
            this._sCircle.RadiusInner = 0F;
            this._sCircle.RegionClip = false;
            this._sCircle.ShapeType = Shape.eShape.Ellipse;
            this._sCircle.Size = new System.Drawing.Size(50, 50);
            this._sCircle.TabIndex = 0;
            this._sCircle.Text = "_sCircle";
            // 
            // DropdownShapeEditor
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this._sDiamond);
            this.Controls.Add(this._sTriangle);
            this.Controls.Add(this._sRectangle);
            this.Controls.Add(this._sCircle);
            this.Name = "DropdownShapeEditor";
            this.Size = new System.Drawing.Size(235, 62);
            this.ResumeLayout(false);

        }
    }
}