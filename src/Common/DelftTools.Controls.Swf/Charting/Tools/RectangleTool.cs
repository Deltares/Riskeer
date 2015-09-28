using System;
using log4net;
using Steema.TeeChart;
using Steema.TeeChart.Tools;
#if WPF 
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
#else
using System.Drawing;
using System.Windows.Forms;

#endif

namespace DelftTools.Controls.Swf.Charting.Tools
{
    internal class RectangleTool : Annotation, IRectangleTool
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RectangleTool));

        private int iEdge;
        private bool iDrag;
        private Point P;

        public event RectangleToolDraggingEventHandler Dragging;
        public event RectangleToolResizingEventHandler Resizing;
        public event RectangleToolResizedEventHandler Resized;
        public event RectangleToolDraggedEventHandler Dragged;

        private bool allowResizeHeight = false;
        private bool allowResizeWidth = false;

        public bool AllowResizeHeight
        {
            get { return allowResizeHeight; }
            set { allowResizeHeight = value; }
        }

        public bool AllowResizeWidth
        {
            get { return allowResizeWidth; }
            set { allowResizeWidth = value; }
        }

        public RectangleTool() : this((Steema.TeeChart.Chart)null) {}

        public RectangleTool(Steema.TeeChart.Chart c)
            : base(c) 
        {
            AutoSize = false;
            allowDrag = true;
            allowResize = true;
            iEdge = -1;
            
            Shape.CustomPosition = true;
            Shape.Shadow.Visible = false;
            Shape.Transparency = 50;
            Shape.Left = 10;
            Shape.Top = 10;
            Shape.Width = 50;
            Shape.Height = 50;
            Shape.Bevel.Inner = Steema.TeeChart.Drawing.BevelStyles.None;
            Shape.Bevel.Outer = Steema.TeeChart.Drawing.BevelStyles.None;
            PositionUnits = PositionUnits.Pixels;
#if ! POCKET 
            Cursor = Cursors.Hand;
#endif
            P = new Point(0,0);

            Shape.Pen.Width = 3;
            Shape.Pen.Color = Color.FromArgb(200, 150, 0, 0);
            Shape.Brush.Transparency = 50;

            allowResizeWidth = true;
        }

        protected override void ChartEvent(EventArgs e)
        {
            if(e is BeforeDrawEventArgs)
            {
                if (Shape.Left < Chart.ChartRect.Left)
                {
                    Shape.Left = Chart.ChartRect.Left;
                }
                if (Shape.Left + Shape.Width > Chart.ChartRect.Right)
                {
                    Shape.Left = Chart.ChartRect.Right - Shape.Width;
                }

                // Shape.Left = chart.ChartRect.Left;
                // Shape.Width = chart.ChartRect.Width;
                if (!AllowResizeHeight)
                {
                    Shape.Top = Chart.ChartRect.Top;
                    Shape.Height = Chart.ChartRect.Height;
                }
            }
            base.ChartEvent(e);
        }

        /// <summary>
        /// Gets descriptive text.
        /// </summary>
#if DESIGNATTRIBUTES 
		[Description("Gets descriptive text.")]
#endif
        public override string Description
        {
            get
            {
                return Texts.RectangleTool;
            }
        }

        #region public properties
        private bool allowDrag;
        private bool allowResize;

#if DESIGNATTRIBUTES 
		[DefaultValue(true)]
#endif
        public bool AllowResize
        {
            get
            {
                return allowResize;
            }
            set
            {
                allowResize = value;
            }
        }

#if DESIGNATTRIBUTES 
		[DefaultValue(true)]
#endif
        public bool AllowDrag
        {
            get
            {
                return allowDrag;
            }
            set
            {
                allowDrag = value;
            }
        }
	
        #endregion

        protected virtual void OnDragging(EventArgs e)
        {
            if (Dragging != null)
                Dragging(this, e);
        }

        protected virtual void OnResizing(EventArgs e)
        {
            if (Resizing != null)
                Resizing(this, e);
        }

        protected virtual void OnResized(EventArgs e)
        {
            if (Resized != null)
                Resized(this, e);
        }

        protected virtual void OnDragged(EventArgs e)
        {
            if (Dragged != null)
                Dragged(this, e);
        }

#if WPF 
    private void StartResizing(double X, double Y)
#else
        private void StartResizing(int X, int Y)
#endif
        {
            if (iEdge == 2 || iEdge == 6 || iEdge == 7) 
                P.X = X - Shape.ShapeBounds.Right; 
            else P.X = X - Shape.Left;

            if (iEdge == 3 || iEdge == 5 || iEdge == 7) 
                P.Y = Y - Shape.ShapeBounds.Bottom; 
            else P.Y = Y - Shape.Top;

            Chart.CancelMouse = true;
        }

#if WPF 
    private void StartDragging(double X, double Y)
#else
        private void StartDragging(int X, int Y)
#endif
        {
            P.X = X - Shape.Left;
            P.Y = Y - Shape.Top;
            iDrag = true;
            Chart.CancelMouse = true;
        }

        private void StopDrag()
        {
            OnDragged(EventArgs.Empty);
            iDrag = false;
        }

        private void StopResize()
        {
            OnResized(EventArgs.Empty);
            iEdge = -1;
        }

#if WPF 
    private void TryDrag(double X, double Y)
#else
        private void TryDrag(int X, int Y)
#endif
        {
#if WPF 
      double tmpW, tmpH;
#else
            int tmpW, tmpH;
#endif

            if (X != P.X || Y != P.Y)
            {
                Shape.Left = X - P.X;
                Shape.Top = Y - P.Y;
                
                if (Shape.Left < Chart.ChartRect.Left)
                {
                    Shape.Left = Chart.ChartRect.Left;
                }
                if (Shape.Left > Chart.ChartRect.Right - Shape.Width)
                {
                    Shape.Left = Chart.ChartRect.Right - Shape.Width;
                }

                OnDragging(EventArgs.Empty);
            }
        }

#if WPF 
    private void ChangeLeft(ref double Left, double X)
#else
        private void ChangeLeft(ref int Left, int X)
#endif
        {
            if (allowResizeWidth)
            {
                Left = Math.Min(Shape.Right - 3, X - P.X);
            }
        }

#if WPF 
    private void ChangeTop(ref double Top, double Y)
#else
        private void ChangeTop(ref int Top, int Y)
#endif
        {
            if (allowResizeHeight)
            {
                Top = Math.Min(Shape.Bottom - 3, Y - P.Y);
            }
        }

#if WPF 
    private void ChangeRight(ref double Right, double X)
#else
        private void ChangeRight(ref int Right, int X)
#endif
        {
            if (allowResizeWidth)
            {
                Right = Math.Max(Shape.Left + 3, X - P.X);
            }
        }

#if WPF 
    private void ChangeBottom(ref double Bottom, double Y)
#else
        private void ChangeBottom(ref int Bottom, int Y)
#endif
        {
            if (allowResizeHeight)
            {
                Bottom = Math.Max(Shape.Top + 3, Y - P.Y);
            }
        }


#if WPF 
    private void DoResize(double X, double Y)
#else
        private void DoResize(int X, int Y)
#endif
        {
#if WPF 
      double left, right, top, bottom;
      Rect tmpR = Rect.Empty;
#else
            int left, right, top, bottom;
            Rectangle tmpR = Rectangle.Empty;
#endif

            left = Shape.Left;
            right = Shape.Right;
            top = Shape.Top;
            bottom = Shape.Bottom;
            switch (iEdge)
            {
                case 0:
                    ChangeLeft(ref left, X);

                    if (left < Chart.ChartRect.Left)
                    {
                        left = Chart.ChartRect.Left;
                    }
                    break;
                case 1:
                    ChangeTop(ref top, Y);
                    break;
                case 2:
                    ChangeRight(ref right, X);

                    if (right > Chart.ChartRect.Right)
                    {
                        right = Chart.ChartRect.Right;
                    }
                    break;
                case 3:
                    ChangeBottom(ref bottom, Y);
                    break;
                case 4:
                    ChangeLeft(ref left, X);
                    ChangeTop(ref top, Y);
                    break;
                case 5:
                    ChangeLeft(ref left, X);
                    ChangeBottom(ref bottom, Y);
                    break;
                case 6:
                    ChangeRight(ref right, X);
                    ChangeTop(ref top, Y);
                    break;
                case 7:
                    ChangeRight(ref right, X);
                    ChangeBottom(ref bottom, Y);
                    break;
            }

            tmpR = Steema.TeeChart.Utils.FromLTRB(left, top, right, bottom);
            Shape.ShapeBounds = tmpR;

            Shape.Invalidate();


            OnResizing(EventArgs.Empty);
        }

        protected override void MouseEvent(MouseEventKinds kind, MouseEventArgs e, ref Cursor c)
        {
      
            if (Active)
            {
#if WPF 
        Point tmP = e.GetPosition(Chart.Parent.GetControl());
#else
                Point tmP = new Point(e.X, e.Y);
#endif
                switch (kind)
                {
                    case MouseEventKinds.Down:
#if WPF 
            if (Utils.GetMouseButton(e) == MouseButton.Left)
#else
                        if (Steema.TeeChart.Utils.GetMouseButton(e) == MouseButtons.Left)
#endif
                        {
                            if(AllowResize)
                            {
                                iEdge = ClickedEdge(tmP.X, tmP.Y);
                            }
                            if (iEdge != -1)
                            {
                                StartResizing(tmP.X, tmP.Y);
                            }
                            else if (NearRectangle(tmP) && AllowDrag)
                            {
                                StartDragging(tmP.X, tmP.Y);
                            }
                        }
#if POCKET || WPF 
						OnClick(e);
#else
                        if (e.Clicks == 2)
                        {
                            OnDoubleClick(e);
                        }
                        else
                        {
                            OnClick(e);
                        }
#endif
                        break;
                    case MouseEventKinds.Move:
                        if (iDrag)
                        {
                            TryDrag(tmP.X, tmP.Y);
                        }
                        else
                        {
                            if (iEdge != -1)
                            {
                                DoResize(tmP.X, tmP.Y);
                            }
                            else
                            {
                                bool canGuessCursor = GuessEdgeCursor(tmP.X, tmP.Y); //also resets cursor
                                Chart.CancelMouse = canGuessCursor;
                                if (!canGuessCursor)
                                {
                                    if (NearRectangle(tmP))
                                        Chart.Parent.SetCursor(Cursors.Hand);
                                }
                            }
                        }
                        tmpX = tmP.X;
                        tmpY = tmP.Y;
                        break;
                    case MouseEventKinds.Up:
                        if (iDrag)
                            StopDrag();
                        else if (iEdge != -1)
                            StopResize();
                        break;
                }
            }
        }

        private bool NearRectangle(Point p)
        {
            Rectangle rect = Rectangle.FromLTRB(Left, Top, Left + Width, Top + Height);
            rect.Inflate(10, 0);

            return rect.Contains(p);
        }

        private bool TrySet(Cursor aCursor)
        {
            if (AllowResize)
            {
                Chart.Parent.SetCursor(aCursor);
                return true;
            }
            else return false;
        }

#if WPF 
    private bool GuessEdgeCursor(double x, double y)
#else
        private bool GuessEdgeCursor(int x, int y)
#endif
        {
            bool result=false;
#if WPF 
      Chart.parent.SetCursor(Cursors.Arrow);
#else
            Chart.Parent.SetCursor(Cursors.Default);
#endif
            switch (ClickedEdge(x, y))
            {
#if ! POCKET 
                case 0:
                    if (allowResizeWidth)
                    {
                        result = TrySet(Cursors.SizeWE);
                    }
                    break;
                case 1:
                    if (allowResizeHeight)
                    {
                        result = TrySet(Cursors.SizeNS);
                    }
                    break;
                case 2:
                    if (allowResizeWidth)
                    {
                        result = TrySet(Cursors.SizeWE);
                    }
                    break;
                case 3:
                    if (allowResizeHeight)
                    {
                        result = TrySet(Cursors.SizeNS);
                    }
                    break;
                case 4:
                    result = TrySet(Cursors.SizeNWSE);
                    break;
                case 5:
                    result = TrySet(Cursors.SizeNESW);
                    break;
                case 6:
                    result = TrySet(Cursors.SizeNESW);
                    break;
                case 7:
                    result = TrySet(Cursors.SizeNWSE);
                    break;
#endif
                default:
#if WPF 
          if (Clicked(x, y) && Cursor != Cursors.Arrow)
#else
                    if (Clicked(x, y) && Cursor != Cursors.Default)
#endif
                    {
                        result = TrySet(Cursor);
                    }
                    break;
            }

            return result;
        }

#if WPF 
    private int ClickedEdge(double x, double y)
#else
        private int ClickedEdge(int x, int y)
#endif
        {
            const int Tolerance = 4;
#if WPF 
      Rect R;
#else
            Rectangle R;
#endif
            int result = -1;

            if (Clicked(x, y))
            {
                R = Shape.ShapeBounds;
                if (Math.Abs(x - R.Left) < Tolerance)
                {
                    if (Math.Abs(y - R.Top) < Tolerance)
                    {
                        result = -1; // disable corner dragging
                        // result = 4;
                    }
                    else if (Math.Abs(y - R.Bottom) < Tolerance)
                    {
                        result = -1; // disable corner dragging
                        // result = 5;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else if (Math.Abs(y - R.Top) < Tolerance)
                {
                    if (Math.Abs(x - R.Right) < Tolerance)
                    {
                        result = -1; // disable corner dragging
                        //result = 6;
                    }
                    else
                    {
                        result = 1;
                    }
                }
                else if (Math.Abs(x - R.Right) < Tolerance)
                {
                    if (Math.Abs(y - R.Bottom) < Tolerance)
                    {
                        result = -1; // disable corner dragging
                        // result = 7;
                    }
                    else
                    {
                        result = 2;
                    }
                }
                else if (Math.Abs(y - R.Bottom) < Tolerance)
                {
                    result = 3;
                }
            }

            return result;
        }

        public void FireResisedEvent()
        {
            OnResized(EventArgs.Empty);
        }

        public void FireDraggedEvent()
        {
            OnDragged(EventArgs.Empty);
        }

        #region IChartViewTool Members

        public IChartView ChartView { get; set; }

        #endregion
        
        public bool Enabled
        {
            get;
            set;
        }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                base.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public event EventHandler<EventArgs> ActiveChanged;
    }

    public delegate void RectangleToolDraggingEventHandler(object sender, EventArgs e);

    public delegate void RectangleToolResizingEventHandler(object sender, EventArgs e);

    public delegate void RectangleToolResizedEventHandler(object sender, EventArgs e);

    public delegate void RectangleToolDraggedEventHandler(object sender, EventArgs e);
}