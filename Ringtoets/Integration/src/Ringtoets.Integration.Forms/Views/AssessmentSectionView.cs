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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
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

        private MapData hydraulicBoundaryDatabaseLocationsData;
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
                hydraulicBoundaryDatabaseLocationsData = AddOrUpdateMapData(hydraulicBoundaryDatabaseLocationsData, GetHydraulicBoundaryLocations());
                referenceLineData = AddOrUpdateMapData(referenceLineData, GetReferenceLineData());
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

        private MapData GetReferenceLineData()
        {
            if (data == null || data.ReferenceLine == null)
            {
                return MapDataFactory.CreateEmptyLineData(RingtoetsCommonDataResources.ReferenceLine_DisplayName);
            }
            return MapDataFactory.Create(data.ReferenceLine);
        }

        private MapData GetHydraulicBoundaryLocations()
        {
            if (data == null || data.HydraulicBoundaryDatabase == null)
            {
                return MapDataFactory.CreateEmptyPointData(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName);
            }
            return MapDataFactory.Create(data.HydraulicBoundaryDatabase);
        }
    }
}