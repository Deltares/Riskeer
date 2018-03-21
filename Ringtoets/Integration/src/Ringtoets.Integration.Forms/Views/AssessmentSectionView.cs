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

        private Observer assessmentSectionObserver;
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
        /// Creates a new instance of <see cref="AssessmentSectionView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionView(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            this.assessmentSection = assessmentSection;

            CreateObservers();

            var mapDataCollection = new MapDataCollection(Resources.AssessmentSectionMap_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);

            SetAllMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
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
            waveHeightCalculationsForSignalingNormObserver.Dispose();
            waveHeightCalculationsForLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = assessmentSection
            };

            waterLevelCalculationsForFactorizedSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            waterLevelCalculationsForSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            waterLevelCalculationsForLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            waterLevelCalculationsForFactorizedLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            waveHeightCalculationsForFactorizedSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
            waveHeightCalculationsForSignalingNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaveHeightCalculationsForSignalingNorm);
            waveHeightCalculationsForLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            waveHeightCalculationsForFactorizedLowerLimitNormObserver = CreateHydraulicBoundaryLocationCalculationsObserver(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);

            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
        }

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> CreateHydraulicBoundaryLocationCalculationsObserver(
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            return new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(
                UpdateHydraulicBoundaryLocationsMapData, calc => calc)
            {
                Observable = calculations
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetHydraulicBoundaryLocationsMapData();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(assessmentSection.ReferenceLine, assessmentSection.Id, assessmentSection.Name);
        }

        private void SetHydraulicBoundaryLocationsMapData()
        {
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(assessmentSection);
        }

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void UpdateHydraulicBoundaryLocationsMapData()
        {
            SetHydraulicBoundaryLocationsMapData();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }
    }
}