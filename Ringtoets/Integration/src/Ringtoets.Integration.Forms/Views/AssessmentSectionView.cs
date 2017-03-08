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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for an assessment section.
    /// </summary>
    public partial class AssessmentSectionView : UserControl, IMapView
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private readonly Observer backgroundMapDataObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;

        private WmtsMapData backgroundMapData;

        private IAssessmentSection data;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionView"/>.
        /// </summary>
        public AssessmentSectionView()
        {
            InitializeComponent();

            assessmentSectionObserver = new Observer(() =>
            {
                if (!ReferenceEquals(hydraulicBoundaryDatabaseObserver.Observable, data.HydraulicBoundaryDatabase))
                {
                    hydraulicBoundaryDatabaseObserver.Observable = data.HydraulicBoundaryDatabase;
                }

                UpdateMapData();
            });
            hydraulicBoundaryDatabaseObserver = new Observer(UpdateMapData);
            backgroundMapDataObserver = new Observer(UpdateBackgroundMapData);

            mapDataCollection = new MapDataCollection(Resources.AssessmentSectionMap_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
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

                assessmentSectionObserver.Observable = data;
                hydraulicBoundaryDatabaseObserver.Observable = data?.HydraulicBoundaryDatabase;
                backgroundMapDataObserver.Observable = data?.BackgroundMapData2;

                if (data == null)
                {
                    Map.Data = null;
                    Map.BackgroundMapData = null;
                }
                else
                {
                    SetMapDataFeatures();

                    backgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(data.BackgroundMapData2);

                    Map.Data = mapDataCollection;
                    Map.BackgroundMapData = backgroundMapData;
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

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateBackgroundMapData()
        {
            RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(backgroundMapData, data.BackgroundMapData2);
            backgroundMapData.NotifyObservers();
        }

        private void UpdateMapData()
        {
            SetMapDataFeatures();

            referenceLineMapData.NotifyObservers();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }

        private void SetMapDataFeatures()
        {
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(data.ReferenceLine, data.Id, data.Name);
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(data.HydraulicBoundaryDatabase);
        }
    }
}