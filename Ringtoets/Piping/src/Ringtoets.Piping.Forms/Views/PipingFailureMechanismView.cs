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
using Core.Components.Gis.Forms;
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
                if (HasReferenceLinePoints())
                {
                    mapDataList.Add(GetReferenceLineMapData());
                }

                if (HasHydraulicBoundaryLocations())
                {
                    mapDataList.Add(GetHydraulicBoundaryLocationsMapData());
                }

                if (HasSections())
                {
                    mapDataList.Add(GetSectionsMapData());
                }

                if (HasSurfaceLines())
                {
                    mapDataList.Add(GetSurfaceLinesMapData());
                }
            }

            mapControl.Data = new MapDataCollection(mapDataList, PipingDataResources.PipingFailureMechanism_DisplayName);        }

        private MapData GetReferenceLineMapData()
        {
            Point2D[] referenceLinePoints = data.Parent.ReferenceLine.Points.ToArray();
            return new MapLineData(referenceLinePoints, RingtoetsCommonDataResources.ReferenceLine_DisplayName);
        }

        private MapData GetHydraulicBoundaryLocationsMapData()
        {
            Point2D[] hrLocations = data.Parent.HydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();
            return new MapPointData(hrLocations, RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName);
        }

        private MapData GetSurfaceLinesMapData()
        {
            IEnumerable<IEnumerable<Point2D>> surfaceLines = data.WrappedData.SurfaceLines.Select(sl => sl.Points.Select(p => new Point2D(p.X, p.Y)));
            return new MapMultiLineData(surfaceLines, RingtoetsCommonDataResources.SurfaceLine_DisplayName);
        }

        private MapData GetSectionsMapData()
        {
            IEnumerable<IEnumerable<Point2D>> sectionLines = data.WrappedData.Sections.Select(sl => sl.Points);
            return new MapMultiLineData(sectionLines, RingtoetsCommonDataResources.FailureMechanism_Sections_DisplayName);
        }

        private bool HasReferenceLinePoints()
        {
            return data.Parent.ReferenceLine != null && data.Parent.ReferenceLine.Points.Any();
        }

        private bool HasHydraulicBoundaryLocations()
        {
            return data.Parent.HydraulicBoundaryDatabase != null && data.Parent.HydraulicBoundaryDatabase.Locations.Any();
        }

        private bool HasSurfaceLines()
        {
            return data.WrappedData.SurfaceLines.Any();
        }

        private bool HasSections()
        {
            return data.WrappedData.Sections.Any();
        }
    }
}