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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Primitives;
using MacroStabilityInwardsDataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a macrostability inwards failure mechanism.
    /// </summary>
    public partial class MacroStabilityInwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer surfaceLinesObserver;
        private readonly Observer stochasticSoilModelsObserver;

        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario> calculationObserver;
        private readonly RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine> surfaceLineObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapLineData stochasticSoilModelsMapData;
        private readonly MapLineData surfaceLinesMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData calculationsMapData;

        private MacroStabilityInwardsFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismView"/>.
        /// </summary>
        public MacroStabilityInwardsFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(() =>
            {
                if (!ReferenceEquals(hydraulicBoundaryDatabaseObserver.Observable, data.Parent.HydraulicBoundaryDatabase))
                {
                    hydraulicBoundaryDatabaseObserver.Observable = data.Parent.HydraulicBoundaryDatabase;
                }

                UpdateMapData();
            });
            hydraulicBoundaryDatabaseObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData);
            surfaceLinesObserver = new Observer(UpdateSurfaceLinesMapData);
            stochasticSoilModelsObserver = new Observer(UpdateStochasticSoilModelsMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<MacroStabilityInwardsCalculationScenario>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario>(UpdateCalculationsMapData, pcg => pcg.Children);
            surfaceLineObserver = new RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine>(UpdateSurfaceLinesMapData, rpslc => rpslc);

            mapDataCollection = new MapDataCollection(MacroStabilityInwardsDataResources.MacroStabilityInwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            stochasticSoilModelsMapData = MacroStabilityInwardsMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = MacroStabilityInwardsMapDataFactory.CreateSurfaceLinesMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(stochasticSoilModelsMapData);
            mapDataCollection.Add(surfaceLinesMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(calculationsMapData);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as MacroStabilityInwardsFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    hydraulicBoundaryDatabaseObserver.Observable = null;
                    stochasticSoilModelsObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;
                    surfaceLineObserver.Observable = null;
                    surfaceLinesObserver.Observable = null;

                    ringtoetsMapControl.RemoveAllData();
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    hydraulicBoundaryDatabaseObserver.Observable = data.Parent.HydraulicBoundaryDatabase;
                    stochasticSoilModelsObserver.Observable = data.WrappedData.StochasticSoilModels;
                    calculationInputObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationGroupObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationObserver.Observable = data.WrappedData.CalculationsGroup;
                    surfaceLinesObserver.Observable = data.WrappedData.SurfaceLines;
                    surfaceLineObserver.Observable = data.WrappedData.SurfaceLines;

                    SetAllMapDataFeatures();

                    ringtoetsMapControl.SetAllData(mapDataCollection, data.Parent.BackgroundData);
                }
            }
        }

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
            hydraulicBoundaryDatabaseObserver.Dispose();
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

        private void UpdateMapData()
        {
            UpdateCalculationsMapData();
            UpdateHydraulicBoundaryLocationsMapData();
            UpdateReferenceLineMapData();

            UpdateSectionsMapData();
            UpdateSurfaceLinesMapData();
            UpdateStochasticSoilModelsMapData();
        }

        private void SetAllMapDataFeatures()
        {
            SetCalculationsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetReferenceLineMapData();

            SetSectionsMapData();
            SetSurfaceLinesMapData();
            SetStochasticSoilModelsMapData();
        }

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations =
                data.WrappedData.CalculationsGroup.GetCalculations().Cast<MacroStabilityInwardsCalculationScenario>();
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
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(hydraulicBoundaryDatabase);
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
            ReferenceLine referenceLine = data.Parent.ReferenceLine;
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
        }

        #endregion

        #region Sections MapData

        private void UpdateSectionsMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;

            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
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
            MacroStabilityInwardsSurfaceLineCollection macroStabilityInwardsSurfaceLines = data.WrappedData.SurfaceLines;
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
            StochasticSoilModelCollection stochasticSoilModels = data.WrappedData.StochasticSoilModels;
            stochasticSoilModelsMapData.Features = MacroStabilityInwardsMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels.ToArray());
        }

        #endregion
    }
}