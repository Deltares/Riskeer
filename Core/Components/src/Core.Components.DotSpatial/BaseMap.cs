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

using System.Windows.Forms;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.Data;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial
{
    /// <summary>
    /// The map view
    /// </summary>
    public sealed class BaseMap : Control, IMap
    {
        private readonly MapDataFactory mapDataFactory = new MapDataFactory();
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

                data = value;
                DrawFeatureSets();
            }
        }

        private void DrawFeatureSets()
        {
            map.ClearLayers();
            if (data != null)
            {
                map.Layers.Add(mapDataFactory.Create(data));
            }
        }

        private void InitializeMapView()
        {
            map = new Map
            {
                ProjectionModeDefine = ActionMode.Never,
                Dock = DockStyle.Fill,
                FunctionMode = FunctionMode.Pan,
            };
            Controls.Add(map);
        }
    }
}