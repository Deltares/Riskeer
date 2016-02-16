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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;

using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;

using DotSpatial.Controls;
using DotSpatial.Data;

using IMap = Core.Components.Gis.IMap;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// This class describes a map view with configured projection and function mode.
    /// </summary>
    public sealed class BaseMap : Control, IMap
    {
        private readonly MapDataFactory mapDataFactory = new MapDataFactory();
        private MapData data;
        private Map map;

        /// <summary>
        /// Creates a new instance of <see cref="BaseMap"/>.
        /// </summary>
        public BaseMap()
        {
            InitializeMapView();
        }

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
                foreach (FeatureSet featureSet in mapDataFactory.Create(data))
                {
                    map.Layers.Add(featureSet);
                }
            }
        }

        private void InitializeMapView()
        {
            map = new Map
            {
                ProjectionModeDefine = ActionMode.Never,
                Dock = DockStyle.Fill,
                FunctionMode = FunctionMode.Pan
            };

            Controls.Add(map);
        }
    }
}