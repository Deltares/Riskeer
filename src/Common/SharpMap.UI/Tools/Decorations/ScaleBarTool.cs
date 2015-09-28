using System;
using System.Drawing;
using System.Windows.Forms;
using GeoAPI.Geometries;
using SharpMap.CoordinateSystems.Transformations;

namespace SharpMap.UI.Tools.Decorations
{
    /// <summary>
    /// When this tool is active it displays a scalebar on the mapcontrol.
    /// </summary>
    public class ScaleBarTool : LayoutComponentTool
    {
        private ScaleBar bar;
        private bool initScreenPosition = false;

        /// <summary>
        /// Creates the scale bar layout component.
        /// </summary>
        /// <param name="mapControl">The map control it operates on</param>
        public ScaleBarTool()
        {
            Name = "ScaleBar";
            bar = new ScaleBar
                      {
                          BarUnit = MapUnits.ws_muMeter,
                          MapUnit = MapUnits.ws_muMeter,
                          AlignMent = StringAlignment.Near
                      };

            //bar.BorderVisible = true;
            //this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;           
        }



        /// <summary>
        /// Draws a scalebar on the screen.
        /// </summary>
        public override void Render(Graphics graphics, Map mapBox)
        {
            if (Visible)
            {
                if (!initScreenPosition)
                {
                    initScreenPosition = SetScreenLocationForAnchor();
                }
                
                graphics.FillRectangle(new SolidBrush(GetBackGroundColor()), new Rectangle(screenLocation, ScreenRectangle.Size));

                if (Selected)
                {
                    graphics.FillRectangle(new SolidBrush(SelectionColor), new Rectangle(screenLocation, ScreenRectangle.Size));
                }

                // Get the current map scale
                double meters = GetSegmentInMeters();

                //display km scale on bar if map has small scale 
                bar.BarUnit = meters<5000 ? MapUnits.ws_muMeter : MapUnits.ws_muKilometer;

                if (meters > 0) // A valid scale was found
                {
                    bar.SetScale(meters, size.Width);
                    bar.DrawTheControl(graphics, ScreenRectangle);
                    Size = ScreenRectangle.Size;
                }
            }
            base.Render(graphics, mapBox);
        }

        /// <summary>
        /// Calculate the width of one scale bar segment, based on the current map zoom factor.
        /// </summary>
        /// <returns>The segment width in meters</returns>
        // TODO: Only recalculate the segment when the zoom level of the map changes
        private double GetSegmentInMeters()
        {
            // Get the beginnign and ending world coordinates of a virtual line width the lenght of 
            // this scale bar size.Width in pixels.
            var measure0 = Map.ImageToWorld(new PointF(screenLocation.X, screenLocation.Y));
            var measure1 = Map.ImageToWorld(new PointF(screenLocation.X + size.Width, screenLocation.Y + 1));

            if (Map.CoordinateSystem != null)
                return GeodeticDistance.Distance(Map.CoordinateSystem, measure0, measure1);
            return Math.Sqrt(Math.Pow(measure1.X - measure0.X, 2) + Math.Pow(measure1.Y - measure0.Y, 2));
        }

        private Point GetInitScreenLocation()
        {
            int margin = 5;

            var point = new Point(margin, margin);

            if ((Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
            {
                point.Y = Map.Size.Height - size.Height - margin;
            }

            if ((Anchor & AnchorStyles.Right) == AnchorStyles.Right)
            {
                point.X = Map.Size.Width - size.Width - margin;
            }

            return point;
        }
    }
}