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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.Factories;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a dune erosion failure mechanism.
    /// </summary>
    public partial class DuneErosionFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer duneLocationsObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData duneLocationsMapData;

        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificFactorizedSignalingNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificSignalingNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificLowerLimitNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForLowerLimitNormObserver;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForFactorizedLowerLimitNormObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section in which the <paramref name="failureMechanism"/>
        /// belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneErosionFailureMechanismView(DuneErosionFailureMechanism failureMechanism,
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

            failureMechanismObserver = new Observer(UpdateMapData)
            {
                Observable = failureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateMapData)
            {
                Observable = assessmentSection
            };

            duneLocationsObserver = new Observer(UpdateMapData)
            {
                Observable = failureMechanism.DuneLocations
            };

            calculationsForMechanismSpecificFactorizedSignalingNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            calculationsForMechanismSpecificSignalingNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            calculationsForMechanismSpecificLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            calculationsForLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForLowerLimitNorm);
            calculationsForFactorizedLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForFactorizedLowerLimitNorm);

            var mapDataCollection = new MapDataCollection(DuneErosionDataResources.DuneErosionFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            duneLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(duneLocationsMapData);

            SetMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, AssessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; }

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
            duneLocationsObserver.Dispose();

            calculationsForMechanismSpecificFactorizedSignalingNormObserver?.Dispose();
            calculationsForMechanismSpecificSignalingNormObserver?.Dispose();
            calculationsForMechanismSpecificLowerLimitNormObserver?.Dispose();
            calculationsForLowerLimitNormObserver?.Dispose();
            calculationsForFactorizedLowerLimitNormObserver?.Dispose();

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
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
            duneLocationsMapData.NotifyObservers();
        }

        private void SetMapDataFeatures()
        {
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;

            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
            duneLocationsMapData.Features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(FailureMechanism);
        }

        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> CreateDuneLocationCalculationsObserver(
            IObservableEnumerable<DuneLocationCalculation> calculations)
        {
            return new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(UpdateMapData, calc => calc)
            {
                Observable = calculations
            };
        }
    }
}