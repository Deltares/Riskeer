// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;

namespace Demo.Riskeer.Views
{
    /// <summary>
    /// This class represents a simple view with a map, to which data can be added.
    /// </summary>
    public partial class MapDataView : UserControl, IMapView
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapDataView"/>.
        /// </summary>
        public MapDataView()
        {
            InitializeComponent();

            ((MapControl)Map).BackgroundMapData = new WmtsMapData("Test",
                                                                  "http://geodata.nationaalgeoregister.nl/tiles/service/wmts?request=GetCapabilities&service=WMTS",
                                                                  "brtachtergrondkaart(EPSG:28992)",
                                                                  "image/png");
        }

        public object Data
        {
            get
            {
                return Map.Data;
            }
            set
            {
                mapControl.Data = value as MapDataCollection;
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