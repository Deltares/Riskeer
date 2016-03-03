using System.Windows.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Forms;

namespace Core.Plugins.DotSpatial.Test
{
    /// <summary>
    /// Simple <see cref="IMapView"/> implementation which can be used in tests.
    /// </summary>
    public class TestMapView : Control, IMapView
    {
        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return (MapControl) Data;
            }
        }
    }
}