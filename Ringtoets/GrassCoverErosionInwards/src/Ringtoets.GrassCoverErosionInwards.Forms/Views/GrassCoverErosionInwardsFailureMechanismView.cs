﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Factories;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion inwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionInwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData dikeProfilesMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;
        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private Observer dikeProfilesObserver;

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation> calculationObserver;
        private RecursiveObserver<DikeProfileCollection, DikeProfile> dikeProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismView(GrassCoverErosionInwardsFailureMechanism failureMechanism,
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

            CreateObservers();

            var mapDataCollection = new MapDataCollection(GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            dikeProfilesMapData = RingtoetsMapDataFactory.CreateDikeProfileMapData();
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
            mapDataCollection.Add(dikeProfilesMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(calculationsMapData);

            SetAllMapDataFeatures();

            ringtoetsMapControl.SetAllData(mapDataCollection, AssessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism { get; }

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
            waterLevelCalculationsForFactorizedSignalingNormObserver.Dispose();
            waterLevelCalculationsForSignalingNormObserver.Dispose();
            waterLevelCalculationsForLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedSignalingNormObserver.Dispose();
            waveHeightCalculationsForSignalingNormObserver.Dispose();
            waveHeightCalculationsForLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();

            dikeProfilesObserver.Dispose();
            dikeProfileObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsMapData)
            {
                Observable = FailureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            dikeProfilesObserver = new Observer(UpdateDikeProfilesMapData)
            {
                Observable = FailureMechanism.DikeProfiles
            };

            waterLevelCalculationsForFactorizedSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForFactorizedLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForFactorizedSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForFactorizedLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionInwardsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            dikeProfileObserver = new RecursiveObserver<DikeProfileCollection, DikeProfile>(UpdateDikeProfilesMapData, dpc => dpc)
            {
                Observable = FailureMechanism.DikeProfiles
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetCalculationsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetReferenceLineMapData();

            SetSectionsMapData();
            SetDikeProfilesMapData();
        }

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().Cast<GrassCoverErosionInwardsCalculation>();

            calculationsMapData.Features = GrassCoverErosionInwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
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
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(AssessmentSection);
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

        #region DikeProfiles MapData

        private void UpdateDikeProfilesMapData()
        {
            SetDikeProfilesMapData();
            dikeProfilesMapData.NotifyObservers();
            foreshoreProfilesMapData.NotifyObservers();
        }

        private void SetDikeProfilesMapData()
        {
            IEnumerable<DikeProfile> dikeProfiles = FailureMechanism.DikeProfiles;

            dikeProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(dikeProfiles.Select(dp => dp.ForeshoreProfile));
        }

        #endregion
    }
}