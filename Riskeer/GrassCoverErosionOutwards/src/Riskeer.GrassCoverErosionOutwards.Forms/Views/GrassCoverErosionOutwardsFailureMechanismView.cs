// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
        private MapDataCollection mapDataCollection;
        private MapLineData referenceLineMapData;
        private MapPointData hydraulicBoundaryLocationsMapData;
        private MapLineData foreshoreProfilesMapData;
        private MapLineData calculationsMapData;

        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private MapLineData simpleAssemblyMapData;
        private MapLineData detailedAssemblyMapData;
        private MapLineData tailorMadeAssemblyMapData;
        private MapLineData combinedAssemblyMapData;

        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private Observer foreshoreProfilesObserver;

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<CalculationGroup, WaveConditionsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation> calculationObserver;
        private RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile> foreshoreProfileObserver;
        private RecursiveObserver<IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>, GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
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

            CreateObservers();

            CreateMapData();
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
            referenceLineObserver.Dispose();
            waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver.Dispose();
            waterLevelCalculationsForMechanismSpecificSignalingNormObserver.Dispose();
            waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver.Dispose();
            waveHeightCalculationsForMechanismSpecificSignalingNormObserver.Dispose();
            waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            foreshoreProfilesObserver.Dispose();
            foreshoreProfileObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            sectionResultObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateMapData()
        {
            mapDataCollection = new MapDataCollection(GrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            MapDataCollection sectionsMapDataCollection = RingtoetsMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            MapDataCollection assemblyMapDataCollection = AssemblyMapDataFactory.CreateAssemblyMapDataCollection();
            tailorMadeAssemblyMapData = AssemblyMapDataFactory.CreateTailorMadeAssemblyMapData();
            detailedAssemblyMapData = AssemblyMapDataFactory.CreateDetailedAssemblyMapData();
            simpleAssemblyMapData = AssemblyMapDataFactory.CreateSimpleAssemblyMapData();
            combinedAssemblyMapData = AssemblyMapDataFactory.CreateCombinedAssemblyMapData();

            mapDataCollection.Add(referenceLineMapData);

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(sectionsMapDataCollection);

            assemblyMapDataCollection.Add(tailorMadeAssemblyMapData);
            assemblyMapDataCollection.Add(detailedAssemblyMapData);
            assemblyMapDataCollection.Add(simpleAssemblyMapData);
            assemblyMapDataCollection.Add(combinedAssemblyMapData);
            mapDataCollection.Add(assemblyMapDataCollection);

            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(calculationsMapData);
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateFailureMechanismData)
            {
                Observable = FailureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateReferenceLineData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            foreshoreProfilesObserver = new Observer(UpdateForeshoreProfilesMapData)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };

            waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForMechanismSpecificSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waterLevelCalculationsForFactorizedLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForMechanismSpecificSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                FailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);
            waveHeightCalculationsForFactorizedLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, UpdateHydraulicBoundaryLocationsMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, WaveConditionsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            foreshoreProfileObserver = new RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile>(UpdateForeshoreProfilesMapData, coll => coll)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };

            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                GrassCoverErosionOutwardsFailureMechanismSectionResult>(UpdateAssemblyMapData, sr => sr)
            {
                Observable = FailureMechanism.SectionResults
            };
        }

        private void SetMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetForeshoreProfilesMapData();
            SetCalculationsMapData();
            SetAssemblyMapData();
        }

        #region Assembly MapData

        private void UpdateAssemblyMapData()
        {
            SetAssemblyMapData();
            simpleAssemblyMapData.NotifyObservers();
            detailedAssemblyMapData.NotifyObservers();
            tailorMadeAssemblyMapData.NotifyObservers();
            combinedAssemblyMapData.NotifyObservers();
        }

        private void SetAssemblyMapData()
        {
            simpleAssemblyMapData.Features = GrassCoverErosionOutwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(FailureMechanism);
            detailedAssemblyMapData.Features = GrassCoverErosionOutwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(FailureMechanism);
            tailorMadeAssemblyMapData.Features = GrassCoverErosionOutwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(FailureMechanism);
            combinedAssemblyMapData.Features = GrassCoverErosionOutwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(FailureMechanism);
        }

        #endregion

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();

            UpdateAssemblyMapData();
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

        #region AssessmentSection MapData

        private void UpdateReferenceLineData()
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

        #region FailureMechanism MapData

        private void UpdateFailureMechanismData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();

            UpdateAssemblyMapData();
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