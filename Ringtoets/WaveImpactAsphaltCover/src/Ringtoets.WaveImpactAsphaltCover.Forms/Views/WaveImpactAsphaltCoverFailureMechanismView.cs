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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HydraRing.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using WaveImpactAsphaltCoverDataResources = Ringtoets.WaveImpactAsphaltCover.Data.Properties.Resources;
using WaveImpactAsphaltCoverFormsResources = Ringtoets.WaveImpactAsphaltCover.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a wave impact asphalt cover failure mechanism.
    /// </summary>
    public partial class WaveImpactAsphaltCoverFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer foreshoreProfilesObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;
        private readonly MapLineData foreshoreProfilesMapData;

        private WaveImpactAsphaltCoverFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverFailureMechanismView"/>.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(UpdateMapData);
            foreshoreProfilesObserver = new Observer(UpdateMapData);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();

            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            mapControl.Data.Add(referenceLineMapData);
            mapControl.Data.Add(sectionsMapData);
            mapControl.Data.Add(sectionsStartPointMapData);
            mapControl.Data.Add(sectionsEndPointMapData);
            mapControl.Data.Add(hydraulicBoundaryDatabaseMapData);
            mapControl.Data.Add(foreshoreProfilesMapData);

            mapControl.Data.Name = WaveImpactAsphaltCoverDataResources.WaveImpactAsphaltCoverFailureMechanism_DisplayName;
            mapControl.Data.NotifyObservers();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as WaveImpactAsphaltCoverFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    foreshoreProfilesObserver.Observable = null;

                    Map.ResetMapData();
                    return;
                }

                failureMechanismObserver.Observable = data.WrappedData;
                assessmentSectionObserver.Observable = data.Parent;
                foreshoreProfilesObserver.Observable = data.WrappedData.ForeshoreProfiles;

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
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            foreshoreProfilesObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            ReferenceLine referenceLine = null;
            IEnumerable<FailureMechanismSection> failureMechanismSections = null;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = null;
            IEnumerable<ForeshoreProfile> foreshoreProfiles = null;

            if (data != null)
            {
                referenceLine = data.Parent.ReferenceLine;
                failureMechanismSections = data.WrappedData.Sections;
                hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
                foreshoreProfiles = data.WrappedData.ForeshoreProfiles;
            }

            UpdateFeatureBasedMapData(referenceLineMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine));
            UpdateFeatureBasedMapData(sectionsMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections));
            UpdateFeatureBasedMapData(sectionsStartPointMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections));
            UpdateFeatureBasedMapData(sectionsEndPointMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections));
            UpdateFeatureBasedMapData(hydraulicBoundaryDatabaseMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(hydraulicBoundaryDatabase));
            UpdateFeatureBasedMapData(foreshoreProfilesMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles));
        }

        private static void UpdateFeatureBasedMapData(FeatureBasedMapData mapData, MapFeature[] features)
        {
            mapData.Features = features;
            mapData.NotifyObservers();
        }
    }
}