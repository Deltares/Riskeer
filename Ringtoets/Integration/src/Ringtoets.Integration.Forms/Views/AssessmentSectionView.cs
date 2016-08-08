﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for an assessment section.
    /// </summary>
    public partial class AssessmentSectionView : UserControl, IMapView
    {
        private readonly Observer assessmentSectionObserver;

        private readonly MapControl mapControl;

        private readonly MapLineData referenceLineMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;

        private IAssessmentSection data;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionView"/>.
        /// </summary>
        public AssessmentSectionView()
        {
            InitializeComponent();

            assessmentSectionObserver = new Observer(UpdateMapData);

            mapControl = new MapControl
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(mapControl);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();

            mapControl.Data.Add(referenceLineMapData);
            mapControl.Data.Add(hydraulicBoundaryDatabaseMapData);

            mapControl.Data.Name = Resources.AssessmentSectionMap_DisplayName;
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

                if (data == null)
                {
                    Map.ResetMapData();
                    return;
                }

                UpdateMapData();
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

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            UpdateFeatureBasedMapData(referenceLineMapData, RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(data != null ? data.ReferenceLine : null));
            UpdateFeatureBasedMapData(hydraulicBoundaryDatabaseMapData, RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(data != null ? data.HydraulicBoundaryDatabase : null));

            mapControl.Data.NotifyObservers();
        }

        private static void UpdateFeatureBasedMapData(FeatureBasedMapData mapData, MapFeature[] features)
        {
            mapData.Features = features;
            mapData.NotifyObservers();
        }
    }
}