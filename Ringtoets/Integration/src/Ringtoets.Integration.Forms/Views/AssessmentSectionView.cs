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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a simple view with a map, to which data can be added. 
    /// </summary>
    public partial class AssessmentSectionView : UserControl, IMapView, IObserver
    {
        private readonly MapControl mapControl;
        private IAssessmentSection data;

        private MapData hydraulicBoundaryDatabaseLocations;
        private MapData referenceLineData;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionView"/>.
        /// </summary>
        public AssessmentSectionView()
        {
            mapControl = new MapControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(mapControl);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as IAssessmentSection;

                if (data != null)
                {
                    data.Detach(this);
                    SetDataToMap();
                    data.Attach(this);
                }
            }
        }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }

        public void UpdateObserver()
        {
            if (data != null)
            {
                SetDataToMap();
            }
        }

        private void SetDataToMap()
        {
            mapControl.Data.Name = Resources.AssessmentSectionMap_DisplayName;
            
            if (data != null)
            {
                // Bottommost layer
                hydraulicBoundaryDatabaseLocations = AddOrUpdateMapData(hydraulicBoundaryDatabaseLocations, GetHydraulicBoundaryLocations);
                referenceLineData = AddOrUpdateMapData(referenceLineData, GetReferenceLineData);
                // Topmost layer
            }

            mapControl.Data.NotifyObservers();

        }

        private MapData AddOrUpdateMapData(MapData oldMapData, Func<MapData> creationMethod)
        {
            MapData newMapData = creationMethod();

            if (oldMapData == null)
            {
                mapControl.Data.Add(newMapData);
            }
            else
            {
                mapControl.Data.Replace(oldMapData, newMapData);
            }

            return newMapData;
        }

        private MapData GetReferenceLineData()
        {
            ReferenceLine referenceLine = data.ReferenceLine;
            IEnumerable<Point2D> referenceLinePoints = referenceLine == null ?
                                                           Enumerable.Empty<Point2D>() :
                                                           referenceLine.Points;
            return new MapLineData(GetMapFeature(referenceLinePoints), RingtoetsCommonDataResources.ReferenceLine_DisplayName);
        }

        private MapData GetHydraulicBoundaryLocations()
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.HydraulicBoundaryDatabase;
            IEnumerable<Point2D> hrLocations = hydraulicBoundaryDatabase == null ?
                                                   Enumerable.Empty<Point2D>() :
                                                   hydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();
            return new MapPointData(GetMapFeature(hrLocations), RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName);
        }

        private IEnumerable<MapFeature> GetMapFeature(IEnumerable<Point2D> points)
        {
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(points)
                })
            };
            return features;
        }
    }
}