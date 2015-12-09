using System.Drawing;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools.Zooming;

namespace Core.GIS.SharpMapTestUtil
{
    /// <summary>
    /// Provides common GIS testing functionality.
    /// </summary>
    public class MapTestHelper : Form // TODO: incapsulate Form as a local variable in ShowMap()
    {
        private readonly Label coordinateLabel;

        public MapTestHelper()
        {
            MapControl = new MapControl();
            MapControl.Dock = DockStyle.Fill;
            // disable dragdrop because it breaks the test runtime
            MapControl.AllowDrop = false;

            Controls.Add(MapControl);

            coordinateLabel = new Label();
            coordinateLabel.Width = 500;
            coordinateLabel.Location = new Point(5, 5);

            MapControl.Controls.Add(coordinateLabel);
            MapControl.Resize += delegate { MapControl.Refresh(); };
            MapControl.ActivateTool(MapControl.GetToolByType<PanZoomTool>());

            MapControl.ActivateTool(MapControl.SelectTool);
        }

        public MapControl MapControl { get; set; }

        /// <summary>
        /// Method show a new map control.
        /// </summary>
        /// <param name="map"></param>
        public static void ShowModal(Map map)
        {
            new MapTestHelper().ShowMap(map);
        }

        public void ShowMap(Map map)
        {
            MapControl.Map = map;

            map.ZoomToExtents();

            MapControl.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                var point = map.ImageToWorld(new PointF(e.X, e.Y));
                coordinateLabel.Text = string.Format("{0}:{1}", point.X, point.Y);
            };

            WindowsFormsTestHelper.ShowModal(this);

            map.Dispose();
        }
    }
}