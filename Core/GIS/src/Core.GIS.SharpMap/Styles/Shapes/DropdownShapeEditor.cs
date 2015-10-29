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

namespace Core.GIS.SharpMap.Styles.Shapes
{
    [ToolboxItemFilter("Prevent", ToolboxItemFilterType.Prevent)]
    [ToolboxItem(false)]
    public class DropdownShapeEditor : UserControl
    {
        // Fields
        private readonly IWindowsFormsEditorService _editorService = null;

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

        private readonly Shape defaultShape = new Shape();

        // Methods
        public DropdownShapeEditor(IWindowsFormsEditorService editorService)
        {
            InitializeComponent();
            _editorService = editorService;

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

        public Shape.eShape SelectedShapeType { get; private set; }

        public Shape TheShape
        {
            get
            {
                return _TheShape;
            }
            set
            {
                _TheShape = value;
                Invalidate();
            }
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
        internal virtual Shape sCircle
        {
            [DebuggerNonUserCode]
            get
            {
                return _sCircle;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_sCircle != null)
                {
                    _sCircle.Click -= new EventHandler(sCircle_Click);
                    _sCircle.MouseLeave -= new EventHandler(Shape_MouseLeave);
                    _sCircle.MouseEnter -= new EventHandler(Shape_MouseEnter);
                }
                _sCircle = value;
                if (_sCircle != null)
                {
                    _sCircle.Click += new EventHandler(sCircle_Click);
                    _sCircle.MouseLeave += new EventHandler(Shape_MouseLeave);
                    _sCircle.MouseEnter += new EventHandler(Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sDiamond
        {
            [DebuggerNonUserCode]
            get
            {
                return _sDiamond;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_sDiamond != null)
                {
                    _sDiamond.Click -= new EventHandler(sCircle_Click);
                    _sDiamond.MouseLeave -= new EventHandler(Shape_MouseLeave);
                    _sDiamond.MouseEnter -= new EventHandler(Shape_MouseEnter);
                }
                _sDiamond = value;
                if (_sDiamond != null)
                {
                    _sDiamond.Click += new EventHandler(sCircle_Click);
                    _sDiamond.MouseLeave += new EventHandler(Shape_MouseLeave);
                    _sDiamond.MouseEnter += new EventHandler(Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sRectangle
        {
            [DebuggerNonUserCode]
            get
            {
                return _sRectangle;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_sRectangle != null)
                {
                    _sRectangle.Click -= new EventHandler(sCircle_Click);
                    _sRectangle.MouseLeave -= new EventHandler(Shape_MouseLeave);
                    _sRectangle.MouseEnter -= new EventHandler(Shape_MouseEnter);
                }
                _sRectangle = value;
                if (_sRectangle != null)
                {
                    _sRectangle.Click += new EventHandler(sCircle_Click);
                    _sRectangle.MouseLeave += new EventHandler(Shape_MouseLeave);
                    _sRectangle.MouseEnter += new EventHandler(Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sStar
        {
            [DebuggerNonUserCode]
            get
            {
                return _sStar;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_sStar != null)
                {
                    _sStar.Click -= new EventHandler(sCircle_Click);
                    _sStar.MouseLeave -= new EventHandler(Shape_MouseLeave);
                    _sStar.MouseEnter -= new EventHandler(Shape_MouseEnter);
                }
                _sStar = value;
                if (_sStar != null)
                {
                    _sStar.Click += new EventHandler(sCircle_Click);
                    _sStar.MouseLeave += new EventHandler(Shape_MouseLeave);
                    _sStar.MouseEnter += new EventHandler(Shape_MouseEnter);
                }
            }
        }

        internal virtual Shape sTriangle
        {
            [DebuggerNonUserCode]
            get
            {
                return _sTriangle;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            [DebuggerNonUserCode]
            set
            {
                if (_sTriangle != null)
                {
                    _sTriangle.Click -= new EventHandler(sCircle_Click);
                    _sTriangle.MouseLeave -= new EventHandler(Shape_MouseLeave);
                    _sTriangle.MouseEnter -= new EventHandler(Shape_MouseEnter);
                }
                _sTriangle = value;
                if (_sTriangle != null)
                {
                    _sTriangle.Click += new EventHandler(sCircle_Click);
                    _sTriangle.MouseLeave += new EventHandler(Shape_MouseLeave);
                    _sTriangle.MouseEnter += new EventHandler(Shape_MouseEnter);
                }
            }
        }

        private void sCircle_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape) sender;

            TheShape.ShapeType = shape.ShapeType;

            _editorService.CloseDropDown();
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

        private void InitializeComponent()
        {
            cBlendItems cBlendItems1 = new cBlendItems();
            cFocalPoints cFocalPoints1 = new cFocalPoints();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(DropdownShapeEditor));
            cBlendItems cBlendItems2 = new cBlendItems();
            cFocalPoints cFocalPoints2 = new cFocalPoints();
            cBlendItems cBlendItems3 = new cBlendItems();
            cFocalPoints cFocalPoints3 = new cFocalPoints();
            cBlendItems cBlendItems4 = new cBlendItems();
            cFocalPoints cFocalPoints4 = new cFocalPoints();
            _sDiamond = new Shape();
            _sTriangle = new Shape();
            _sRectangle = new Shape();
            _sCircle = new Shape();
            SuspendLayout();
            // 
            // _sDiamond
            // 
            _sDiamond.BorderColor = Color.Black;
            _sDiamond.BorderStyle = DashStyle.Solid;
            _sDiamond.BorderWidth = 2F;
            cBlendItems1.iColor = new Color[]
            {
                Color.White,
                Color.White
            };
            cBlendItems1.iPoint = new float[]
            {
                0F,
                1F
            };
            _sDiamond.ColorFillBlend = cBlendItems1;
            _sDiamond.ColorFillSolid = SystemColors.Control;
            _sDiamond.Corners.All = ((short) (0));
            _sDiamond.Corners.LowerLeft = ((short) (0));
            _sDiamond.Corners.LowerRight = ((short) (0));
            _sDiamond.Corners.UpperLeft = ((short) (0));
            _sDiamond.Corners.UpperRight = ((short) (0));
            _sDiamond.FillType = Shape.eFillType.Solid;
            _sDiamond.FillTypeLinear = LinearGradientMode.Horizontal;
            cFocalPoints1.CenterPoint = ((PointF) (resources.GetObject("cFocalPoints1.CenterPoint")));
            cFocalPoints1.FocusScales = ((PointF) (resources.GetObject("cFocalPoints1.FocusScales")));
            _sDiamond.FocalPoints = cFocalPoints1;
            _sDiamond.Location = new Point(175, 3);
            _sDiamond.Name = "_sDiamond";
            _sDiamond.RadiusInner = 0F;
            _sDiamond.RegionClip = false;
            _sDiamond.ShapeType = Shape.eShape.Diamond;
            _sDiamond.Size = new Size(50, 50);
            _sDiamond.TabIndex = 3;
            _sDiamond.Text = "_sDiamond";
            // 
            // _sTriangle
            // 
            _sTriangle.BorderColor = Color.Black;
            _sTriangle.BorderStyle = DashStyle.Solid;
            _sTriangle.BorderWidth = 2F;
            cBlendItems2.iColor = new Color[]
            {
                Color.White,
                Color.White
            };
            cBlendItems2.iPoint = new float[]
            {
                0F,
                1F
            };
            _sTriangle.ColorFillBlend = cBlendItems2;
            _sTriangle.ColorFillSolid = SystemColors.Control;
            _sTriangle.Corners.All = ((short) (0));
            _sTriangle.Corners.LowerLeft = ((short) (0));
            _sTriangle.Corners.LowerRight = ((short) (0));
            _sTriangle.Corners.UpperLeft = ((short) (0));
            _sTriangle.Corners.UpperRight = ((short) (0));
            _sTriangle.FillType = Shape.eFillType.Solid;
            _sTriangle.FillTypeLinear = LinearGradientMode.Horizontal;
            cFocalPoints2.CenterPoint = ((PointF) (resources.GetObject("cFocalPoints2.CenterPoint")));
            cFocalPoints2.FocusScales = ((PointF) (resources.GetObject("cFocalPoints2.FocusScales")));
            _sTriangle.FocalPoints = cFocalPoints2;
            _sTriangle.Location = new Point(119, 3);
            _sTriangle.Name = "_sTriangle";
            _sTriangle.RadiusInner = 0F;
            _sTriangle.RegionClip = false;
            _sTriangle.ShapeType = Shape.eShape.Triangle;
            _sTriangle.Size = new Size(50, 50);
            _sTriangle.TabIndex = 2;
            _sTriangle.Text = "_sTriangle";
            // 
            // _sRectangle
            // 
            _sRectangle.BorderColor = Color.Black;
            _sRectangle.BorderStyle = DashStyle.Solid;
            _sRectangle.BorderWidth = 2F;
            cBlendItems3.iColor = new Color[]
            {
                Color.White,
                Color.White
            };
            cBlendItems3.iPoint = new float[]
            {
                0F,
                1F
            };
            _sRectangle.ColorFillBlend = cBlendItems3;
            _sRectangle.ColorFillSolid = SystemColors.Control;
            _sRectangle.Corners.All = ((short) (0));
            _sRectangle.Corners.LowerLeft = ((short) (0));
            _sRectangle.Corners.LowerRight = ((short) (0));
            _sRectangle.Corners.UpperLeft = ((short) (0));
            _sRectangle.Corners.UpperRight = ((short) (0));
            _sRectangle.FillType = Shape.eFillType.Solid;
            _sRectangle.FillTypeLinear = LinearGradientMode.Horizontal;
            cFocalPoints3.CenterPoint = ((PointF) (resources.GetObject("cFocalPoints3.CenterPoint")));
            cFocalPoints3.FocusScales = ((PointF) (resources.GetObject("cFocalPoints3.FocusScales")));
            _sRectangle.FocalPoints = cFocalPoints3;
            _sRectangle.Location = new Point(63, 3);
            _sRectangle.Name = "_sRectangle";
            _sRectangle.RadiusInner = 0F;
            _sRectangle.RegionClip = false;
            _sRectangle.ShapeType = Shape.eShape.Rectangle;
            _sRectangle.Size = new Size(50, 50);
            _sRectangle.TabIndex = 1;
            _sRectangle.Text = "_sRectangle";
            // 
            // _sCircle
            // 
            _sCircle.BorderColor = Color.Black;
            _sCircle.BorderStyle = DashStyle.Solid;
            _sCircle.BorderWidth = 2F;
            cBlendItems4.iColor = new Color[]
            {
                Color.White,
                Color.White
            };
            cBlendItems4.iPoint = new float[]
            {
                0F,
                1F
            };
            _sCircle.ColorFillBlend = cBlendItems4;
            _sCircle.ColorFillSolid = SystemColors.Control;
            _sCircle.Corners.All = ((short) (0));
            _sCircle.Corners.LowerLeft = ((short) (0));
            _sCircle.Corners.LowerRight = ((short) (0));
            _sCircle.Corners.UpperLeft = ((short) (0));
            _sCircle.Corners.UpperRight = ((short) (0));
            _sCircle.FillType = Shape.eFillType.Solid;
            _sCircle.FillTypeLinear = LinearGradientMode.Horizontal;
            cFocalPoints4.CenterPoint = ((PointF) (resources.GetObject("cFocalPoints4.CenterPoint")));
            cFocalPoints4.FocusScales = ((PointF) (resources.GetObject("cFocalPoints4.FocusScales")));
            _sCircle.FocalPoints = cFocalPoints4;
            _sCircle.Location = new Point(7, 3);
            _sCircle.Name = "_sCircle";
            _sCircle.RadiusInner = 0F;
            _sCircle.RegionClip = false;
            _sCircle.ShapeType = Shape.eShape.Ellipse;
            _sCircle.Size = new Size(50, 50);
            _sCircle.TabIndex = 0;
            _sCircle.Text = "_sCircle";
            // 
            // DropdownShapeEditor
            // 
            BackColor = SystemColors.Window;
            Controls.Add(_sDiamond);
            Controls.Add(_sTriangle);
            Controls.Add(_sRectangle);
            Controls.Add(_sCircle);
            Name = "DropdownShapeEditor";
            Size = new Size(235, 62);
            ResumeLayout(false);
        }
    }
}