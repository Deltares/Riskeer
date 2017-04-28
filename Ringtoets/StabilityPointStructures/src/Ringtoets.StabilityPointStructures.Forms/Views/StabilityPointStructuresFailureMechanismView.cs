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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Views;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using StabilityPointStructuresDataResources = Ringtoets.StabilityPointStructures.Data.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a stability point structures failure mechanism.
    /// </summary>
    public partial class StabilityPointStructuresFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private readonly Observer foreshoreProfilesObserver;
        private readonly Observer structuresObserver;

        private readonly RecursiveObserver<CalculationGroup, StabilityPointStructuresInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, StructuresCalculation<StabilityPointStructuresInput>> calculationObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapPointData structuresMapData;
        private readonly MapLineData calculationsMapData;

        private StabilityPointStructuresFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresFailureMechanismView"/>.
        /// </summary>
        public StabilityPointStructuresFailureMechanismView()
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
            hydraulicBoundaryDatabaseObserver = new Observer(UpdateMapData);
            foreshoreProfilesObserver = new Observer(UpdateMapData);
            structuresObserver = new Observer(UpdateMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, StabilityPointStructuresInput>(
                UpdateMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<StructuresCalculation<StabilityPointStructuresInput>>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, StructuresCalculation<StabilityPointStructuresInput>>(UpdateMapData, pcg => pcg.Children);

            mapDataCollection = new MapDataCollection(StabilityPointStructuresDataResources.StabilityPointStructuresFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            structuresMapData = RingtoetsMapDataFactory.CreateStructuresMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(structuresMapData);
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
                data = value as StabilityPointStructuresFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    hydraulicBoundaryDatabaseObserver.Observable = null;
                    foreshoreProfilesObserver.Observable = null;
                    structuresObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;

                    ringtoetsMapControl.RemoveAllData();
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    hydraulicBoundaryDatabaseObserver.Observable = data.Parent.HydraulicBoundaryDatabase;
                    foreshoreProfilesObserver.Observable = data.WrappedData.ForeshoreProfiles;
                    structuresObserver.Observable = data.WrappedData.StabilityPointStructures;
                    calculationInputObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationGroupObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationObserver.Observable = data.WrappedData.CalculationsGroup;

                    SetMapDataFeatures();

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
            foreshoreProfilesObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            structuresObserver.Dispose();

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
            hydraulicBoundaryLocationsMapData.NotifyObservers();
            foreshoreProfilesMapData.NotifyObservers();
            structuresMapData.NotifyObservers();
            calculationsMapData.NotifyObservers();
        }

        private void SetMapDataFeatures()
        {
            ReferenceLine referenceLine = data.Parent.ReferenceLine;
            IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
            IEnumerable<ForeshoreProfile> foreshoreProfiles = data.WrappedData.ForeshoreProfiles;
            IEnumerable<StabilityPointStructure> structures = data.WrappedData.StabilityPointStructures;
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations =
                data.WrappedData.CalculationsGroup.GetCalculations().Cast<StructuresCalculation<StabilityPointStructuresInput>>();

            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(hydraulicBoundaryDatabase);
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
            structuresMapData.Features = RingtoetsMapDataFeaturesFactory.CreateStructuresFeatures(structures);
            calculationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateStructureCalculationsFeatures
                <StabilityPointStructuresInput, StabilityPointStructure>(calculations);
        }
    }
}