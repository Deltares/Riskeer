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

using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView, IObserver
    {
        private PipingFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        public PipingFailureMechanismView()
        {
            InitializeComponent();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                var newValue = value as PipingFailureMechanismContext;

                DetachFromData();
                data = newValue;
                SetDataToMap();
                AttachToData();
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

        private void AttachToData()
        {
            if (data != null)
            {
                data.Parent.Attach(this);
                var surfaceLines = data.WrappedData.SurfaceLines as IObservable;
                if (surfaceLines != null)
                {
                    surfaceLines.Attach(this);
                }
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Parent.Detach(this);
                var surfaceLines = data.WrappedData.SurfaceLines as IObservable;
                if (surfaceLines != null)
                {
                    surfaceLines.Detach(this);
                }
            }
        }

        private void SetDataToMap()
        {
            var mapDataList = new List<MapData>();

            if (data != null)
            {
                // Bottom most layer
                mapDataList.Add(GetSurfaceLinesMapData());
                mapDataList.Add(GetSectionsMapData());
                mapDataList.Add(GetSectionsStartPointsMapData());
                mapDataList.Add(GetSectionsEndPointsMapData());
                mapDataList.Add(GetHydraulicBoundaryLocationsMapData());
                mapDataList.Add(GetReferenceLineMapData());
                // Topmost layer
            }

            mapControl.Data = new MapDataCollection(mapDataList, PipingDataResources.PipingFailureMechanism_DisplayName);
        }

        private MapData GetReferenceLineMapData()
        {
            ReferenceLine referenceLine = data.Parent.ReferenceLine;
            IEnumerable<Point2D> referenceLinePoints = referenceLine == null ?
                                                           Enumerable.Empty<Point2D>() :
                                                           referenceLine.Points;
            return new MapLineData(GetMapFeature(referenceLinePoints), RingtoetsCommonDataResources.ReferenceLine_DisplayName);
        }

        private MapData GetHydraulicBoundaryLocationsMapData()
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;

            IEnumerable<Point2D> hrLocations = hydraulicBoundaryDatabase == null ?
                                                   Enumerable.Empty<Point2D>() :
                                                   hydraulicBoundaryDatabase.Locations.Select(h => h.Location);
            return new MapPointData(GetMapFeature(hrLocations), RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName);
        }

        private MapData GetSurfaceLinesMapData()
        {
            IEnumerable<IEnumerable<Point2D>> surfaceLines = data.WrappedData.SurfaceLines.Select(sl => sl.Points.Select(p => new Point2D(p.X, p.Y)));
            return new MapMultiLineData(surfaceLines, RingtoetsCommonDataResources.SurfaceLine_DisplayName);
        }

        private MapData GetSectionsMapData()
        {
            IEnumerable<IEnumerable<Point2D>> sectionLines = data.WrappedData.Sections.Select(sl => sl.Points);
            return new MapMultiLineData(sectionLines, Resources.FailureMechanism_Sections_DisplayName);
        }

        private MapData GetSectionsStartPointsMapData()
        {
            IEnumerable<Point2D> startPoints = data.WrappedData.Sections.Select(sl => sl.GetStart());
            string mapDataName = string.Format("{0} ({1})",
                                               Resources.FailureMechanism_Sections_DisplayName,
                                               Resources.FailureMechanismSections_StartPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName);
        }

        private MapData GetSectionsEndPointsMapData()
        {
            IEnumerable<Point2D> startPoints = data.WrappedData.Sections.Select(sl => sl.GetLast());
            string mapDataName = string.Format("{0} ({1})",
                                               Resources.FailureMechanism_Sections_DisplayName,
                                               Resources.FailureMechanismSections_EndPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName);
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