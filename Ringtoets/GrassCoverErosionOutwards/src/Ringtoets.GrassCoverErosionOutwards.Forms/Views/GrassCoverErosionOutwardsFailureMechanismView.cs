﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Factories;
using Ringtoets.Revetment.Data;
using GrassCoverErosionOutwardsDataResources = Ringtoets.GrassCoverErosionOutwards.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion outwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionOutwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;
        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private Observer foreshoreProfilesObserver;

        private RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;
        private RecursiveObserver<CalculationGroup, WaveConditionsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation> calculationObserver;
        private RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile> foreshoreProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismView"/>.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanismView(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                             IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;

            CreateObservers(failureMechanism);

            var mapDataCollection = new MapDataCollection(GrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(calculationsMapData);

            SetMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, AssessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return ringtoetsMapControl.MapControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            hydraulicBoundaryLocationObserver.Dispose();
            foreshoreProfilesObserver.Dispose();
            foreshoreProfileObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateObservers(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            failureMechanismObserver = new Observer(UpdateSectionsMapData)
            {
                Observable = failureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            foreshoreProfilesObserver = new Observer(UpdateForeshoreProfilesMapData)
            {
                Observable = failureMechanism.ForeshoreProfiles
            };

            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(
                UpdateHydraulicBoundaryLocationsMapData, hbl => hbl)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            calculationInputObserver = new RecursiveObserver<CalculationGroup, WaveConditionsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)))
            {
                Observable = failureMechanism.WaveConditionsCalculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = failureMechanism.WaveConditionsCalculationGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = failureMechanism.WaveConditionsCalculationGroup
            };
            foreshoreProfileObserver = new RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile>(UpdateForeshoreProfilesMapData, coll => coll)
            {
                Observable = failureMechanism.ForeshoreProfiles
            };
        }

        private void SetMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetForeshoreProfilesMapData();
            SetCalculationsMapData();
        }

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations =
                FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
            calculationsMapData.Features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
        }

        #endregion

        #region HydraulicBoundaryLocations MapData

        private void UpdateHydraulicBoundaryLocationsMapData()
        {
            SetHydraulicBoundaryLocationsMapData();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }

        private void SetHydraulicBoundaryLocationsMapData()
        {
            hydraulicBoundaryLocationsMapData.Features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationsFeatures(AssessmentSection, FailureMechanism);
        }

        #endregion

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion

        #region Sections MapData

        private void UpdateSectionsMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;

            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
        }

        #endregion

        #region Foreshore Profiles MapData

        private void UpdateForeshoreProfilesMapData()
        {
            SetForeshoreProfilesMapData();
            foreshoreProfilesMapData.NotifyObservers();
        }

        private void SetForeshoreProfilesMapData()
        {
            IEnumerable<ForeshoreProfile> foreshoreProfiles = FailureMechanism.ForeshoreProfiles;
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
        }

        #endregion
    }
}