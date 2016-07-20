using System.Windows.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;

namespace Demo.Ringtoets.Views
{
    /// <summary>
    /// This class represents a simple view with a map, to which data can be added.
    /// </summary>
    public partial class MapDataView : UserControl, IMapView
    {
        private MapDataCollection data;

        /// <summary>
        /// Creates a new instance of <see cref="MapDataView"/>.
        /// </summary>
        public MapDataView()
        {
            InitializeComponent();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (MapDataCollection)value;

                if (data != null)
                {
                    foreach (var mapData in data.List)
                    {
                        Map.Data.Add(mapData);
                    }

                    Map.Data.Name = data.Name;
                    Map.Data.NotifyObservers();
                }
            }
        }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }
    }
}
