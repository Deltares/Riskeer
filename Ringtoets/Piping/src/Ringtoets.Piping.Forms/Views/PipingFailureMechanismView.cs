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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.Factories;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer surfaceLinesObserver;
        private readonly Observer stochasticSoilModelsObserver;

        private readonly RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingCalculationScenario> calculationObserver;
        private readonly RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine> surfaceLineObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapLineData stochasticSoilModelsMapData;
        private readonly MapLineData surfaceLinesMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData calculationsMapData;

        private PipingFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show data for.</param>
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

            failureMechanismObserver = new Observer(UpdateMapData)
            {
                Observable = failureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateMapData)
            {
                Observable = assessmentSection
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
            surfaceLinesObserver = new Observer(UpdateSurfaceLinesMapData)
            {
                Observable = failureMechanism.SurfaceLines
            };
            stochasticSoilModelsObserver = new Observer(UpdateStochasticSoilModelsMapData)
            {
                Observable = failureMechanism.StochasticSoilModels
            };

            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(
                UpdateHydraulicBoundaryLocationsMapData, hbl => hbl)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
            calculationInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculationScenario>().Select(pc => pc.InputParameters)))
            {
                Observable = failureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, PipingCalculationScenario>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };
            surfaceLineObserver = new RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine>(UpdateSurfaceLinesMapData, rpslc => rpslc)
            {
                Observable = failureMechanism.SurfaceLines
            };

            mapDataCollection = new MapDataCollection(PipingDataResources.PipingFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            stochasticSoilModelsMapData = RingtoetsMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = RingtoetsMapDataFactory.CreateSurfaceLinesMapData();
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

            SetAllMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, AssessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public PipingFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

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
                    hydraulicBoundaryLocationsObserver.Observable = null;
                    hydraulicBoundaryLocationObserver.Observable = null;
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
                    hydraulicBoundaryLocationsObserver.Observable = data.Parent.HydraulicBoundaryDatabase.Locations;
                    hydraulicBoundaryLocationObserver.Observable = data.Parent.HydraulicBoundaryDatabase.Locations;
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
            hydraulicBoundaryLocationsObserver.Dispose();
            hydraulicBoundaryLocationObserver.Dispose();
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
            IEnumerable<PipingCalculationScenario> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().Cast<PipingCalculationScenario>();
            calculationsMapData.Features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
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
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = AssessmentSection.HydraulicBoundaryDatabase;
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
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
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
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;

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