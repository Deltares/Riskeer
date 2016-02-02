using System.Windows.Forms;
using Core.Components.DotSpatial;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial.Test
{
    /// <summary>
    /// Simple <see cref="IMapView"/> implementation which can be used in tests.
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
