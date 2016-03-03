using System.Windows.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis;
using Core.Components.Gis.Forms;

namespace Core.Plugins.DotSpatial.Test
{
    /// <summary>
    /// Simple <see cref="Core.Components.Gis.Forms.IMapView"/> implementation which can be used in tests.
    /// </summary>
    public class TestMapView : Control, IMapView
    {
        public object Data { get; set; }

        public IMap Map
        {
            get
            {
                return (BaseMap) Data;
            }
        }
    }
}