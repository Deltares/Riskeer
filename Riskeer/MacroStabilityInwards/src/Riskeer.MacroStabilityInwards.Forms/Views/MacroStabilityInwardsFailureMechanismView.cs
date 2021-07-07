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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Helpers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Factories;
using Riskeer.MacroStabilityInwards.Primitives;
using MacroStabilityInwardsDataResources = Riskeer.MacroStabilityInwards.Data.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a macro stability inwards failure mechanism.
    /// </summary>
    public partial class MacroStabilityInwardsFailureMechanismView : UserControl, IMapView
    {
        private MapLineData referenceLineMapData;
        private MapLineData stochasticSoilModelsMapData;
        private MapLineData surfaceLinesMapData;
        private MapPointData hydraulicBoundaryLocationsMapData;
        private MapLineData calculationsMapData;

        private Observer hydraulicBoundaryLocationsObserver;
        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer surfaceLinesObserver;
        private Observer stochasticSoilModelsObserver;

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNormObserver;
        private RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario> calculationObserver;
        private RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine> surfaceLineObserver;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismView(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
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
        public MacroStabilityInwardsFailureMechanism FailureMechanism { get; }

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
            waterLevelCalculationsForFactorizedSignalingNormObserver.Dispose();
            waterLevelCalculationsForSignalingNormObserver.Dispose();
            waterLevelCalculationsForLowerLimitNormObserver.Dispose();
            waterLevelCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedSignalingNormObserver.Dispose();
            waveHeightCalculationsForSignalingNormObserver.Dispose();
            waveHeightCalculationsForLowerLimitNormObserver.Dispose();
            waveHeightCalculationsForFactorizedLowerLimitNormObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            stochasticSoilModelsObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            surfaceLinesObserver.Dispose();
            surfaceLineObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void CreateMapData()
        {
            MapDataCollection = new MapDataCollection(MacroStabilityInwardsDataResources.MacroStabilityInwardsFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            stochasticSoilModelsMapData = RiskeerMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = RiskeerMapDataFactory.CreateSurfaceLinesMapData();
            calculationsMapData = RiskeerMapDataFactory.CreateCalculationsMapData();

            MapDataCollection.Add(referenceLineMapData);
            MapDataCollection.Add(stochasticSoilModelsMapData);
            MapDataCollection.Add(surfaceLinesMapData);

            MapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            MapDataCollection.Add(calculationsMapData);
        }

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
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            surfaceLinesObserver = new Observer(UpdateSurfaceLinesMapData)
            {
                Observable = FailureMechanism.SurfaceLines
            };
            stochasticSoilModelsObserver = new Observer(UpdateStochasticSoilModelsMapData)
            {
                Observable = FailureMechanism.StochasticSoilModels
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

            calculationInputObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<MacroStabilityInwardsCalculationScenario>().Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };

            surfaceLineObserver = new RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine>(UpdateSurfaceLinesMapData, rpslc => rpslc)
            {
                Observable = FailureMechanism.SurfaceLines
            };
        }

        protected virtual void SetAllMapDataFeatures()
        {
            SetCalculationsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetReferenceLineMapData();

            SetSurfaceLinesMapData();
            SetStochasticSoilModelsMapData();
        }

        #region Calculations MapData

        protected virtual void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().Cast<MacroStabilityInwardsCalculationScenario>();
            calculationsMapData.Features = MacroStabilityInwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
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

        #region AssessmentSection MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion

        #region SurfaceLines MapData

        private void UpdateSurfaceLinesMapData()
        {
            SetSurfaceLinesMapData();
            surfaceLinesMapData.NotifyObservers();
        }

        private void SetSurfaceLinesMapData()
        {
            MacroStabilityInwardsSurfaceLineCollection macroStabilityInwardsSurfaceLines = FailureMechanism.SurfaceLines;
            surfaceLinesMapData.Features = MacroStabilityInwardsMapDataFeaturesFactory.CreateSurfaceLineFeatures(macroStabilityInwardsSurfaceLines.ToArray());
        }

        #endregion

        #region StochasticSoilModels MapData

        private void UpdateStochasticSoilModelsMapData()
        {
            SetStochasticSoilModelsMapData();
            stochasticSoilModelsMapData.NotifyObservers();
        }

        private void SetStochasticSoilModelsMapData()
        {
            MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModels = FailureMechanism.StochasticSoilModels;
            stochasticSoilModelsMapData.Features = MacroStabilityInwardsMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels.ToArray());
        }

        #endregion
    }
}