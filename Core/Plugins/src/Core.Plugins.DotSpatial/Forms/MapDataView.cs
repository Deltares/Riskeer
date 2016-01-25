﻿using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial;
using Core.Components.DotSpatial.Data;

namespace Core.Plugins.DotSpatial.Forms
{
    /// <summary>
    /// This class describes the user control for the Map.
    /// </summary>
    public partial class MapDataView : UserControl, IView
    {
        private readonly BaseMap baseMap;
        private MapData data;

        /// <summary>
        /// Creates a new instance of MapDataView and adds the <see cref="BaseMap"/> to the <see cref="Control.Controls"/>.
        /// </summary>
        public MapDataView()
        {
            baseMap = new BaseMap
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(baseMap);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (MapData) value;
                baseMap.SetMapData(data);
            }
        }
    }
}