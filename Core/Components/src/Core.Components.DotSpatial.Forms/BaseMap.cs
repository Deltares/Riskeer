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
using Core.Common.Base;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using DotSpatial.Controls;
using IMap = Core.Components.Gis.IMap;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// This class describes a map view with configured projection and function mode.
    /// </summary>
    public sealed class BaseMap : Control, IMap, IObserver
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

                DetachFromData();
                data = value;
                AttachToData();
                DrawFeatureSets();
            }
        }

        public void ZoomToAll()
        {
            map.ZoomToMaxExtent();
        }

        public void UpdateObserver()
        {
            DrawFeatureSets();
        }

        /// <summary>
        /// Attaches the <see cref="BaseMap"/> to the currently set <see cref="Data"/>, if there is any.
        /// </summary>
        private void AttachToData()
        {
            if (data != null)
            {
                data.Attach(this);
            }
        }

        /// <summary>
        /// Detaches the <see cref="BaseMap"/> to the currently set <see cref="Data"/>, if there is any.
        /// </summary>
        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
        }

        private void DrawFeatureSets()
        {
            map.ClearLayers();
            if (data != null)
            {
                foreach (IMapFeatureLayer mapLayer in mapDataFactory.Create(data))
                {
                    map.Layers.Add(mapLayer);
                }
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