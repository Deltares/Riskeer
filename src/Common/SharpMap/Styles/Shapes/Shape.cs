// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SharpMap.Styles.Shapes
{
    /// <summary>
    /// TODO: split this class into Shape (dependent ONLY on System.Drawing) and ShapeControl (move to SharpMap.UI)
    /// </summary>
    public class Shape : Control, IShape
    {
        // Fields
        private Color _BorderColor;
        private bool _BorderShow;
        private DashStyle _BorderStyle;
        private float _BorderWidth;
        private cBlendItems _ColorFillBlend;
        private Color _ColorFillSolid;
        private CornersProperty _Corners;
        private eFillType _FillType;
        private LinearGradientMode _FillTypeLinear;
        private cFocalPoints _FocalPoints;
        private float _RadiusInner;
        private bool _RegionClip;
        private eShape _Shape;
        private IContainer components;

        // Methods
        public Shape()
        {
            base.Paint += new PaintEventHandler(this.Shape_Paint);
            this._Corners = new CornersProperty();
            this._FocalPoints = new cFocalPoints(0.5, 0.5, 0.0, 0.0);
            this._BorderStyle = DashStyle.Solid;
            this._RadiusInner = 0f;
            this._BorderShow = true;
            this._BorderWidth = 2f;
            this._ColorFillSolid = SystemColors.Control;
            Color[] S0 = new Color[] { Color.White, Color.White };
            this._ColorFillBlend = new cBlendItems(S0, new float[] { 0f, 1f });
            this._FillType = eFillType.Solid;
            this._FillTypeLinear = LinearGradientMode.Horizontal;
            this._BorderColor = Color.Black;
            this._RegionClip = false;
            this.InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
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

        public GraphicsPath DrawHeart(float Xc, float Yc, RectangleF rect)
        {
            Xc += rect.X;
            Yc += rect.Y;
            GraphicsPath gp = new GraphicsPath();
            float RadiusOuter = (rect.Width - (rect.X / 2f)) / 2f;
            Rectangle HeartTopLeftSquare = new Rectangle((int) Math.Round((double) Xc), (int) Math.Round((double) Yc), (int) Math.Round((double) RadiusOuter), (int) Math.Round((double) RadiusOuter));
            Rectangle HeartTopRightSquare = new Rectangle((int) Math.Round((double) (Xc + RadiusOuter)), (int) Math.Round((double) Yc), (int) Math.Round((double) RadiusOuter), (int) Math.Round((double) RadiusOuter));
            gp.AddArc(HeartTopLeftSquare, 135f, 210f);
            gp.AddArc(HeartTopRightSquare, 180f, 210f);
            gp.AddLine((float) (Xc + RadiusOuter), (float) (Yc + (RadiusOuter * 2f)), (float) (Xc + RadiusOuter), (float) (Yc + (RadiusOuter * 2f)));
            gp.CloseFigure();
            return gp;
        }

        public GraphicsPath DrawStar(float Xc, float Yc, RectangleF rect, float RadiusInner)
        {
            GraphicsPath gp = new GraphicsPath();
            float xRadiusOuter = rect.Width / 2f;
            float yRadiusOuter = rect.Height / 2f;
            Xc += xRadiusOuter;
            Yc += yRadiusOuter;
            float sin36 = (float) Math.Sin(0.62831853071795862);
            float sin72 = (float) Math.Sin(1.2566370614359173);
            float cos36 = (float) Math.Cos(0.62831853071795862);
            float cos72 = (float) Math.Cos(1.2566370614359173);
            float xInnerRadius = ((xRadiusOuter * cos72) / cos36) + (xRadiusOuter * RadiusInner);
            float yInnerRadius = ((yRadiusOuter * cos72) / cos36) + (yRadiusOuter * RadiusInner);
            Yc += (yRadiusOuter - (Yc - (yRadiusOuter * cos72))) / 4f;
            PointF[] pts = new PointF[] { new PointF(Xc, Yc - yRadiusOuter), new PointF(Xc + (xInnerRadius * sin36), Yc - (yInnerRadius * cos36)), new PointF(Xc + (xRadiusOuter * sin72), Yc - (yRadiusOuter * cos72)), new PointF(Xc + (xInnerRadius * sin72), Yc + (yInnerRadius * cos72)), new PointF(Xc + (xRadiusOuter * sin36), Yc + (yRadiusOuter * cos36)), new PointF(Xc, Yc + yInnerRadius), new PointF(Xc - (xRadiusOuter * sin36), Yc + (yRadiusOuter * cos36)), new PointF(Xc - (xInnerRadius * sin72), Yc + (yInnerRadius * cos72)), new PointF(Xc - (xRadiusOuter * sin72), Yc - (yRadiusOuter * cos72)), new PointF(Xc - (xInnerRadius * sin36), Yc - (yInnerRadius * cos36)) };
            gp.AddPolygon(pts);
            return gp;
        }

        public GraphicsPath GetPath(eShape Shape, RectangleF rect, [Optional, DefaultParameterValue(0f)] float RI)
        {
            PointF S1;
            PointF S2;
            PointF[] S0;
            GraphicsPath gp = new GraphicsPath();
            switch (Shape)
            {
                case eShape.Ellipse:
                    gp.AddEllipse(rect);
                    return gp;

                case eShape.Rectangle:
                    return this.GetRoundedRectPath(rect, this.Corners);

                case eShape.Triangle:
                    {
                        S0 = new PointF[3];
                        S0[0] = new PointF(rect.Width / 2f, rect.Y);
                        S1 = new PointF(rect.Width, rect.Y + rect.Height);
                        S0[1] = S1;
                        S2 = new PointF(rect.X, rect.Y + rect.Height);
                        S0[2] = S2;
                        PointF[] pts = S0;
                        gp.AddPolygon(pts);
                        return gp;
                    }
                case eShape.Diamond:
                    {
                        S0 = new PointF[4];
                        S2 = new PointF(rect.Width / 2f, rect.Y);
                        S0[0] = S2;
                        S1 = new PointF(rect.X + rect.Width, (rect.Y + rect.Height) / 2f);
                        S0[1] = S1;
                        S0[2] = new PointF(rect.X + (rect.Width / 2f), rect.Y + rect.Height);
                        PointF S3 = new PointF(rect.X, (rect.Y + rect.Height) / 2f);
                        S0[3] = S3;
                        PointF[] pts = S0;
                        gp.AddPolygon(pts);
                        return gp;
                    }
            }
            return gp;
        }

        public GraphicsPath GetRoundedRectPath(RectangleF BaseRect, CornersProperty rCorners)
        {
            RectangleF ArcRect;
            SizeF S0;
            GraphicsPath MyPath = new GraphicsPath();
            if (rCorners.All == -1)
            {
                GraphicsPath L0 = MyPath;
                if (rCorners.UpperLeft == 0)
                {
                    L0.AddLine(BaseRect.X, BaseRect.Y, BaseRect.X, BaseRect.Y);
                }
                else
                {
                    S0 = new SizeF((float) (rCorners.UpperLeft * 2), (float) (rCorners.UpperLeft * 2));
                    ArcRect = new RectangleF(BaseRect.Location, S0);
                    L0.AddArc(ArcRect, 180f, 90f);
                }
                if (rCorners.UpperRight == 0)
                {
                    L0.AddLine(BaseRect.X + rCorners.UpperLeft, BaseRect.Y, BaseRect.Right - rCorners.UpperRight, BaseRect.Top);
                }
                else
                {
                    S0 = new SizeF((float) (rCorners.UpperRight * 2), (float) (rCorners.UpperRight * 2));
                    ArcRect = new RectangleF(BaseRect.Location, S0);
                    ArcRect.X = BaseRect.Right - (rCorners.UpperRight * 2);
                    L0.AddArc(ArcRect, 270f, 90f);
                }
                if (rCorners.LowerRight == 0)
                {
                    L0.AddLine(BaseRect.Right, BaseRect.Top + rCorners.UpperRight, BaseRect.Right, BaseRect.Bottom - rCorners.LowerRight);
                }
                else
                {
                    S0 = new SizeF((float) (rCorners.LowerRight * 2), (float) (rCorners.LowerRight * 2));
                    ArcRect = new RectangleF(BaseRect.Location, S0);
                    ArcRect.Y = BaseRect.Bottom - (rCorners.LowerRight * 2);
                    ArcRect.X = BaseRect.Right - (rCorners.LowerRight * 2);
                    L0.AddArc(ArcRect, 0f, 90f);
                }
                if (rCorners.LowerLeft == 0)
                {
                    L0.AddLine(BaseRect.Right - rCorners.LowerRight, BaseRect.Bottom, BaseRect.X - rCorners.LowerLeft, BaseRect.Bottom);
                }
                else
                {
                    S0 = new SizeF((float) (rCorners.LowerLeft * 2), (float) (rCorners.LowerLeft * 2));
                    ArcRect = new RectangleF(BaseRect.Location, S0);
                    ArcRect.Y = BaseRect.Bottom - (rCorners.LowerLeft * 2);
                    L0.AddArc(ArcRect, 90f, 90f);
                }
                L0.CloseFigure();
                L0 = null;
                return MyPath;
            }
            GraphicsPath L1 = MyPath;
            if (rCorners.All == 0)
            {
                L1.AddRectangle(BaseRect);
            }
            else
            {
                S0 = new SizeF((float) (rCorners.All * 2), (float) (rCorners.All * 2));
                ArcRect = new RectangleF(BaseRect.Location, S0);
                L1.AddArc(ArcRect, 180f, 90f);
                ArcRect.X = BaseRect.Right - (rCorners.All * 2);
                L1.AddArc(ArcRect, 270f, 90f);
                ArcRect.Y = BaseRect.Bottom - (rCorners.All * 2);
                L1.AddArc(ArcRect, 0f, 90f);
                ArcRect.X = BaseRect.Left;
                L1.AddArc(ArcRect, 90f, 90f);
            }
            L1.CloseFigure();
            L1 = null;
            return MyPath;
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "Shape";
            Size S0 = new Size(100, 100);
            this.Size = S0;
            this.ResumeLayout(false);
        }

        private void Shape_Paint(object sender, PaintEventArgs e)
        {
            Paint(e.Graphics);
        }

        public void Paint(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float adjWidth = this.Width - this.BorderWidth;
            float adjHeight = this.Height - this.BorderWidth;
            RectangleF rect = new RectangleF(this.BorderWidth / 2f, this.BorderWidth / 2f, adjWidth - 1f, adjHeight - 1f);
            GraphicsPath gp = new GraphicsPath();
            gp = this.GetPath(this.ShapeType, rect, this.RadiusInner);
            switch (this.FillType)
            {
                case eFillType.Solid:
                    using (Brush br = new SolidBrush(this.ColorFillSolid))
                    {
                        g.FillPath(br, gp);
                    }
                    break;

                case eFillType.GradientLinear:
                    using (LinearGradientBrush br = new LinearGradientBrush(rect, Color.White, Color.White, this.FillTypeLinear))
                    {
                        ColorBlend cb = new ColorBlend();
                        cb.Colors = this.ColorFillBlend.iColor;
                        cb.Positions = this.ColorFillBlend.iPoint;
                        br.InterpolationColors = cb;
                        g.FillPath(br, gp);
                    }
                    break;

                case eFillType.GradientPath:
                    using (PathGradientBrush br = new PathGradientBrush(gp))
                    {
                        ColorBlend cb = new ColorBlend();
                        cb.Colors = this.ColorFillBlend.iColor;
                        cb.Positions = this.ColorFillBlend.iPoint;
                        br.FocusScales = this.FocalPoints.FocusScales;
                        PointF S3 = new PointF(adjWidth * this.FocalPoints.CenterPoint.X, adjHeight * this.FocalPoints.CenterPoint.Y);
                        br.CenterPoint = S3;
                        br.InterpolationColors = cb;
                        g.FillPath(br, gp);
                    }
                    break;
            }
            if ((this.BorderWidth > 0f) & this.BorderShow)
            {
                using (Pen pn = new Pen(this.BorderColor, this.BorderWidth))
                {
                    if ((this.BorderWidth > 1f))
                    {
                        pn.LineJoin = LineJoin.Round;
                    }
                    pn.DashStyle = this.BorderStyle;
                    try
                    {
                        g.DrawPath(pn, gp);
                    }
                    catch (OutOfMemoryException)
                    {
                        //TOOLS-8188
                        //on occassions this calls a OutOfMemoryException, even though there is still plenty of memory. This is related to
                        //a buggy DashPattern (even though it happens when DashStyle is Solid). For now the 'fix' is to just ignore this exception
                        //somewhat related link:
                        //http://stackoverflow.com/questions/4270015/why-doesnt-drawing-an-ellipse-with-pendashpattern-set-produce-the-expected-re
                    }
                }
            }
            this.Region = null;
            if (this.RegionClip)
            {
                rect = new RectangleF((this.BorderWidth / 2f) - ((float)0.5), (this.BorderWidth / 2f) - ((float)0.5), adjWidth, adjHeight);
                if (this.ShapeType == eShape.Rectangle)
                {
                    rect = new RectangleF(this.BorderWidth / 2f, this.BorderWidth / 2f, adjWidth, adjHeight);
                }
                Region mRegion = new Region(this.GetPath(this.ShapeType, rect, this.RadiusInner));
                this.Region = mRegion;
                mRegion.Dispose();
            }
            gp.Dispose();
        }

        // Properties
        [Description("The Color for the border"), Category("Shape")]
        public Color BorderColor
        {
            get
            {
                return this._BorderColor;
            }
            set
            {
                this._BorderColor = value;
                this.Invalidate();
            }
        }

        [DefaultValue(true), Description("Show or not show the Border"), Category("Shape")]
        public bool BorderShow
        {
            get
            {
                return this._BorderShow;
            }
            set
            {
                this._BorderShow = value;
                this.Invalidate();
            }
        }

        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor)), Description("The line dash style used to draw state borders."), Browsable(true), Category("Shape")]
        public DashStyle BorderStyle
        {
            get
            {
                return this._BorderStyle;
            }
            set
            {
                this._BorderStyle = value;
                this.Invalidate();
            }
        }

        [DefaultValue(2), Description("The Width of the border around the shape"), Category("Shape")]
        public float BorderWidth
        {
            get
            {
                return this._BorderWidth;
            }
            set
            {
                this._BorderWidth = value;
                this.Invalidate();
            }
        }
        
        // TODO: port BlendTypeEditor
        // [Editor(typeof(BlendTypeEditor), typeof(UITypeEditor)), Description("The ColorBlend to fill the shape"), Category("Shape")]
        public cBlendItems ColorFillBlend
        {
            get
            {
                return this._ColorFillBlend;
            }
            set
            {
                this._ColorFillBlend = value;
                this.Invalidate();
            }
        }

        [Category("Shape"), Description("The Solid Color to fill the shape")]
        public Color ColorFillSolid
        {
            get
            {
                return this._ColorFillSolid;
            }
            set
            {
                this._ColorFillSolid = value;
                this.Invalidate();
            }
        }

        [RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Get or Set the Corner Radii"), Category("Shape")]
        public CornersProperty Corners
        {
            get
            {
                return this._Corners;
            }
            set
            {
                this._Corners = value;
                this.Refresh();
            }
        }

        [Category("Shape"), Description("The Fill Type to apply to the Shape")]
        public eFillType FillType
        {
            get
            {
                return this._FillType;
            }
            set
            {
                this._FillType = value;
                this.Invalidate();
            }
        }

        [Description("The Linear Blend type"), Category("Shape")]
        public LinearGradientMode FillTypeLinear
        {
            get
            {
                return this._FillTypeLinear;
            }
            set
            {
                this._FillTypeLinear = value;
                this.Invalidate();
            }
        }

        // TODO: portFocalTypeEditor
        // [Editor(typeof(FocalTypeEditor), typeof(UITypeEditor)), Category("Shape"), Description("The CenterPoint and FocusScales for the ColorBlend")]
        public cFocalPoints FocalPoints
        {
            get
            {
                return this._FocalPoints;
            }
            set
            {
                this._FocalPoints = value;
                this.Invalidate();
            }
        }

        [Browsable(true), Editor(typeof(RadiusInnerTypeEditor), typeof(UITypeEditor)), Category("Shape"), Description("The Inner Radius for the Star Shape")]
        public float RadiusInner
        {
            get
            {
                return this._RadiusInner;
            }
            set
            {
                this._RadiusInner = value;
                this.Invalidate();
            }
        }

        [Description("Clip off outside area"), Category("Shape"), RefreshProperties(RefreshProperties.All)]
        public bool RegionClip
        {
            get
            {
                return this._RegionClip;
            }
            set
            {
                this._RegionClip = value;
                this.Invalidate();
            }
        }

        ShapeType IShape.ShapeType
        {
            get { return (ShapeType)Enum.Parse(typeof (ShapeType), ShapeType.ToString()); }
            set { ShapeType = (Shape.eShape) Enum.Parse(typeof (Shape.eShape), value.ToString()); }
        }

        
        ///<summary>
        ///
        ///</summary>
        [Category("Shape"), Description("The Shape"), Editor(typeof(ShapeTypeEditor), typeof(UITypeEditor))]
        public eShape ShapeType
        {
            get
            {
                return this._Shape;
            }
            set
            {
                this._Shape = value;
                this.Invalidate();
            }
        }

        // Nested Types
        public enum eFillType
        {
            Solid,
            GradientLinear,
            GradientPath
        }

        public enum eShape
        {
            Ellipse,
            Rectangle,
            Triangle,
            Diamond,
        }
    }
}