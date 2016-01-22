using System;
using System.IO;
using System.Windows.Forms;
using Core.Components.DotSpatial.Data;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseMap));
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
        /// <exception cref="ArgumentException">Thrown when <paramref name="mapData"/> is invalid.</exception>
        public void SetMapData(MapData mapData)
        {
            try
            {
                if (mapData.IsValid())
                {
                    data = mapData;
                    LoadData();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not set the data on the map.", e.Message);
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

                log.InfoFormat(Resources.BaseMap_LoadData_Shape_file_on_path__0__is_added_to_the_map_, filePath);
            }
        }
    }
}