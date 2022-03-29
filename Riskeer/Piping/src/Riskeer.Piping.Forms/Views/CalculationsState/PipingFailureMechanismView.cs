﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.Factories;
using Riskeer.Piping.Primitives;
using PipingDataResources = Riskeer.Piping.Data.Properties.Resources;

namespace Riskeer.Piping.Forms.Views.CalculationsState
{
    /// <summary>
    /// Calculations state view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView
    {
        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;

        private MapLineData referenceLineMapData;
        private MapLineData stochasticSoilModelsMapData;
        private MapLineData surfaceLinesMapData;
        private MapLineData semiProbabilisticCalculationsMapData;
        private MapLineData probabilisticCalculationsMapData;

        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer surfaceLinesObserver;
        private Observer stochasticSoilModelsObserver;

        private RecursiveObserver<CalculationGroup, SemiProbabilisticPipingInput> semiProbabilisticCalculationInputObserver;
        private RecursiveObserver<CalculationGroup, ProbabilisticPipingInput> probabilisticCalculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, SemiProbabilisticPipingCalculationScenario> semiProbabilisticCalculationObserver;
        private RecursiveObserver<CalculationGroup, ProbabilisticPipingCalculationScenario> probabilisticCalculationObserver;
        private RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine> surfaceLineObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismView(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
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
        public PipingFailureMechanism FailureMechanism { get; }

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
            hydraulicBoundaryLocationsMapLayer.Dispose();
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            stochasticSoilModelsObserver.Dispose();
            semiProbabilisticCalculationInputObserver.Dispose();
            probabilisticCalculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            semiProbabilisticCalculationObserver.Dispose();
            probabilisticCalculationObserver.Dispose();
            surfaceLinesObserver.Dispose();
            surfaceLineObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the map data.
        /// </summary>
        protected virtual void CreateMapData()
        {
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(AssessmentSection);

            MapDataCollection = new MapDataCollection(PipingDataResources.PipingFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            stochasticSoilModelsMapData = RiskeerMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = RiskeerMapDataFactory.CreateSurfaceLinesMapData();
            semiProbabilisticCalculationsMapData = PipingMapDataFactory.CreateSemiProbabilisticCalculationsMapData();
            probabilisticCalculationsMapData = PipingMapDataFactory.CreateProbabilisticCalculationsMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            MapDataCollection.Add(referenceLineMapData);
            MapDataCollection.Add(stochasticSoilModelsMapData);
            MapDataCollection.Add(surfaceLinesMapData);

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            MapDataCollection.Add(sectionsMapDataCollection);
            MapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
            MapDataCollection.Add(probabilisticCalculationsMapData);
            MapDataCollection.Add(semiProbabilisticCalculationsMapData);
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
            surfaceLinesObserver = new Observer(UpdateSurfaceLinesMapData)
            {
                Observable = FailureMechanism.SurfaceLines
            };
            stochasticSoilModelsObserver = new Observer(UpdateStochasticSoilModelsMapData)
            {
                Observable = FailureMechanism.StochasticSoilModels
            };

            semiProbabilisticCalculationInputObserver = new RecursiveObserver<CalculationGroup, SemiProbabilisticPipingInput>(
                UpdateSemiProbabilisticCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<SemiProbabilisticPipingCalculationScenario>().Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            probabilisticCalculationInputObserver = new RecursiveObserver<CalculationGroup, ProbabilisticPipingInput>(
                UpdateProbabilisticCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<ProbabilisticPipingCalculationScenario>().Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(() =>
            {
                UpdateSemiProbabilisticCalculationsMapData();
                UpdateProbabilisticCalculationsMapData();
            }, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            semiProbabilisticCalculationObserver = new RecursiveObserver<CalculationGroup, SemiProbabilisticPipingCalculationScenario>(UpdateSemiProbabilisticCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            probabilisticCalculationObserver = new RecursiveObserver<CalculationGroup, ProbabilisticPipingCalculationScenario>(UpdateProbabilisticCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            surfaceLineObserver = new RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine>(UpdateSurfaceLinesMapData, rpslc => rpslc)
            {
                Observable = FailureMechanism.SurfaceLines
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetCalculationsMapData<SemiProbabilisticPipingCalculationScenario>(semiProbabilisticCalculationsMapData);
            SetCalculationsMapData<ProbabilisticPipingCalculationScenario>(probabilisticCalculationsMapData);
            SetReferenceLineMapData();

            SetSectionsMapData();
            SetSurfaceLinesMapData();
            SetStochasticSoilModelsMapData();
        }

        #region Calculations MapData

        private void UpdateSemiProbabilisticCalculationsMapData()
        {
            SetCalculationsMapData<SemiProbabilisticPipingCalculationScenario>(semiProbabilisticCalculationsMapData);
            semiProbabilisticCalculationsMapData.NotifyObservers();
        }

        private void UpdateProbabilisticCalculationsMapData()
        {
            SetCalculationsMapData<ProbabilisticPipingCalculationScenario>(probabilisticCalculationsMapData);
            probabilisticCalculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData<TCalculationScenario>(FeatureBasedMapData calculationsMapData)
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>
        {
            IEnumerable<TCalculationScenario> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().OfType<TCalculationScenario>();
            calculationsMapData.Features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
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

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;

            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
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
            PipingSurfaceLineCollection pipingSurfaceLines = FailureMechanism.SurfaceLines;
            surfaceLinesMapData.Features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(pipingSurfaceLines.ToArray());
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
            PipingStochasticSoilModelCollection stochasticSoilModels = FailureMechanism.StochasticSoilModels;
            stochasticSoilModelsMapData.Features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels.ToArray());
        }

        #endregion
    }
}