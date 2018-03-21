// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for an assessment section.
    /// </summary>
    public partial class AssessmentSectionView : UserControl, IMapView
    {
        private readonly IAssessmentSection assessmentSection;

        private readonly MapLineData referenceLineMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;

        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNormObserver;
        private readonly RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionView"/>.
        /// </summary>
        public AssessmentSectionView(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            assessmentSectionObserver = new Observer(UpdateMapData)
            {
                Observable = assessmentSection
            };

            waterLevelCalculationsForFactorizedSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            waterLevelCalculationsForSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            waterLevelCalculationsForLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            waterLevelCalculationsForFactorizedLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            waveHeightCalculationsForFactorizedSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);

            hydraulicBoundaryLocationsObserver = new Observer(UpdateMapData)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(
                UpdateMapData, hbl => hbl)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };

            var mapDataCollection = new MapDataCollection(Resources.AssessmentSectionMap_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);

            this.assessmentSection = assessmentSection;

            SetMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
        }

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> CreateHydraulicBoundaryLocationCalculationsObserver(
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            return new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(
                UpdateMapData, calc => calc)
            {
                Observable = calculations
            };
        }

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
            assessmentSectionObserver.Dispose();
            waterLevelCalculationsForFactorizedSignalingNormObserver.Dispose();
            waterLevelCalculationsForSignalingNormObserver.Dispose();
            waterLevelCalculationsForLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedSignalingNormObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            hydraulicBoundaryLocationObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            SetMapDataFeatures();

            referenceLineMapData.NotifyObservers();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }

        private void SetMapDataFeatures()
        {
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(assessmentSection.ReferenceLine, assessmentSection.Id, assessmentSection.Name);
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(assessmentSection);
        }
    }
}