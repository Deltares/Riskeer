// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Core.Plugins.DotSpatial.Forms;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Views
{
    public partial class PipingFailureMechanismView :  UserControl, IMapView
    {
        private readonly BaseMap map;
        private PipingFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        public PipingFailureMechanismView()
        {
            map = new BaseMap
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(map);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as PipingFailureMechanismContext;

                if (data != null)
                {
                    SetDataToMap();
                }
            }
        }

        public IMap Map
        {
            get
            {
                return map;
            }
        }

        private void SetDataToMap()
        {
            var mapDataList = new List<MapData>();

            if (HasReferenceLinePoints())
            {
                mapDataList.Add(GetReferenceLineData());
            }

            if (HasHydraulicBoundaryLocations())
            {
                mapDataList.Add(GetHydraulicBoundaryLocations());
            }

            map.Data = new MapDataCollection(mapDataList);
        }

        private MapData GetReferenceLineData()
        {
            Point2D[] referenceLinePoints = data.Parent.ReferenceLine.Points.ToArray();
            return new MapLineData(referenceLinePoints);
        }

        private MapData GetHydraulicBoundaryLocations()
        {
            Point2D[] hrLocations = data.Parent.HydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();
            return new MapPointData(hrLocations);
        }

        private bool HasReferenceLinePoints()
        {
            return data.Parent.ReferenceLine != null && data.Parent.ReferenceLine.Points.Any();
        }

        private bool HasHydraulicBoundaryLocations()
        {
            return data.Parent.HydraulicBoundaryDatabase != null && data.Parent.HydraulicBoundaryDatabase.Locations.Any();
        }
    }
}