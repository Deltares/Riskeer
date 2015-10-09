using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils.Aop;
using GeoAPI.Geometries;

namespace SharpMap.UI.Tools.Decorations
{
    /// <summary>
    /// A base class for layout-related components on a map such as a legend, scale bar or north arrow.
    /// It implements drag and drop moving of the component.
    /// </summary>
    [Entity]
    public abstract class LayoutComponentTool : MapTool
    {
        private const int Margin = 5;

        protected Point screenLocation;
        protected Size size;
        protected Color SelectionColor = Color.FromArgb(80, 135, 206, 250);
        private Point mouseSelectStart = new Point(-1, -1);
        private AnchorStyles anchor;

        private bool componentDragging; // True if the user is dragging the layout component to move it
        private Point dragOffset; // The offset from the top-left of the bitmap to the actual mouse click point
        private Size oldMapSize; // Store the 'old' size of the map to compare to changes in the map size
        private bool visible;
        private int backGroundTransparencyPercentage = 50;

        public LayoutComponentTool()
        {
            screenLocation = new Point(0, 0);
            visible = true;
            UseAnchor = true;
        }

        public override bool RendersInScreenCoordinates
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the anchoring of the component, sticking to 1 or more map control edges
        /// </summary>
        public AnchorStyles Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
                UseAnchor = true;
            }
        }

        public int BackGroundTransparencyPercentage
        {
            get
            {
                return backGroundTransparencyPercentage;
            }
            set
            {
                if (value < 0)
                {
                    backGroundTransparencyPercentage = 0;
                }
                else if (value > 100)
                {
                    backGroundTransparencyPercentage = 100;
                }
                else
                {
                    backGroundTransparencyPercentage = value;
                }
                MapControl.Invalidate(MapControl.ClientRectangle);
            }
        }

        /// <summary>
        /// The location of the component on the screen, relative to the top-left of the map control.
        /// </summary>
        public Point ScreenLocation
        {
            get
            {
                return screenLocation;
            }
            set
            {
                screenLocation = value;
            }
        }

        /// <summary>
        /// The size of the component on the screen, in pixels
        /// </summary>
        public virtual Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        /// <summary>
        /// If the layout component should be drawn to the screen
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Returns the rectangle of this component on the screen
        /// </summary>
        public Rectangle ScreenRectangle
        {
            get
            {
                return new Rectangle(screenLocation, Size);
            }
        }

        /// <summary>
        /// Correct the new screen location to the actual visible control area.
        /// </summary>
        protected void CorrectScreenLocation()
        {
            // Check boundaries
            if ((screenLocation.X + Size.Width) > (Map.Size.Width - Margin))
            {
                screenLocation.X = Map.Size.Width - Size.Width - Margin;
            }
            if ((screenLocation.Y + Size.Height) > (Map.Size.Height - Margin))
            {
                screenLocation.Y = Map.Size.Height - Size.Height - Margin;
            }
            if (screenLocation.X < Margin)
            {
                screenLocation.X = Margin;
            }
            if (screenLocation.Y < Margin)
            {
                screenLocation.Y = Margin;
            }
        }

        protected Color GetBackGroundColor()
        {
            var alpha = (int) Math.Round(255.0*((100 - backGroundTransparencyPercentage)/100.0));
            var backColor = Map != null ? Map.BackColor : Color.White;

            return Color.FromArgb(alpha, backColor);
        }

        /// <summary>
        /// When the map's Size property is changed, adjust our screen location according to the anchoring.
        /// </summary>
        private void ReflectMapSizeChanges()
        {
            if (oldMapSize == Map.Size)
            {
                return;
            }

            // First time, get the current map size
            if (oldMapSize.Height == 0 && oldMapSize.Width == 0)
            {
                oldMapSize = Map.Size;
            }

            if (UseAnchor)
            {
                SetScreenLocationForAnchor();
            }

            CorrectScreenLocation();

            // Store the new size for future comparison
            oldMapSize = Map.Size;

            DoDrawing(true);
        }

        private int GetOffSet(AnchorStyles anchorStyle)
        {
            if (!anchorStyle.HasFlag(AnchorStyles.Left) && !anchorStyle.HasFlag(AnchorStyles.Right))
            {
                return 0;
            }

            var toolsBeforeThisOne = MapControl.Tools.Take(MapControl.Tools.IndexOf(this) + 1).OfType<LayoutComponentTool>();
            var layoutComponentTools = toolsBeforeThisOne.Where(t => t.Visible && t.UseAnchor && t.Anchor.HasFlag(anchorStyle) && t != this);

            var offSet = layoutComponentTools.Sum(t => t.Size.Width);

            return offSet;
        }

        #region IMapTool Members

        /// <summary>
        /// A layout component is always active (returning true), showing the control on the screen and 
        /// allowing interactions with it.
        /// </summary>
        public override bool AlwaysActive
        {
            get
            {
                return true;
            }
        }

        public bool UseAnchor { get; set; }

        public bool Selected { get; private set; }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!Selected || e.KeyCode != Keys.Delete)
            {
                return;
            }

            Selected = false;
            Visible = false;
            Map.Render(); // needed for refresh of ribbon buttons :(
            MapControl.Invalidate(MapControl.ClientRectangle);
        }

        /// <summary>
        /// When the left mouse button is clicked on the layout component and the move tool was 
        /// selected, start dragging. 
        /// Ignore when another feature is already selected.
        /// </summary>
        /// <param name="worldPosition">The world location clicked</param>
        /// <param name="e">The mouse state</param>
        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            var bounds = new Rectangle(ScreenLocation, Size);

            if (!Visible || e.Button != MouseButtons.Left || !bounds.Contains(e.Location))
            {
                base.OnMouseDown(worldPosition, e);
                return;
            }

            if (MapControl.MoveTool.IsActive || MapControl.SelectTool.IsActive)
            {
                mouseSelectStart = e.Location;
            }

            if (Selected && MapControl.MoveTool.IsActive)
            {
                MapControl.MoveTool.MovingLayoutComponent = true;

                var mouseDownLocation = Map.WorldToImage(worldPosition);
                dragOffset = new Point((int) mouseDownLocation.X - screenLocation.X, (int) mouseDownLocation.Y - screenLocation.Y);
                componentDragging = true;
            }

            base.OnMouseDown(worldPosition, e);
        }

        /// <summary>
        /// While dragging (left mouse button), adjust the component screen location.
        /// </summary>
        /// <param name="worldPosition">The new location</param>
        /// <param name="e">The mouse state</param>
        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (Visible && componentDragging)
            {
                // Adjust the location of this layout component
                PointF newLocation = Map.WorldToImage(worldPosition);
                screenLocation.X = (int) newLocation.X - dragOffset.X;
                screenLocation.Y = (int) newLocation.Y - dragOffset.Y;

                UseAnchor = false;

                CorrectScreenLocation();

                StartDrawing();
                DoDrawing(true);
                StopDrawing();
            }

            base.OnMouseMove(worldPosition, e);
        }

        /// <summary>
        /// When the left mouse button was released, stop the dragging of the layout component.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (componentDragging)
            {
                MapControl.MoveTool.MovingLayoutComponent = false;
                componentDragging = false;
            }

            Selected = mouseSelectStart == e.Location && !MapControl.SelectTool.Selection.Any();

            base.OnMouseUp(worldPosition, e);
        }

        /// <summary>
        /// Painting the component includes correcting the component's screen location to any map size changes.
        /// </summary>
        /// <param name="e"></param>
        public override void OnPaint(PaintEventArgs e)
        {
            // TODO: Instead of doing this all the time it should rather do this when the map's size has actually changed
            ReflectMapSizeChanges();

            if (Visible)
            {
                // Rendering of the visual appearance of this component
                Render(e.Graphics, MapControl.Map);
            }
        }

        /// <summary>
        /// Set the initial screenlocation based on anchorstyles
        /// </summary>
        /// <returns></returns>
        public bool SetScreenLocationForAnchor()
        {
            if (Map == null || !UseAnchor)
            {
                return false;
            }

            var point = new Point(Margin, Margin);

            if (Anchor.HasFlag(AnchorStyles.Left))
            {
                point.X += GetOffSet(Anchor);
            }

            if (Anchor.HasFlag(AnchorStyles.Bottom))
            {
                point.Y = Map.Size.Height - Size.Height - Margin;
            }

            if (Anchor.HasFlag(AnchorStyles.Right))
            {
                point.X = Map.Size.Width - Size.Width - Margin - GetOffSet(Anchor);
            }

            screenLocation = point;

            MapControl.Invalidate(MapControl.ClientRectangle);

            return true;
        }

        #endregion
    }
}