// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;

namespace Core.Plugins.Map.Test
{
    /// <summary>
    /// Simple <see cref="IMapView"/> implementation which can be used in tests.
    /// </summary>
    public class TestMapView : Control, IMapView
    {
        private readonly MapControl mapControl;

        /// <summary>
        /// Creates a new instance of <see cref="TestMapView"/> and initializes some <see cref="MapData"/>.
        /// </summary>
        public TestMapView()
        {
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(new MapPointData("test points")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(2, 2)
                            }
                        })
                    }),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(4, 4)
                            }
                        })
                    })
                }
            });

            mapControl = new MapControl
            {
                Data = mapDataCollection
            };

            Controls.Add(mapControl);
        }

        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }
    }
}