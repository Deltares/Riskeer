// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

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
    /// The map view
    /// </summary>
    public sealed class BaseMap : Control, IMap
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
        /// <exception cref="ArgumentNullException">Thrown when <see cref="MapData"/> is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <see cref="MapData"/> does not exist.</exception>
        /// <exception cref="MapDataException">Thrown when the data in <see cref="MapData"/> is not valid.</exception>
        public MapData Data
        {
            get
            {
                return data;
            }
            set
            {
                if (IsDisposed)
                {
                    return;
                }

                if (value == null)
                {
                    throw new ArgumentNullException("MapData", "MapData is required when adding shapeFiles");
                }

                if (value.IsValid())
                {
                    data = value;
                    LoadData();
                }
                else
                {
                    throw new MapDataException(Resources.BaseMap_SetMapData_The_data_available_in_MapData_is_not_valid_);
                }
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