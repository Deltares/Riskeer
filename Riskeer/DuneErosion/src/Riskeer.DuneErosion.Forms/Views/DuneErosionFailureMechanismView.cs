// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Factories;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Factories;
using DuneErosionDataResources = Riskeer.DuneErosion.Data.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a dune erosion failure mechanism.
    /// </summary>
    public partial class DuneErosionFailureMechanismView : UserControl, IMapView
    {
        private MapLineData referenceLineMapData;
        private MapPointData duneLocationsMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer duneLocationsObserver;

        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForMechanismSpecificLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsForFactorizedLowerLimitNormObserver;

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
                return riskeerMapControl.MapControl;
            }
        }

        /// <summary>
        /// Gets the <see cref="MapDataCollection"/>.
        /// </summary>
        protected MapDataCollection MapDataCollection { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            CreateObservers();

            CreateMapData();

            SetAllMapDataFeatures();

            riskeerMapControl.SetAllData(MapDataCollection, AssessmentSection.BackgroundData);

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            duneLocationsObserver.Dispose();

            calculationsForMechanismSpecificFactorizedSignalingNormObserver.Dispose();
            calculationsForMechanismSpecificSignalingNormObserver.Dispose();
            calculationsForMechanismSpecificLowerLimitNormObserver.Dispose();
            calculationsForLowerLimitNormObserver.Dispose();
            calculationsForFactorizedLowerLimitNormObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the observers.
        /// </summary>        
        protected virtual void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
            duneLocationsObserver = new Observer(UpdateDuneLocationMapData)
            {
                Observable = FailureMechanism.DuneLocations
            };

            calculationsForMechanismSpecificFactorizedSignalingNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            calculationsForMechanismSpecificSignalingNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            calculationsForMechanismSpecificLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            calculationsForLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForLowerLimitNorm);
            calculationsForFactorizedLowerLimitNormObserver = CreateDuneLocationCalculationsObserver(FailureMechanism.CalculationsForFactorizedLowerLimitNorm);
        }

        /// <summary>
        /// Creates the map data.
        /// </summary>
        protected virtual void CreateMapData()
        {
            MapDataCollection = new MapDataCollection(DuneErosionDataResources.DuneErosionFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            duneLocationsMapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            MapDataCollection.Add(referenceLineMapData);

            MapDataCollection.Add(duneLocationsMapData);
        }

        protected virtual void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetDuneLocationMapData();
        }

        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> CreateDuneLocationCalculationsObserver(
            IObservableEnumerable<DuneLocationCalculation> calculations)
        {
            return new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(UpdateDuneLocationMapData, calc => calc)
            {
                Observable = calculations
            };
        }

        #region AssessmentSection MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(AssessmentSection.ReferenceLine,
                                                                                                      AssessmentSection.Id,
                                                                                                      AssessmentSection.Name);
        }

        #endregion

        #region DuneLocation MapData

        private void UpdateDuneLocationMapData()
        {
            SetDuneLocationMapData();
            duneLocationsMapData.NotifyObservers();
        }

        private void SetDuneLocationMapData()
        {
            duneLocationsMapData.Features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(FailureMechanism);
        }

        #endregion
    }
}