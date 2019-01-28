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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.Helpers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Forms.Factories;
using Riskeer.Integration.Forms.Observers;
using Riskeer.Integration.Forms.Properties;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// The map view for the assembly result per section for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>. 
    /// </summary>
    public partial class AssemblyResultPerSectionMapView : UserControl, IMapView
    {
        private readonly MapLineData assemblyResultsMapData;
        private readonly MapLineData referenceLineMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;

        private Observer assessmentSectionResultObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNormObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultPerSectionMapView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssemblyResultPerSectionMapView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            AssessmentSection = assessmentSection;

            CreateObservers();

            var mapDataCollection = new MapDataCollection(Resources.AssemblyResultPerSectionMapView_DisplayName);
            assemblyResultsMapData = CombinedSectionAssemblyMapDataFactory.CreateCombinedSectionAssemblyResultMapData();
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(assemblyResultsMapData);

            SetAllMapDataFeatures();
            SetWarningPanel();

            ringtoetsMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the <see cref="Riskeer.Integration.Data.AssessmentSection"/> the view belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

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
            if (disposing)
            {
                assessmentSectionResultObserver.Dispose();
                waterLevelCalculationsForFactorizedSignalingNormObserver.Dispose();
                waterLevelCalculationsForSignalingNormObserver.Dispose();
                waterLevelCalculationsForLowerLimitNormObserver.Dispose();
                waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
                waveHeightCalculationsForFactorizedSignalingNormObserver.Dispose();
                waveHeightCalculationsForSignalingNormObserver.Dispose();
                waveHeightCalculationsForLowerLimitNormObserver.Dispose();
                waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
                hydraulicBoundaryLocationsObserver.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateObservers()
        {
            assessmentSectionResultObserver = new Observer(() =>
            {
                UpdateAssessmentSectionData();
                SetWarningPanel();
            })
            {
                Observable = new AssessmentSectionResultObserver(AssessmentSection)
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

            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetAssemblyResultsMapData();
        }

        private void SetWarningPanel()
        {
            warningPanel.Visible = AssessmentSectionHelper.HasManualAssemblyResults(AssessmentSection);
        }

        private void UpdateAssessmentSectionData()
        {
            UpdateAssemblyResultsMapData();
            UpdateReferenceLineMapData();
        }

        #region AssemblyResults MapData

        private void UpdateAssemblyResultsMapData()
        {
            SetAssemblyResultsMapData();
            assemblyResultsMapData.NotifyObservers();
        }

        private void SetAssemblyResultsMapData()
        {
            assemblyResultsMapData.Features = AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(AssessmentSection);
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
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(AssessmentSection.ReferenceLine,
                                                                                                        AssessmentSection.Id,
                                                                                                        AssessmentSection.Name);
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
    }
}