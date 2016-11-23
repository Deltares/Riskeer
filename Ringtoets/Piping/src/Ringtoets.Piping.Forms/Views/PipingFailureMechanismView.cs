// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer surfaceLinesObserver;
        private readonly Observer stochasticSoilModelsObserver;

        private readonly RecursiveObserver<CalculationGroup, PipingInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingCalculationScenario> calculationObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapLineData stochasticSoilModelsMapData;
        private readonly MapLineData surfaceLinesMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;
        private readonly MapLineData calculationsMapData;

        private PipingFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        public PipingFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(UpdateMapData);
            surfaceLinesObserver = new Observer(UpdateMapData);
            stochasticSoilModelsObserver = new Observer(UpdateMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(
                UpdateMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculationScenario>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, PipingCalculationScenario>(UpdateMapData, pcg => pcg.Children);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();

            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            stochasticSoilModelsMapData = PipingMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = PipingMapDataFactory.CreateSurfaceLinesMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapControl.Data.Add(referenceLineMapData);
            mapControl.Data.Add(sectionsMapData);
            mapControl.Data.Add(stochasticSoilModelsMapData);
            mapControl.Data.Add(surfaceLinesMapData);
            mapControl.Data.Add(sectionsStartPointMapData);
            mapControl.Data.Add(sectionsEndPointMapData);
            mapControl.Data.Add(hydraulicBoundaryDatabaseMapData);
            mapControl.Data.Add(calculationsMapData);
        
            mapControl.Data.Name = PipingDataResources.PipingFailureMechanism_DisplayName;
            mapControl.Data.NotifyObservers();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as PipingFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    surfaceLinesObserver.Observable = null;
                    stochasticSoilModelsObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    surfaceLinesObserver.Observable = data.WrappedData.SurfaceLines;
                    stochasticSoilModelsObserver.Observable = data.WrappedData.StochasticSoilModels;
                    calculationInputObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationGroupObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationObserver.Observable = data.WrappedData.CalculationsGroup;
                }

                UpdateMapData();
            }
        }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            stochasticSoilModelsObserver.Dispose();
            surfaceLinesObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            if (data == null)
            {
                Map.ResetMapData();
            }
            else
            {
                ReferenceLine referenceLine = data.Parent.ReferenceLine;
                IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;
                ObservableList<StochasticSoilModel> stochasticSoilModels = data.WrappedData.StochasticSoilModels;
                ObservableList<RingtoetsPipingSurfaceLine> ringtoetsPipingSurfaceLines = data.WrappedData.SurfaceLines;
                HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
                IEnumerable<PipingCalculationScenario> calculations = 
                    data.WrappedData.CalculationsGroup.GetCalculations().Cast<PipingCalculationScenario>();

                referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
                sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
                stochasticSoilModelsMapData.Features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels);
                surfaceLinesMapData.Features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(ringtoetsPipingSurfaceLines);
                sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
                sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
                hydraulicBoundaryDatabaseMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels(hydraulicBoundaryDatabase);
                calculationsMapData.Features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(calculations);

                mapControl.Data.NotifyObservers();
            }
        }
    }
}