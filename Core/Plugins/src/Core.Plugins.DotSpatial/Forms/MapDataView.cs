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
using Core.Components.DotSpatial;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis;
using Core.Components.Gis.Data;

namespace Core.Plugins.DotSpatial.Forms
{
    /// <summary>
    /// This class represents a simple view with a map, to which data can be added.
    /// </summary>
    public partial class MapDataView : UserControl, IMapView
    {
        private readonly BaseMap baseMap;
        private MapData data;

        /// <summary>
        /// Creates a new instance of <see cref="MapDataView"/>.
        /// </summary>
        public MapDataView()
        {
            baseMap = new BaseMap
            {
                Dock = DockStyle.Fill
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

                if (data != null)
                {
                    Map.Data = data;
                }
            }
        }

        public IMap Map
        {
            get
            {
                return baseMap;
            }
        }
    }
}