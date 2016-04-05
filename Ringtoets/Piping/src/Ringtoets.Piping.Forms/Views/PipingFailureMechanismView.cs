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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView, IObserver
    {
        private PipingFailureMechanismContext data;

        private MapData hydraulicBoundaryDatabaseLocations;
        private MapData referenceLineData;
        private MapData surfaceLinesMapData;
        private MapData sectionsMapData;
        private MapData sectionsStartPointsMapData;
        private MapData sectionsEndPointMapData;

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
                data.WrappedData.Attach(this);
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Parent.Detach(this);
                data.WrappedData.Detach(this);
            }
        }

        private void SetDataToMap()
        {
            mapControl.Data.Name = PipingDataResources.PipingFailureMechanism_DisplayName;

            if (data != null)
            {
                // Bottom most layer
                referenceLineData = AddOrUpdateMapData(referenceLineData, GetReferenceLineMapData());
                surfaceLinesMapData = AddOrUpdateMapData(surfaceLinesMapData, GetSurfaceLinesMapData());
                sectionsMapData = AddOrUpdateMapData(sectionsMapData, GetSectionsMapData());
                sectionsStartPointsMapData = AddOrUpdateMapData(sectionsStartPointsMapData, GetSectionsStartPointsMapData());
                sectionsEndPointMapData = AddOrUpdateMapData(sectionsEndPointMapData, GetSectionsEndPointsMapData());
                hydraulicBoundaryDatabaseLocations = AddOrUpdateMapData(hydraulicBoundaryDatabaseLocations, GetHydraulicBoundaryLocationsMapData());
                // Topmost layer
            }

            mapControl.Data.NotifyObservers();
        }

        private MapData AddOrUpdateMapData(MapData oldMapData, MapData newMapData)
        {
            if (oldMapData != null)
            {
                mapControl.Data.Remove(oldMapData);
            }
            if (newMapData != null)
            {
                mapControl.Data.Add(newMapData);
            }

            return newMapData;
        }

        private MapData GetReferenceLineMapData()
        {
            if (data == null || data.Parent == null || data.Parent.ReferenceLine == null)
            {
                return PipingMapDataFactory.CreateEmptyLineData(RingtoetsCommonDataResources.ReferenceLine_DisplayName);
            }
            return PipingMapDataFactory.Create(data.Parent.ReferenceLine);
        }

        private MapData GetHydraulicBoundaryLocationsMapData()
        {
            if (data == null || data.Parent == null || data.Parent.HydraulicBoundaryDatabase == null)
            {
                return PipingMapDataFactory.CreateEmptyPointData(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName);
            }
            return PipingMapDataFactory.Create(data.Parent.HydraulicBoundaryDatabase);
        }

        private MapData GetSurfaceLinesMapData()
        {
            if (data == null || data.WrappedData == null || data.WrappedData.SurfaceLines == null || !data.WrappedData.SurfaceLines.Any())
            {
                return PipingMapDataFactory.CreateEmptyLineData(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName);
            }
            return PipingMapDataFactory.Create(data.WrappedData.SurfaceLines);
        }

        private MapData GetSectionsMapData()
        {
            if (data == null || data.WrappedData == null || data.WrappedData.Sections == null || !data.WrappedData.Sections.Any())
            {
                return PipingMapDataFactory.CreateEmptyLineData(Resources.FailureMechanism_Sections_DisplayName);
            }
            return PipingMapDataFactory.Create(data.WrappedData.Sections);
        }

        private MapData GetSectionsStartPointsMapData()
        {
            if (data == null || data.WrappedData == null || data.WrappedData.Sections == null || !data.WrappedData.Sections.Any())
            {
                string mapDataName = string.Format("{0} ({1})",
                                                   Resources.FailureMechanism_Sections_DisplayName,
                                                   Resources.FailureMechanismSections_StartPoints_DisplayName);
                return PipingMapDataFactory.CreateEmptyPointData(mapDataName);
            }
            return PipingMapDataFactory.CreateStartPoints(data.WrappedData.Sections);
        }

        private MapData GetSectionsEndPointsMapData()
        {
            if (data == null || data.WrappedData == null || data.WrappedData.Sections == null || !data.WrappedData.Sections.Any())
            {
                string mapDataName = string.Format("{0} ({1})",
                                                   Resources.FailureMechanism_Sections_DisplayName,
                                                   Resources.FailureMechanismSections_EndPoints_DisplayName);
                return PipingMapDataFactory.CreateEmptyPointData(mapDataName);
            }
            return PipingMapDataFactory.CreateEndPoints(data.WrappedData.Sections);
        }
    }

    public static class PipingMapDataFactory
    {
        public static MapData Create(ICollection<RingtoetsPipingSurfaceLine> surfaceLines)
        {
            var mapFeatures = new List<MapFeature>
            {
                new MapFeature(surfaceLines.Select(surfaceLine => new MapGeometry(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)))))
            };

            return new MapLineData(mapFeatures, PipingFormsResources.PipingSurfaceLinesCollection_DisplayName)
            {
                Style = new LineStyle(Color.DarkSeaGreen, 2, DashStyle.Solid)
            };
        }

        public static MapData Create(IEnumerable<FailureMechanismSection> sections)
        {
            var mapFeatures = new List<MapFeature>
            {
                new MapFeature(sections.Select(section => new MapGeometry(section.Points.Select(p => new Point2D(p.X, p.Y)))))
            };

            return new MapLineData(mapFeatures, Resources.FailureMechanism_Sections_DisplayName)
            {
                Style = new LineStyle(Color.Khaki, 3, DashStyle.Dot)
            };
        }

        public static MapData CreateStartPoints(IEnumerable<FailureMechanismSection> sections)
        {

            IEnumerable<Point2D> startPoints = sections.Select(sl => sl.GetStart());
            string mapDataName = string.Format("{0} ({1})",
                                               Resources.FailureMechanism_Sections_DisplayName,
                                               Resources.FailureMechanismSections_StartPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }

        public static MapData CreateEndPoints(IEnumerable<FailureMechanismSection> sections)
        {
            IEnumerable<Point2D> startPoints = sections.Select(sl => sl.GetLast());
            string mapDataName = string.Format("{0} ({1})",
                                               Resources.FailureMechanism_Sections_DisplayName,
                                               Resources.FailureMechanismSections_EndPoints_DisplayName);
            return new MapPointData(GetMapFeature(startPoints), mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, 15, PointSymbol.Triangle)
            };
        }

        public static MapData Create(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException();
            }
            var features = GetMapFeature(referenceLine.Points);

            return new MapLineData(features, RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, 3, DashStyle.Solid)
            };
        }

        public static MapData Create(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            IEnumerable<Point2D> locations = hydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();

            var features = GetMapFeature(locations);

            return new MapPointData(features, RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, 6, PointSymbol.Circle)
            };
        }

        private static IEnumerable<MapFeature> GetMapFeature(IEnumerable<Point2D> points)
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

        public static MapData CreateEmptyLineData(string name)
        {
            return new MapLineData(Enumerable.Empty<MapFeature>(), name);
        }

        public static MapData CreateEmptyPointData(string name)
        {
            return new MapPointData(Enumerable.Empty<MapFeature>(), name);
        }
    }
}