using System;
using System.IO;
using System.Windows.Forms;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.Exceptions;
using Core.Components.DotSpatial.Properties;
using DotSpatial.Controls;
using log4net;

namespace Core.Components.DotSpatial
{
    /// <summary>
    /// This class describes a map view
    /// </summary>
    public sealed class BaseMap : Control
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseMap));
        private MapData data;
        private Map map;

        /// <summary>
        /// Creates a new instance of <see cref="BaseMap"/>
        /// </summary>
        public BaseMap()
        {
            InitializeMapView();
        }

        /// <summary>
        /// Gets and sets the <see cref="Data"/>. When <see cref="Data"/> is not empty it will load the data on the map.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/> is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="mapData"/> does not exist.</exception>
        /// <exception cref="MapDataException">Thrown when the data in <paramref name="mapData"/> is not valid.</exception>
        public void SetMapData(MapData mapData)
        {
            if (IsDisposed)
            {
                return;
            }

            if (mapData == null)
            {
                throw new ArgumentNullException("mapData", "MapData is required when adding shapeFiles");
            }

            if (mapData.IsValid())
            {
                data = mapData;
                LoadData();
            }
            else
            {
                throw new MapDataException("The data available in MapData is not valid.");
            }
        }

        /// <summary>
        /// Initialize the <see cref="Map"/> for the <see cref="BaseMap"/>
        /// </summary>
        private void InitializeMapView()
        {
            map = new Map
            {
                Dock = DockStyle.Fill,
                FunctionMode = FunctionMode.Pan,
            };
            Controls.Add(map);
        }

        /// <summary>
        /// Loads the data from the files given in <see cref="Data"/> and shows them on the <see cref="Map"/>.
        /// </summary>
        private void LoadData()
        {
            foreach (string filePath in data.FilePaths)
            {
                map.AddLayer(filePath);

                Log.InfoFormat(Resources.BaseMap_LoadData_Shape_file_on_path__0__is_added_to_the_map_, filePath);
            }
        }
    }
}