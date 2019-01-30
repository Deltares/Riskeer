// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a failure mechanism with a detailed assessment.
    /// </summary>
    public partial class FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult> : UserControl, IMapView
        where TFailureMechanism : IHasSectionResults<TSectionResult>
        where TSectionResult : FailureMechanismSectionResult
    {
        private readonly Func<IEnumerable<MapFeature>> getSimpleAssemblyFeaturesFunc;
        private readonly Func<IEnumerable<MapFeature>> getDetailedAssemblyFeaturesFunc;
        private readonly Func<IEnumerable<MapFeature>> getTailorMadeAssemblyFeaturesFunc;
        private readonly Func<IEnumerable<MapFeature>> getCombinedAssemblyFeaturesFunc;

        private MapDataCollection mapDataCollection;
        private MapLineData referenceLineMapData;
        private MapPointData hydraulicBoundaryLocationsMapData;

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

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismWithDetailedAssessmentView{TFailureMechanism, TSectionResult}"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <param name="getSimpleAssemblyFeaturesFunc">The <see cref="Func{T}"/> that returns a collection of <see cref="MapFeature"/> representing
        /// the simple assembly results for this failure mechanism.</param>
        /// <param name="getDetailedAssemblyFeaturesFunc">The <see cref="Func{T}"/> that returns a collection of <see cref="MapFeature"/> representing
        /// the detailed assembly results for this failure mechanism.</param>
        /// <param name="getTailorMadeAssemblyFeaturesFunc">The <see cref="Func{T}"/> that returns a collection of <see cref="MapFeature"/> representing
        /// the tailor made assembly results for this failure mechanism.</param>
        /// <param name="getCombinedAssemblyFeaturesFunc">The <see cref="Func{T}"/> that returns a collection of <see cref="MapFeature"/> representing
        /// the combined assembly results for this failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismWithDetailedAssessmentView(TFailureMechanism failureMechanism,
                                                          IAssessmentSection assessmentSection,
                                                          Func<IEnumerable<MapFeature>> getSimpleAssemblyFeaturesFunc,
                                                          Func<IEnumerable<MapFeature>> getDetailedAssemblyFeaturesFunc,
                                                          Func<IEnumerable<MapFeature>> getTailorMadeAssemblyFeaturesFunc,
                                                          Func<IEnumerable<MapFeature>> getCombinedAssemblyFeaturesFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getSimpleAssemblyFeaturesFunc == null)
            {
                throw new ArgumentNullException(nameof(getSimpleAssemblyFeaturesFunc));
            }

            if (getDetailedAssemblyFeaturesFunc == null)
            {
                throw new ArgumentNullException(nameof(getDetailedAssemblyFeaturesFunc));
            }

            if (getTailorMadeAssemblyFeaturesFunc == null)
            {
                throw new ArgumentNullException(nameof(getTailorMadeAssemblyFeaturesFunc));
            }

            if (getCombinedAssemblyFeaturesFunc == null)
            {
                throw new ArgumentNullException(nameof(getCombinedAssemblyFeaturesFunc));
            }

            this.getSimpleAssemblyFeaturesFunc = getSimpleAssemblyFeaturesFunc;
            this.getDetailedAssemblyFeaturesFunc = getDetailedAssemblyFeaturesFunc;
            this.getTailorMadeAssemblyFeaturesFunc = getTailorMadeAssemblyFeaturesFunc;
            this.getCombinedAssemblyFeaturesFunc = getCombinedAssemblyFeaturesFunc;

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;

            CreateObservers();

            CreateMapData();
            SetAllMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public TFailureMechanism FailureMechanism { get; }

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
            hydraulicBoundaryLocationsObserver.Dispose();
            waterLevelCalculationsForFactorizedSignalingNormObserver.Dispose();
            waterLevelCalculationsForSignalingNormObserver.Dispose();
            waterLevelCalculationsForLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedSignalingNormObserver.Dispose();
            waveHeightCalculationsForSignalingNormObserver.Dispose();
            waveHeightCalculationsForLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            sectionResultObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateMapData()
        {
            mapDataCollection = new MapDataCollection(FailureMechanism.Name);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

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
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = FailureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
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

            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult>(UpdateAssemblyMapData, sr => sr)
            {
                Observable = FailureMechanism.SectionResults
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
            SetHydraulicBoundaryLocationsMapData();
            UpdateAssemblyMapData();
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
            simpleAssemblyMapData.Features = getSimpleAssemblyFeaturesFunc();
            detailedAssemblyMapData.Features = getDetailedAssemblyFeaturesFunc();
            tailorMadeAssemblyMapData.Features = getTailorMadeAssemblyFeaturesFunc();
            combinedAssemblyMapData.Features = getCombinedAssemblyFeaturesFunc();
        }

        #endregion

        #region AssessmentSection MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(
                AssessmentSection.ReferenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
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

            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
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
            hydraulicBoundaryLocationsMapData.Features = RiskeerMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(AssessmentSection);
        }

        #endregion
    }
}